using LibraryManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Api.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(b => b.Title).HasMaxLength(200);
            builder.Property(b => b.ISBN).HasMaxLength(30);
            builder.Property(b => b.Language).HasMaxLength(50);
            builder.Property(b => b.Edition).HasMaxLength(50);
            builder.Property(b => b.Summary).HasMaxLength(1000);
            builder.HasIndex(b => b.ISBN).IsUnique();

            builder.HasMany(b => b.BorrowTransactions).WithOne(bt => bt.Book).HasForeignKey(bt => bt.BookId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(b => b.BookAuthors).WithOne(ba => ba.Book).HasForeignKey(ba => ba.BookId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
