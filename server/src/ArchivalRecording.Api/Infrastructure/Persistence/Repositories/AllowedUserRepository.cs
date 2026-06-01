using ArchivalRecording.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArchivalRecording.Api.Infrastructure.Persistence.Repositories;

public class AllowedUserRepository(AppDbContext db) : IAllowedUserRepository
{
    public Task<bool> IsEmailAllowedAsync(string email)
        => db.AllowedUsers.AnyAsync(u => u.Email == email);
}
