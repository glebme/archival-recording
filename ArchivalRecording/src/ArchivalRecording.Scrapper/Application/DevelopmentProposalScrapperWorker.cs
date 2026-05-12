using Microsoft.Extensions.Options;
using NCrontab;

namespace DevelopmentProposalScrapper.Application;

public class DevelopmentProposalScrapperWorker : BackgroundService
{
    private readonly ILogger<DevelopmentProposalScrapperWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CrontabSchedule _schedule;
    private readonly DevelopmentProposalScrapperSettings _settings;
    private readonly TimeProvider _timeProvider;

    private DateTime _nextRun;

    public DevelopmentProposalScrapperWorker(
        ILogger<DevelopmentProposalScrapperWorker> logger,
        IOptions<DevelopmentProposalScrapperSettings> options,
        IServiceScopeFactory serviceScopeFactory,
        TimeProvider timeProvider)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _timeProvider = timeProvider;
        _settings = options.Value ?? throw new ArgumentNullException(nameof(options), "DevelopmentProposalScrapperSettings cannot be null.");

        _schedule = CrontabSchedule.Parse(_settings.CronSchedule);
        _nextRun = _schedule.GetNextOccurrence(_timeProvider.GetLocalNow().DateTime);
        _logger.LogInformation("Worker scheduled to run at: {time}", _nextRun);
        _logger.LogInformation("Starting Development Proposal Scrapper...");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_timeProvider.GetLocalNow().DateTime >= _nextRun && _settings.IsEnabled)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                using var scope = _serviceScopeFactory.CreateScope();
                var scrapperService = scope.ServiceProvider.GetRequiredService<IDevelopmentProposalScrapperService>();

                try
                {
                    await scrapperService.FetchDaApplications(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scrapper service encountered an unhandled exception");
                }
            }

            _nextRun = _schedule.GetNextOccurrence(_timeProvider.GetLocalNow().DateTime);
            var delay = _nextRun - _timeProvider.GetLocalNow().DateTime;

            await Task.Delay(delay, stoppingToken);
        }
    }
}
