using DevelopmentProposalScrapper;
using DevelopmentProposalScrapper.Services;
using Refit;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// Configure settings
var configuration = builder.Configuration;
configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.Configure<DevelopmentProposalScrapperSettings>(configuration.GetSection("ArchivaRecording.Scrapper"));

//  API Clients
builder.Services.AddRefitClient<IOnlineDAApi>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri("https://api.apps1.nsw.gov.au");
});
builder.Services.AddScoped<IOnlineDAClient, OnlineDaClient>();

var host = builder.Build();
host.Run();
