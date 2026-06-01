using ArchivalRecording.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArchivalRecording.Api.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AllowedUser> AllowedUsers => Set<AllowedUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
