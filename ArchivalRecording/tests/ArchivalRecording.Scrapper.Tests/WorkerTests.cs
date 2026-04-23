using DevelopmentProposalScrapper.Application;
using DevelopmentProposalScrapper.Domain.Repositories;
using DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;
using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;
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
    private Mock<ILogger<DevelopmentProposalScrapper.Application.DevelopmentProposalScrapperWorker>> _loggerMock;
    private Mock<IDevelopmentProposalScrapperService> _scrapperServiceMock;
    private Mock<IServiceScopeFactory> _scopeFactoryMock;
    private Mock<IServiceScope> _scopeMock;
    private Mock<IServiceProvider> _serviceProviderMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<DevelopmentProposalScrapperWorker>>();
        _optionsMock = new Mock<IOptions<DevelopmentProposalScrapperSettings>>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _scrapperServiceMock = new Mock<IDevelopmentProposalScrapperService>();
    }

    [Test]
    public async Task Worker_RunsAtNextOccurrence_WhenEnabled()
    {
        // Arrange
        var worker = CreateWorker(true, "*/1 * * * *"); // Every 1 minutes
        _scrapperServiceMock.Setup(s => s.FetchDaApplications()).ReturnsAsync(5).Verifiable(); // Mock the service to return a count of 5 applications

        Task.Delay(TimeSpan.FromMinutes(1)).Wait(); // Wait for 1 minute to ensure the datetime is after the 1 minute
        
        // Act
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(10)); // Allow the worker to run for 10 seconds
        await worker.StartAsync(cts.Token);

        // Assert
        _scrapperServiceMock.Verify();
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
        _scrapperServiceMock.VerifyNoOtherCalls();
    }
    
    private DevelopmentProposalScrapper.Application.DevelopmentProposalScrapperWorker CreateWorker(bool isEnabled, string cronSchedule)
    {
        var settings = new DevelopmentProposalScrapperSettings
        {
            IsEnabled = isEnabled,
            CronSchedule = cronSchedule
        };

        _optionsMock.Setup(o => o.Value).Returns(settings);
        _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(s => s.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IDevelopmentProposalScrapperService))).Returns(_scrapperServiceMock.Object);
        
        return new DevelopmentProposalScrapper.Application.DevelopmentProposalScrapperWorker(_loggerMock.Object, _optionsMock.Object, _scopeFactoryMock.Object);
    }
}