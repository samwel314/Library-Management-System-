using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Entities;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Services
{
    public class MemberService : IMemberService
    {
        private readonly LibraryDbContext _db;

        public MemberService(LibraryDbContext db)
        {
            _db = db;
        }

        public async Task<ResultT<int?>> CreateAsync(MemberRequestDto requestDto, CancellationToken cancellation)
        {
            requestDto.Name = requestDto.Name.Trim();
            requestDto.Phone = requestDto.Phone.Trim();
            requestDto.Email = requestDto.Email.Trim();

            var member = new Member
            {
                Name = requestDto.Name,
                Phone = requestDto.Phone,
                Email = requestDto.Email,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Members.AddAsync(member, cancellation);
            await _db.SaveChangesAsync(cancellation);

            return ResultT<int?>.Success(member.Id, "Member created successfully");
        }

        public async Task<ResultT<IEnumerable<MemberDto>>> GetAllAsync(CancellationToken cancellation)
        {
            var members = await _db.Members
                .AsNoTracking()
                .Select(m => new MemberDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Phone = m.Phone,
                    Email = m.Email,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync(cancellation);

            return ResultT<IEnumerable<MemberDto>>.Success(members);
        }

        public async Task<ResultT<MemberDto>> GetByIdAsync(int id, CancellationToken cancellation)
        {
            var member = await _db.Members
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MemberDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Phone = m.Phone,
                    Email = m.Email,
                    CreatedAt = m.CreatedAt
                })
                .FirstOrDefaultAsync(cancellation);

            if (member == null)
                return ResultT<MemberDto>.Failure("Member not found", ErrorType.NotFound);

            return ResultT<MemberDto>.Success(member);
        }

        public async Task<Result> UpdateAsync(int id, MemberRequestDto requestDto, CancellationToken cancellation)
        {
            requestDto.Name = requestDto.Name.Trim();
            requestDto.Phone = requestDto.Phone.Trim();
            requestDto.Email = requestDto.Email.Trim();

            var member = await _db.Members.FindAsync(id, cancellation);

            if (member == null)
                return Result.Failure("Member not found", ErrorType.NotFound);

            member.Name = requestDto.Name;
            member.Phone = requestDto.Phone;
            member.Email = requestDto.Email;

            await _db.SaveChangesAsync(cancellation);

            return Result.Success("Member updated successfully");
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellation)
        {
            var member = await _db.Members.FindAsync(id, cancellation);

            if (member == null)
                return Result.Failure("Member not found", ErrorType.NotFound);

            var hasBorrowHistory = await _db.BorrowTransactions
                .AnyAsync(b => b.MemberId == id, cancellation);

            if (hasBorrowHistory)
                return Result.Failure("Cannot delete member because they have borrow history.", ErrorType.Validation);

            _db.Members.Remove(member);
            await _db.SaveChangesAsync(cancellation);

            return Result.Success("Member deleted successfully");
        }
    }
}
