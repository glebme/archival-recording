using ArchivalRecording.Api.Application.Auth;
using ArchivalRecording.Api.Domain.Repositories;
using ArchivalRecording.Api.Infrastructure.External.Clients.Google;
using ArchivalRecording.Api.Infrastructure.Persistence;
using ArchivalRecording.Api.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArchivalRecording.Api.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("PostgresConnectionString")));

        services.AddHttpClient();
        services.AddScoped<IGoogleAuthClient, GoogleAuthClient>();
        services.AddScoped<IAllowedUserRepository, AllowedUserRepository>();
        services.AddSingleton<IJwtService, JwtService>();

        return services;
    }
}
