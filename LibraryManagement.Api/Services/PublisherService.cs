using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Entities;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagement.Api.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly LibraryDbContext _db;

        public PublisherService(LibraryDbContext db)
        {
            _db = db;
        }

        public async Task<ResultT<int?>> CreateAsync(PublisherRequestDto requestDto, CancellationToken cancellation)
        {
            requestDto.Name = requestDto.Name.Trim();
            requestDto.Country = requestDto.Country?.Trim();
            requestDto.ContactInfo = requestDto.ContactInfo?.Trim();
            var isSameNameExist = await _db.Publishers.AnyAsync(c => c.Name == requestDto.Name, cancellation);
            if (isSameNameExist)
                return ResultT<int?>.Failure("Publisher name already exists.", ErrorType.Conflict);
            var publisher = new Publisher
            {
                Name = requestDto.Name,
                ContactInfo = requestDto.ContactInfo,   
                Country = requestDto.Country,   
            };
            await _db.Publishers.AddAsync(publisher , cancellation);
            await _db.SaveChangesAsync(cancellation);
            return ResultT<int?>.Success(publisher.Id, "Publisher created successfully");
        }
        public async Task<ResultT<IEnumerable<PublisherLookupDto>>> GetAllLookupAsync(CancellationToken cancellation)
        {
            var publishers   = await _db.Publishers.AsNoTracking().Select(c => new PublisherLookupDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToListAsync(cancellation);
            return ResultT<IEnumerable<PublisherLookupDto>>.Success(publishers);
        }
        public async Task<ResultT<IEnumerable<PublisherDto>>> GetAllAsync(CancellationToken cancellation)
        {
            var publishers = await _db.Publishers.AsNoTracking().Select(c => new PublisherDto
            {
                Id = c.Id,
                Name = c.Name,
                Country = c.Country , 
                ContactInfo = c.ContactInfo,    
            }).ToListAsync(cancellation);
            return ResultT<IEnumerable<PublisherDto>>.Success(publishers);
        }
        public async Task<ResultT<PublisherDto>> GetByIdAsync(int id, CancellationToken cancellation)
        {
            var publisher = await _db.Publishers.AsNoTracking().Select(c => new PublisherDto
            {
                Id = c.Id,
                Name = c.Name,
                Country = c.Country,
                ContactInfo = c.ContactInfo,
            }).FirstOrDefaultAsync(c => c.Id == id, cancellation);
            if (publisher == null)
                return ResultT<PublisherDto>.Failure("Publisher not found", ErrorType.NotFound);

            return ResultT<PublisherDto>.Success(publisher);
        }
        public async Task<Result> UpdateAsync(int id, PublisherRequestDto requestDto, CancellationToken cancellation)
        {
            requestDto.Name = requestDto.Name.Trim();
            requestDto.Country = requestDto.Country?.Trim();
            requestDto.ContactInfo = requestDto.ContactInfo?.Trim();
            var publisher = await _db.Publishers.FindAsync(id, cancellation);
            if (publisher == null)
                return Result.Failure($"Publisher not found", ErrorType.NotFound);

            var isSameNameExist = await
                _db.Publishers.AnyAsync(c => c.Name == requestDto.Name && c.Id != id, cancellation);
            if (isSameNameExist)
                return Result.Failure("Publisher name already exists.", ErrorType.Conflict);
            publisher.Name = requestDto.Name;
            publisher.ContactInfo = requestDto.ContactInfo;
            publisher.Country = requestDto.Country;
            await _db.SaveChangesAsync(cancellation);
            return Result.Success("Publisher updated successfully");
        }
        public async Task<Result> DeleteAsync(int id, CancellationToken cancellation)
        {
            var publisher = await _db.Publishers.FindAsync(id, cancellation);
            if (publisher == null)
                return Result.Failure($"Publisher not found", ErrorType.NotFound);
            var hasBooks = await _db.Books.AnyAsync(b => b.PublisherId == id, cancellation);
            if (hasBooks)
                return Result.Failure($"Cannot delete publisher because it has associated books.", ErrorType.Validation);
            _db.Publishers.Remove(publisher);
            await _db.SaveChangesAsync(cancellation);

            return Result.Success("Publisher deleted successfuly");
        }
    }
}
