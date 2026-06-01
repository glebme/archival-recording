using System.Net.Http.Headers;
using ArchivalRecording.Api.Infrastructure.External.Models.Google;

namespace ArchivalRecording.Api.Infrastructure.External.Clients.Google;

public class GoogleAuthClient(IHttpClientFactory httpClientFactory) : IGoogleAuthClient
{
    public async Task<GoogleUserInfo?> GetUserInfoAsync(string accessToken)
    {
        using var http = httpClientFactory.CreateClient();
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        return await http.GetFromJsonAsync<GoogleUserInfo>(
            "https://www.googleapis.com/oauth2/v3/userinfo");
    }
}
