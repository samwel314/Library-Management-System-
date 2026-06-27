using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Api.Entities
{
    public class UserActivityLog
    {
        public int Id { get; set; }
        public string ? UserId { get; set; } 
        public IdentityUser? User { get; set; } 
        public string Action { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
