using DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;
using Microsoft.Extensions.Options;
using NCrontab;

namespace DevelopmentProposalScrapper.Application;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly DevelopmentProposalScrapperSettings _settings;
    private readonly CrontabSchedule _schedule;

    private DateTime _nextRun;

    public Worker(ILogger<Worker> logger, IOptions<DevelopmentProposalScrapperSettings> options, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _settings = options.Value;
        _serviceScopeFactory = serviceScopeFactory;
        if (_settings == null)
        {
            throw new ArgumentNullException(nameof(options), "DevelopmentProposalScrapperSettings cannot be null.");
        }

        _schedule = CrontabSchedule.Parse(_settings.CronSchedule);
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        _logger.LogInformation("Worker scheduled to run at: {time}", _nextRun);
        _logger.LogInformation("Starting Development Proposal Scrapper...");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // if (DateTime.Now >= _nextRun && _settings.IsEnabled)
            // {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                using var scope = _serviceScopeFactory.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<IOnlineDAClient>();
                
                var result = await client.GetOnlineDARecordsAsync(5, 14);

                if (result is { IsSuccess: true, Model: not null })
                {
                    var records = result.Model!;
                    _logger.LogInformation("Fetched {count} records.", records.TotalCount);
                }
                else
                {
                    _logger.LogError("Failed to fetch records: {error}", result.ErrorMessage);
                }
            // }

            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            var delay = _nextRun - DateTime.Now;

            await Task.Delay(delay, stoppingToken);
        }
    }
}
