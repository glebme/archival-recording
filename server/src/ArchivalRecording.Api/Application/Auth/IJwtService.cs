namespace ArchivalRecording.Api.Application.Auth;

public interface IJwtService
{
    string GenerateToken(string sub, string email, string name, string? picture);
}
