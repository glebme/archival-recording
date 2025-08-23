using DevelopmentProposalScrapper;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// Configure settings
var configuration = builder.Configuration;
configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.Configure<DevelopmentProposalScrapperSettings>(configuration.GetSection("DevelopmentProposalScrapper"));

var host = builder.Build();
host.Run();