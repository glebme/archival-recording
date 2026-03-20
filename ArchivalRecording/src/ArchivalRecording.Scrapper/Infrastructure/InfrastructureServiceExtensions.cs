using DevelopmentProposalScrapper.Domain.Repositories;
using DevelopmentProposalScrapper.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevelopmentProposalScrapper.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsl => npgsl.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            )
        );
        
        services.AddScoped<IDevelopmentApplicationRepository, DevelopmentApplicationRepository>();
        
        return services;
    }
}
