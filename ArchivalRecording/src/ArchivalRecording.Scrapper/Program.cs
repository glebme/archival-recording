using System.Text.Json;
using System.Text.Json.Serialization;
using DevelopmentProposalScrapper.Application;
using DevelopmentProposalScrapper.Domain.Entities;
using DevelopmentProposalScrapper.Infrastructure;
using DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;
using Refit;
using Shared;
using Shared.JsonConverters;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DevelopmentProposalScrapperWorker>();

// Configure settings
var configuration = builder.Configuration;
var env = builder.Environment;
configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Services.Configure<DevelopmentProposalScrapperSettings>(
    configuration.GetSection("WorkerSchedule:DevelopmentProposalScrapper"));

//  API Clients
builder.Services.AddTransient<HttpLoggingHandler>();
builder.Services.AddRefitClient<IOnlineDAApi>(new RefitSettings
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new DescriptionEnumConverter<ApplicationType>(),
                new DescriptionEnumConverter<ApplicationStatus>(),
            }
        })
    })
    .ConfigureHttpClient(c => { c.BaseAddress = new Uri("https://api.apps1.nsw.gov.au"); })
    .AddHttpMessageHandler<HttpLoggingHandler>();
builder.Services.AddScoped<IOnlineDAClient, OnlineDAClient>();

// Infrastructure
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddSingleton(TimeProvider.System);

// Services
builder.Services.AddScoped<IDevelopmentProposalScrapperService, DevelopmentProposalScrapperService>();

var host = builder.Build();

host.Run();
