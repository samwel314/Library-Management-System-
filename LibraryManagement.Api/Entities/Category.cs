namespace LibraryManagement.Api.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
