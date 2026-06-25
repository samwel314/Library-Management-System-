using LibraryManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Api.Data.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(b => b.Name).HasMaxLength(50);
            builder.Property(b => b.Email).HasMaxLength(50);
            builder.Property(b => b.Phone).HasMaxLength(11);
            builder.HasMany(b => b.BorrowTransactions).WithOne
                (bt => bt.Member).HasForeignKey(bt => bt.MemberId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
