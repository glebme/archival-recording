using ArchivalRecording.Api.Infrastructure.External.Models.Google;

namespace ArchivalRecording.Api.Infrastructure.External.Clients.Google;

public interface IGoogleAuthClient
{
    Task<GoogleUserInfo?> GetUserInfoAsync(string accessToken);
}
