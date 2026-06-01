using System.Text.Json.Serialization;

namespace ArchivalRecording.Api.Infrastructure.External.Models.Google;

public record GoogleUserInfo(
    [property: JsonPropertyName("sub")] string Sub,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("email_verified")] bool EmailVerified,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("picture")] string? Picture);
