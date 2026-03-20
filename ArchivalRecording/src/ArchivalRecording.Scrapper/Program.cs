using System.Text.Json;
using System.Text.Json.Serialization;
using DevelopmentProposalScrapper;
using DevelopmentProposalScrapper.Models.OnlineDA;
using DevelopmentProposalScrapper.Services.OnlineDA;
using Refit;
using Shared;
using Shared.JsonConverters;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

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
            Converters =
            {
                new DescriptionEnumConverter<ApplicationType>(),
                new DescriptionEnumConverter<ApplicationStatus>(),
            }
        })
    })
    .ConfigureHttpClient(c => { c.BaseAddress = new Uri("https://api.apps1.nsw.gov.au"); })
    .AddHttpMessageHandler<HttpLoggingHandler>();
builder.Services.AddScoped<IOnlineDAClient, OnlineDaClient>();

var host = builder.Build();

host.Run();
