using DevelopmentProposalScrapper.Domain.Repositories;
using DevelopmentProposalScrapper.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;

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

        var retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder()
                    .Handle<TimeoutException>()
                    .Handle<InvalidOperationException>()
            })
            .Build();

        services.AddSingleton(retryPipeline);
    }
}
