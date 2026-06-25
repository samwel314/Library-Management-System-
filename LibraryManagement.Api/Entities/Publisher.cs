namespace LibraryManagement.Api.Entities
{
    public class Publisher
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Country { get; set; }
        public string? ContactInfo { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}