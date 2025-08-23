using DevelopmentProposalScrapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevelopmentProposalScrapperTests;

[TestFixture]
public class WorkerTests
{
    private Mock<ILogger<Worker>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<Worker>>();
    }

    [Test]
    public async Task Worker_RunsAtNextOccurrence_WhenEnabled()
    {
        // Arrange
        var worker = CreateWorker(true, "*/1 * * * *"); // Every 1 minutes

        Task.Delay(TimeSpan.FromMinutes(1)).Wait(); // Wait for 1 minute to ensure the datetime is after the 1 minute
        
        // Act
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(10)); // Allow the worker to run for 10 seconds
        await worker.StartAsync(cts.Token);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce,
            "Worker should log at least once at the scheduled time."
        );
    }
    
    [Test]
    public async Task Worker_DoesNotRun_WhenDisabled()
    {
        // Arrange
        var worker = CreateWorker(false, "*/1 * * * *"); // Every 5 minutes 
        
        // Act
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(10)); // Allow the worker to run for
        await worker.StartAsync(cts.Token);
        
        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never,
            "Worker should not log when disabled."
        );
    }
    
    private Worker CreateWorker(bool isEnabled, string cronSchedule)
    {
        var settings = new DevelopmentProposalScrapperSettings
        {
            IsEnabled = isEnabled,
            CronSchedule = cronSchedule
        };
        
        return new Worker(_loggerMock.Object, settings);
    }
}