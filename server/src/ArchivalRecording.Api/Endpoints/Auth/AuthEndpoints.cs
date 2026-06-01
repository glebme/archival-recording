using System.IdentityModel.Tokens.Jwt;
using ArchivalRecording.Api.Application.Auth;
using ArchivalRecording.Api.Domain.Repositories;
using ArchivalRecording.Api.Infrastructure.External.Clients.Google;
using Microsoft.AspNetCore.Authorization;

namespace ArchivalRecording.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/auth");

        auth.MapPost("/google", async (
            HttpContext ctx,
            GoogleAuthRequest request,
            IJwtService jwtService,
            IConfiguration config,
            IGoogleAuthClient googleAuthClient,
            IAllowedUserRepository allowedUsers,
            ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger(nameof(AuthEndpoints));

            try
            {
                var googleUser = await googleAuthClient.GetUserInfoAsync(request.AccessToken);

                if (googleUser is null || !googleUser.EmailVerified)
                {
                    logger.LogWarning("Google userinfo returned null or unverified email.");
                    return Results.Unauthorized();
                }

                if (!await allowedUsers.IsEmailAllowedAsync(googleUser.Email))
                {
                    logger.LogWarning("Login rejected for {Email}: not in allowed users list.", googleUser.Email);
                    return Results.Forbid();
                }

                var expirationMinutes = int.TryParse(
                    config["Authentication:Jwt:ExpirationMinutes"], out var mins) ? mins : 60;

                var token = jwtService.GenerateToken(
                    sub: googleUser.Sub,
                    email: googleUser.Email,
                    name: googleUser.Name,
                    picture: googleUser.Picture);

                ctx.Response.Cookies.Append("access_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = ctx.Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes),
                });

                return Results.Ok(new UserInfo(googleUser.Sub, googleUser.Email, googleUser.Name, googleUser.Picture));
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Google userinfo request failed.");
                return Results.Unauthorized();
            }
        });

        auth.MapGet("/me",
            [Authorize]
            (HttpContext ctx) =>
            {
                var user = ctx.User;
                return Results.Ok(new UserInfo(
                    Sub: user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "",
                    Email: user.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? "",
                    Name: user.FindFirst(JwtRegisteredClaimNames.Name)?.Value ?? "",
                    Picture: user.FindFirst("picture")?.Value));
            });

        auth.MapPost("/logout",
            [Authorize]
            (HttpContext ctx) =>
            {
                ctx.Response.Cookies.Delete("access_token");
                return Results.Ok(new { message = "Logged out." });
            });
    }
}
