using DevelopmentProposalScrapper.Domain.Repositories;
using DevelopmentProposalScrapper.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevelopmentProposalScrapper.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("PostgresConnectionString"),
                npgsl => npgsl.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            )
        );
        
        services.AddScoped<IDevelopmentApplicationRepository, DevelopmentApplicationRepository>();
    }
}
