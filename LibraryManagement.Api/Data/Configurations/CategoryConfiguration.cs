using LibraryManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Api.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name).HasMaxLength(50);
            builder.HasMany(c => c.Books).WithOne(b => b.Category).HasForeignKey(b => b.CategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(c => c.SubCategories).WithOne(c=> c.ParentCategory).
                HasForeignKey(c => c.ParentCategoryId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
