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

        _logger.LogInformation("WorkerStarted {@WorkerConfig}", new
        {
            _settings.CronSchedule,
            NextRun = _nextRun,
            _settings.IsEnabled
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_timeProvider.GetLocalNow().DateTime >= _nextRun && _settings.IsEnabled)
            {
                var cycleEvent = new ScrapeCycleEvent
                {
                    Councils = _settings.Councils,
                    LookbackFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-_settings.LookbackDays)),
                    PageSize = 100
                };

                using var scope = _serviceScopeFactory.CreateScope();
                var scrapperService = scope.ServiceProvider.GetRequiredService<IDevelopmentProposalScrapperService>();

                try
                {
                    await scrapperService.FetchDaApplications(cycleEvent, stoppingToken);
                    cycleEvent.Outcome = cycleEvent.PagesFailed > 0 ? "partial" : "success";
                }
                catch (Exception ex)
                {
                    cycleEvent.Outcome = "failed";
                    cycleEvent.Errors.Add(new PageError(0, "UnhandledException", ex.Message));
                }
                finally
                {
                    cycleEvent.FinishedAt = DateTimeOffset.UtcNow;
                    _logger.LogInformation("ScrapeCycleCompleted {@ScrapeCycle}", cycleEvent);
                }
            }

            _nextRun = _schedule.GetNextOccurrence(_timeProvider.GetLocalNow().DateTime);
            var delay = _nextRun - _timeProvider.GetLocalNow().DateTime;

            await Task.Delay(delay, stoppingToken);
        }
    }
}
