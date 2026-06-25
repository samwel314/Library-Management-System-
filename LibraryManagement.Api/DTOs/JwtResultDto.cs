namespace LibraryManagement.Api.DTOs
{
    public class JwtResultDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
