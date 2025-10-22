using DevelopmentProposalScrapper.Services;
using DevelopmentProposalScrapper.Services.OnlineDA;
using Microsoft.Extensions.Options;
using NCrontab;

namespace DevelopmentProposalScrapper;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DevelopmentProposalScrapperSettings _settings;
    private readonly IOnlineDAClient _client;
    private readonly CrontabSchedule _schedule;

    private DateTime _nextRun;

    public Worker(ILogger<Worker> logger, IOptions<DevelopmentProposalScrapperSettings> options, IOnlineDAClient client)
    {
        _logger = logger;
        _settings = options.Value;
        _client = client;
        if (_settings == null)
        {
            throw new ArgumentNullException(nameof(options), "DevelopmentProposalScrapperSettings cannot be null.");
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
                var result = await _client.GetOnlineDARecordsAsync(5, 14);

                if (result is { IsSuccess: true, Model: not null })
                {
                    var records = result.Model!;
                    _logger.LogInformation("Fetched {count} records.", records.TotalPages * records.PageSize);
                }
                else
                {
                    _logger.LogError("Failed to fetch records: {error}", result.ErrorMessage);
                }
            }

            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            var delay = _nextRun - DateTime.Now;

            await Task.Delay(delay, stoppingToken);
        }
    }
}
