using ArchivalRecording.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArchivalRecording.Api.Infrastructure.Persistence.Configurations;

public class AllowedUserConfiguration : IEntityTypeConfiguration<AllowedUser>
{
    public void Configure(EntityTypeBuilder<AllowedUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
    }
}
