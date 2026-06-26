namespace LibraryManagement.Api.DTOs
{
    public class PublisherDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public string? ContactInfo { get; set; }
    }
}
