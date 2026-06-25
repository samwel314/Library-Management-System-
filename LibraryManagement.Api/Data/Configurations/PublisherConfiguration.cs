using LibraryManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Api.Data.Configurations
{
    public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {
            builder.Property(b => b.Name).HasMaxLength(50);
            builder.Property(b => b.Country).HasMaxLength(50);
            builder.Property(b => b.ContactInfo).HasMaxLength(200);
            builder.HasMany(b => b.Books).WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
