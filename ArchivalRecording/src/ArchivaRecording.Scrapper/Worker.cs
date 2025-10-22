using NCrontab;

namespace DevelopmentProposalScrapper;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DevelopmentProposalScrapperSettings _settings;
    private readonly CrontabSchedule _schedule;
    
    private DateTime _nextRun;

    public Worker(ILogger<Worker> logger, DevelopmentProposalScrapperSettings settings)
    {
        _logger = logger;
        _settings = settings;
        if (_settings == null)
        {
            throw new ArgumentNullException(nameof(settings), "DevelopmentProposalScrapperSettings cannot be null.");
        }
        
        _schedule = CrontabSchedule.Parse(_settings.CronSchedule);
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (DateTime.Now >= _nextRun && _settings.IsEnabled)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            var delay = _nextRun - DateTime.Now;
            
            await Task.Delay(delay, stoppingToken);
        }
    }
}