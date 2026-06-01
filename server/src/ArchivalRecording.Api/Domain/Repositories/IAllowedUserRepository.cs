namespace ArchivalRecording.Api.Domain.Repositories;

public interface IAllowedUserRepository
{
    Task<bool> IsEmailAllowedAsync(string email);
}
