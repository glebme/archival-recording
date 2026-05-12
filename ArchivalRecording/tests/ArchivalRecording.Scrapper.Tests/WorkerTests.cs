using DevelopmentProposalScrapper.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace DevelopmentProposalScrapperTests;

[TestFixture]
public class WorkerTests
{
    private Mock<IOptions<DevelopmentProposalScrapperSettings>> _optionsMock;
    private Mock<ILogger<DevelopmentProposalScrapperWorker>> _loggerMock;
    private Mock<IDevelopmentProposalScrapperService> _scrapperServiceMock;
    private Mock<IServiceScopeFactory> _scopeFactoryMock;
    private Mock<IServiceScope> _scopeMock;
    private Mock<IServiceProvider> _serviceProviderMock;
    private FakeTimeProvider _fakeTimeProvider;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<DevelopmentProposalScrapperWorker>>();
        _optionsMock = new Mock<IOptions<DevelopmentProposalScrapperSettings>>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _scrapperServiceMock = new Mock<IDevelopmentProposalScrapperService>();
        _fakeTimeProvider = new FakeTimeProvider();
    }

    [Test]
    public async Task Worker_CallsFetchDaApplications_WhenEnabled()
    {
        // Arrange
        var worker = CreateWorker(isEnabled: true, cronSchedule: "* * * * *");

        var serviceCalledTcs = new TaskCompletionSource();
        _scrapperServiceMock
            .Setup(s => s.FetchDaApplications(It.IsAny<ScrapeCycleEvent>(), It.IsAny<CancellationToken>()))
            .Callback<ScrapeCycleEvent, CancellationToken>((_, _) => serviceCalledTcs.TrySetResult())
            .ReturnsAsync(5);

        // Advance fake time so the cron condition is immediately true
        _fakeTimeProvider.Advance(TimeSpan.FromMinutes(2));

        // Act
        await worker.StartAsync(CancellationToken.None);
        await serviceCalledTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await worker.StopAsync(CancellationToken.None);

        // Assert
        _scrapperServiceMock.Verify(
            s => s.FetchDaApplications(It.IsAny<ScrapeCycleEvent>(), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce,
            "FetchDaApplications should be called when worker is enabled and schedule is due"
        );
    }

    [Test]
    public async Task Worker_DoesNotCallFetchDaApplications_WhenDisabled()
    {
        // Arrange
        var worker = CreateWorker(isEnabled: false, cronSchedule: "* * * * *");

        // Advance fake time so the cron condition would be true if enabled
        _fakeTimeProvider.Advance(TimeSpan.FromMinutes(2));

        // Act — run worker briefly then stop
        await worker.StartAsync(CancellationToken.None);
        await Task.Delay(100);
        await worker.StopAsync(CancellationToken.None);

        // Assert
        _scrapperServiceMock.Verify(
            s => s.FetchDaApplications(It.IsAny<ScrapeCycleEvent>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "FetchDaApplications should never be called when worker is disabled"
        );
    }

    [Test]
    public async Task Worker_EmitsWideCycleEvent_WhenServiceThrows()
    {
        // Arrange
        var worker = CreateWorker(isEnabled: true, cronSchedule: "* * * * *");

        var serviceCalledTcs = new TaskCompletionSource();
        _scrapperServiceMock
            .Setup(s => s.FetchDaApplications(It.IsAny<ScrapeCycleEvent>(), It.IsAny<CancellationToken>()))
            .Callback<ScrapeCycleEvent, CancellationToken>((_, _) => serviceCalledTcs.TrySetResult())
            .ThrowsAsync(new Exception("Service failed"));

        _fakeTimeProvider.Advance(TimeSpan.FromMinutes(2));

        // Act
        await worker.StartAsync(CancellationToken.None);
        await serviceCalledTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await Task.Delay(50); // brief pause for the finally block to execute
        await worker.StopAsync(CancellationToken.None);

        // Assert — worker should catch the exception and emit the wide event at Information level
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce,
            "Worker should emit a wide cycle event even when the service throws"
        );
    }

    private DevelopmentProposalScrapperWorker CreateWorker(bool isEnabled, string cronSchedule)
    {
        var settings = new DevelopmentProposalScrapperSettings
        {
            IsEnabled = isEnabled,
            CronSchedule = cronSchedule
        };

        _optionsMock.Setup(o => o.Value).Returns(settings);
        _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(s => s.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IDevelopmentProposalScrapperService)))
            .Returns(_scrapperServiceMock.Object);

        return new DevelopmentProposalScrapperWorker(
            _loggerMock.Object,
            _optionsMock.Object,
            _scopeFactoryMock.Object,
            _fakeTimeProvider);
    }
}
