using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Entities;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.EntityFrameworkCore;


namespace TLibraryManagement.Api.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly LibraryDbContext _db;

        public AuthorService(LibraryDbContext db)
        {
            _db = db;
        }

        public async Task<ResultT<int?>> CreateAsync(AuthorRequestDto requestDto, CancellationToken cancellation)
        {
            requestDto.Name = requestDto.Name.Trim();
            requestDto.Bio = requestDto.Bio?.Trim();
            var author = new Author
            {
                Name = requestDto.Name,
                Bio = requestDto.Bio,   
            };
          await  _db.Authors.AddAsync(author, cancellation);
            await _db.SaveChangesAsync(cancellation);
            return ResultT<int?>.Success(author.Id, "Author created successfully");
        }
        public async Task<ResultT<IEnumerable<AuthorLookupDto>>> GetAllLookupAsync(CancellationToken cancellation)
        {
            var authors   = await _db.Authors.AsNoTracking().Select(c => new AuthorLookupDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToListAsync(cancellation);
            return ResultT<IEnumerable<AuthorLookupDto>>.Success(authors);
        }
        public async Task<ResultT<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken cancellation)
        {
            var authors = await _db.Authors.AsNoTracking().Select(c => new AuthorDto
            {
                Id = c.Id,
                Name = c.Name,
                Bio = c.Bio ,   
            }).ToListAsync(cancellation);
            return ResultT<IEnumerable<AuthorDto>>.Success(authors);
        }
        public async Task<ResultT<AuthorDto>> GetByIdAsync(int id, CancellationToken cancellation)
        {
            var author = await _db.Authors.AsNoTracking().Select(c => new AuthorDto
            {
                Id = c.Id,
                Name = c.Name,
                Bio = c.Bio,
            }).FirstOrDefaultAsync(c => c.Id == id, cancellation);
            if (author == null)
                return ResultT<AuthorDto>.Failure("Author not found", ErrorType.NotFound);

            return ResultT<AuthorDto>.Success(author);
        }
        public async Task<Result> UpdateAsync(int id, AuthorRequestDto requestDto, CancellationToken cancellation)
        {
            requestDto.Name = requestDto.Name.Trim();
            requestDto.Bio = requestDto.Bio?.Trim();
            var author = await _db.Authors.FindAsync(id, cancellation);
            if (author == null)
                return Result.Failure($"Author not found", ErrorType.NotFound);

            author.Name = requestDto.Name;
            author.Bio = requestDto.Bio;
            await _db.SaveChangesAsync(cancellation);
            return Result.Success("Author updated successfully");
        }
        public async Task<Result> DeleteAsync(int id, CancellationToken cancellation)
        {
            var author = await _db.Authors.FindAsync(id, cancellation);
            if (author == null)
                return Result.Failure($"Author not found", ErrorType.NotFound);
            var hasBooks = await _db.BookAuthors.AnyAsync(b => b.AuthorId == id, cancellation);
            if (hasBooks)
                return Result.Failure($"Cannot delete Author because it has associated books.", ErrorType.Validation);
            _db.Authors.Remove(author);
            await _db.SaveChangesAsync(cancellation);

            return Result.Success("Author deleted successfully");
        }
    }
}
