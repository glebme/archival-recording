using DevelopmentProposalScrapper;
using DevelopmentProposalScrapper.Models.OnlineDA;
using DevelopmentProposalScrapper.Services.OnlineDA;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shared;

namespace DevelopmentProposalScrapperTests;

[TestFixture]
public class WorkerTests
{
    private Mock<IOptions<DevelopmentProposalScrapperSettings>> _optionsMock;
    private Mock<ILogger<Worker>> _loggerMock;
    private Mock<IOnlineDAClient> _client;
    private Mock<IServiceScopeFactory> _scopeFactoryMock;
    private Mock<IServiceScope> _scopeMock;
    private Mock<IServiceProvider> _serviceProviderMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<Worker>>();
        _optionsMock = new Mock<IOptions<DevelopmentProposalScrapperSettings>>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _client = new Mock<IOnlineDAClient>();
    }

    [Test]
    public async Task Worker_RunsAtNextOccurrence_WhenEnabled()
    {
        // Arrange
        var worker = CreateWorker(true, "*/1 * * * *"); // Every 1 minutes
        _client.Setup(client => client.GetOnlineDARecordsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(
            new Result<OnlineDAResponse>
            {
                IsSuccess = true,
                Model = new OnlineDAResponse
                {
                    TotalPages = 1,
                    PageSize = 10
                }
            });

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

        _optionsMock.Setup(o => o.Value).Returns(settings);
        _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(s => s.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IOnlineDAClient))).Returns(_client.Object);
        _serviceProviderMock.Setup(sp => sp.GetRequiredService<IOnlineDAClient>()).Returns(_client.Object); 
        
        return new Worker(_loggerMock.Object, _optionsMock.Object, _scopeFactoryMock.Object);
    }
}