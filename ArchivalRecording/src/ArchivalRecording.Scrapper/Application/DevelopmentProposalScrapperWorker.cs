using Microsoft.Extensions.Options;
using NCrontab;

namespace DevelopmentProposalScrapper.Application;

public class DevelopmentProposalScrapperWorker : BackgroundService
{
    private readonly ILogger<DevelopmentProposalScrapperWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CrontabSchedule _schedule;
    private readonly DevelopmentProposalScrapperSettings _settings;

    private DateTime _nextRun;

    public DevelopmentProposalScrapperWorker(ILogger<DevelopmentProposalScrapperWorker> logger, IOptions<DevelopmentProposalScrapperSettings> options, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _settings = options.Value ?? throw new ArgumentNullException(nameof(options), "DevelopmentProposalScrapperSettings cannot be null.");

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
                var scrapperService = scope.ServiceProvider.GetRequiredService<IDevelopmentProposalScrapperService>();

                _ = scrapperService.FetchDaApplications();
            // }

            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            var delay = _nextRun - DateTime.Now;

            await Task.Delay(delay, stoppingToken);
        }
    }
}
