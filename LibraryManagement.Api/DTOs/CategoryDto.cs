namespace LibraryManagement.Api.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public CategoryLookupDto? Parent { get; set; } = null!;
    }

}
