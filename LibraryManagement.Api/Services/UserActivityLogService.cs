using LibraryManagement.Api.Data;
using LibraryManagement.Api.Entities;
using LibraryManagement.Api.Services.Interfaces;


namespace LibraryManagement.Api.Services
{
    public class UserActivityLogService : IUserActivityLogService
    {
        private readonly LibraryDbContext _db;
        private readonly ICurrentUserService _currentUser;

        public UserActivityLogService(
            LibraryDbContext db,
            ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public async Task LogAsync(string action,  CancellationToken cancellation = default , bool saveChanges = false)
        {
            var log = new UserActivityLog
            {
                UserId = _currentUser.UserId,
                Action = action,
                CreatedAt = DateTime.UtcNow
            };
          await _db.UserActivityLogs.AddAsync(log); 
            if (saveChanges)
                await _db.SaveChangesAsync(cancellation);
        }
    }
}
