using DevelopmentProposalScrapper.Application;
using DevelopmentProposalScrapper.Domain.Repositories;
using DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;
using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;
using DevelopmentProposalScrapper.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using DomainDevelopmentApplication = DevelopmentProposalScrapper.Domain.Entities.DevelopmentApplication;
using OnlineDADevelopmentApplication = DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA.DevelopmentApplication;

namespace DevelopmentProposalScrapperTests;

[TestFixture]
public class DevelopmentProposalScrapperServiceTests
{
    private Mock<ILogger<IDevelopmentProposalScrapperService>> _loggerMock;
    private Mock<IOnlineDAClient> _onlineDaClientMock;
    private Mock<IDevelopmentApplicationRepository> _repositoryMock;
    private DevelopmentProposalScrapperService _service;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<IDevelopmentProposalScrapperService>>();
        _onlineDaClientMock = new Mock<IOnlineDAClient>();
        _repositoryMock = new Mock<IDevelopmentApplicationRepository>();
        _service = new DevelopmentProposalScrapperService(_loggerMock.Object, _onlineDaClientMock.Object, _repositoryMock.Object);
    }

    [Test]
    public async Task FetchDaApplications_ProcessesMultiplePages_OnPaginatedApiResponses()
    {
        // Arrange
        var page1Applications = new[]
        {
            CreateOnlineDAApplication("APP-001", "Inner West Council"),
            CreateOnlineDAApplication("APP-002", "Inner West Council")
        };

        var page2Applications = new[]
        {
            CreateOnlineDAApplication("APP-003", "City of Sydney Council"),
            CreateOnlineDAApplication("APP-004", "City of Sydney Council")
        };

        var page3Applications = new[]
        {
            CreateOnlineDAApplication("APP-005", "Waverley Council")
        };

        var page1Response = new OnlineDAResponse
        {
            PageNumber = 1,
            PageSize = 2,
            TotalPages = 3,
            TotalCount = 5,
            DevelopmentApplications = page1Applications
        };

        var page2Response = new OnlineDAResponse
        {
            PageNumber = 2,
            PageSize = 2,
            TotalPages = 3,
            TotalCount = 5,
            DevelopmentApplications = page2Applications
        };

        var page3Response = new OnlineDAResponse
        {
            PageNumber = 3,
            PageSize = 2,
            TotalPages = 3,
            TotalCount = 5,
            DevelopmentApplications = page3Applications
        };

        _onlineDaClientMock
            .SetupSequence(c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page1Response))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page2Response))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page3Response));

        _repositoryMock
            .Setup(r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.FetchDaApplications();

        // Assert
        Assert.That(result, Is.EqualTo(5), "Service should return total count of 5 applications");
        _onlineDaClientMock.Verify(
            c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.Exactly(3),
            "API should be called 3 times for 3 pages"
        );
    }

    [Test]
    public async Task FetchDaApplications_SavesAllApplicationsFromAllPages()
    {
        // Arrange
        var savedBatches = new List<IEnumerable<DomainDevelopmentApplication>>();

        var page1Applications = new[]
        {
            CreateOnlineDAApplication("APP-001", "Inner West Council"),
            CreateOnlineDAApplication("APP-002", "Inner West Council")
        };

        var page2Applications = new[]
        {
            CreateOnlineDAApplication("APP-003", "City of Sydney Council")
        };

        var page1Response = new OnlineDAResponse
        {
            PageNumber = 1,
            PageSize = 2,
            TotalPages = 2,
            TotalCount = 3,
            DevelopmentApplications = page1Applications
        };

        var page2Response = new OnlineDAResponse
        {
            PageNumber = 2,
            PageSize = 2,
            TotalPages = 2,
            TotalCount = 3,
            DevelopmentApplications = page2Applications
        };

        _onlineDaClientMock
            .SetupSequence(c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page1Response))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page2Response));

        _repositoryMock
            .Setup(r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()))
            .Callback<IEnumerable<DomainDevelopmentApplication>>(batch => savedBatches.Add(batch.ToList()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.FetchDaApplications();

        // Assert
        Assert.That(result, Is.EqualTo(3));
        
        _repositoryMock.Verify(
            r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()),
            Times.Exactly(2),
            "Repository should be called twice for 2 pages"
        );

        Assert.That(savedBatches, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(savedBatches[0].Count(), Is.EqualTo(2));
            Assert.That(savedBatches[1].Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task FetchDaApplications_ReturnsCorrectCountAfterAllPagesProcessed()
    {
        // Arrange
        var applications = new[]
        {
            CreateOnlineDAApplication("APP-001", "Inner West Council"),
            CreateOnlineDAApplication("APP-002", "Inner West Council"),
            CreateOnlineDAApplication("APP-003", "City of Sydney Council"),
            CreateOnlineDAApplication("APP-004", "City of Sydney Council"),
            CreateOnlineDAApplication("APP-005", "Waverley Council")
        };

        var response = new OnlineDAResponse
        {
            PageNumber = 1,
            PageSize = 5,
            TotalPages = 1,
            TotalCount = 5,
            DevelopmentApplications = applications
        };

        _onlineDaClientMock
            .Setup(c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(response));

        _repositoryMock
            .Setup(r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.FetchDaApplications();

        // Assert
        Assert.That(result, Is.EqualTo(5), "Service should return the total count of saved applications");
    }

    [Test]
    public async Task FetchDaApplications_AccumulatesCountAcrossMultipleBatches()
    {
        // Arrange
        var page1 = new[]
        {
            CreateOnlineDAApplication("APP-001", "Inner West Council"),
            CreateOnlineDAApplication("APP-002", "Inner West Council"),
            CreateOnlineDAApplication("APP-003", "Inner West Council")
        };

        var page2 = new[]
        {
            CreateOnlineDAApplication("APP-004", "City of Sydney Council"),
            CreateOnlineDAApplication("APP-005", "City of Sydney Council")
        };

        var page3 = new[]
        {
            CreateOnlineDAApplication("APP-006", "Waverley Council"),
            CreateOnlineDAApplication("APP-007", "Waverley Council"),
            CreateOnlineDAApplication("APP-008", "Randwick City Council")
        };

        var page1Response = new OnlineDAResponse
        {
            PageNumber = 1,
            PageSize = 3,
            TotalPages = 3,
            TotalCount = 8,
            DevelopmentApplications = page1
        };

        var page2Response = new OnlineDAResponse
        {
            PageNumber = 2,
            PageSize = 3,
            TotalPages = 3,
            TotalCount = 8,
            DevelopmentApplications = page2
        };

        var page3Response = new OnlineDAResponse
        {
            PageNumber = 3,
            PageSize = 3,
            TotalPages = 3,
            TotalCount = 8,
            DevelopmentApplications = page3
        };

        _onlineDaClientMock
            .SetupSequence(c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page1Response))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page2Response))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page3Response));

        _repositoryMock
            .Setup(r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.FetchDaApplications();

        // Assert
        Assert.That(result, Is.EqualTo(8), "Service should accumulate count as 3 + 2 + 3 = 8 applications");
    }

    [Test]
    public async Task FetchDaApplications_StopsIteratingWhenApiReturnsFailure()
    {
        // Arrange
        var page1Applications = new[]
        {
            CreateOnlineDAApplication("APP-001", "Inner West Council"),
            CreateOnlineDAApplication("APP-002", "Inner West Council")
        };

        var page1Response = new OnlineDAResponse
        {
            PageNumber = 1,
            PageSize = 2,
            TotalPages = 3,
            TotalCount = 5,
            DevelopmentApplications = page1Applications
        };

        _onlineDaClientMock
            .SetupSequence(c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page1Response))
            .ReturnsAsync(Result<OnlineDAResponse>.Failure("API Error"));

        _repositoryMock
            .Setup(r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.FetchDaApplications();

        // Assert
        Assert.That(result, Is.EqualTo(2), "Should return count of only successfully saved applications from first page");
        _repositoryMock.Verify(
            r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()),
            Times.Once,
            "Repository should only be called once before API fails"
        );
    }

    [Test]
    public async Task FetchDaApplications_HandlesExceptionFromApi()
    {
        // Arrange
        _onlineDaClientMock
            .Setup(c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new HttpRequestException("Connection failed"));

        // Act
        var result = await _service.FetchDaApplications();

        // Assert
        Assert.That(result, Is.EqualTo(0), "Should return 0 when API throws exception");
        _repositoryMock.Verify(
            r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()),
            Times.Never,
            "Repository should never be called when API throws exception"
        );
    }

    [Test]
    public async Task FetchDaApplications_ContinuesWhenRepositoryThrowsException()
    {
        // Arrange
        var page1Applications = new[]
        {
            CreateOnlineDAApplication("APP-001", "Inner West Council")
        };

        var page2Applications = new[]
        {
            CreateOnlineDAApplication("APP-002", "City of Sydney Council")
        };

        var page1Response = new OnlineDAResponse
        {
            PageNumber = 1,
            PageSize = 1,
            TotalPages = 2,
            TotalCount = 2,
            DevelopmentApplications = page1Applications
        };

        var page2Response = new OnlineDAResponse
        {
            PageNumber = 2,
            PageSize = 1,
            TotalPages = 2,
            TotalCount = 2,
            DevelopmentApplications = page2Applications
        };

        _onlineDaClientMock
            .SetupSequence(c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page1Response))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(page2Response));

        _repositoryMock
            .SetupSequence(r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()))
            .Throws(new InvalidOperationException("Database error"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.FetchDaApplications();

        // Assert
        Assert.That(result, Is.EqualTo(1), "Should return count of only successfully saved applications from second page");
        _repositoryMock.Verify(
            r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()),
            Times.Exactly(2),
            "Repository should be called for both batches"
        );
    }

    [Test]
    public async Task FetchDaApplications_ReturnsZeroWhenNoApplicationsReturned()
    {
        // Arrange
        var response = new OnlineDAResponse
        {
            PageNumber = 1,
            PageSize = 2,
            TotalPages = 1,
            TotalCount = 0,
            DevelopmentApplications = []
        };

        _onlineDaClientMock
            .Setup(c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(response));

        // Act
        var result = await _service.FetchDaApplications();

        // Assert
        Assert.That(result, Is.EqualTo(0), "Should return 0 when no applications are returned");
        _repositoryMock.Verify(
            r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()),
            Times.Never,
            "Repository should not be called when no applications are returned"
        );
    }

    [Test]
    public async Task FetchDaApplications_HandlesNullApplicationsInResponse()
    {
        // Arrange
        var response = new OnlineDAResponse
        {
            PageNumber = 1,
            PageSize = 2,
            TotalPages = 1,
            TotalCount = 0,
            DevelopmentApplications = null
        };

        _onlineDaClientMock
            .Setup(c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(response));

        // Act
        var result = await _service.FetchDaApplications();

        // Assert
        Assert.That(result, Is.EqualTo(0), "Should return 0 when response has null applications");
        _repositoryMock.Verify(
            r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()),
            Times.Never,
            "Repository should not be called when response has null applications"
        );
    }

    [Test]
    public async Task FetchDaApplications_MapsOnlineDAApplicationToDomainEntity()
    {
        // Arrange
        var onlineDaApp = new OnlineDADevelopmentApplication
        {
            PlanningPortalApplicationNumber = "APP-001",
            ApplicationStatus = ApplicationStatus.Determined,
            ApplicationType = ApplicationType.DevelopmentApplication,
            Council = new CouncilInfo { CouncilName = "Inner West Council" },
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DevelopmentType =
            [
                new ProposedDevelopmentType { DevelopmentType = "Residential" }
            ],
            Location =
            [
                new Address { FullAddress = "123 Main Street, Sydney, NSW 2000" }
            ],
            DateLastUpdated = DateTime.UtcNow,
            LodgementDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
            CostOfDevelopment = 1000000,
            CouncilApplicationNumber = "COUNCIL-001"
        };

        var response = new OnlineDAResponse
        {
            PageNumber = 1,
            PageSize = 1,
            TotalPages = 1,
            TotalCount = 1,
            DevelopmentApplications = [onlineDaApp]
        };

        _onlineDaClientMock
            .Setup(c => c.GetDeterminedApplications(It.IsAny<IReadOnlyList<string>>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result<OnlineDAResponse>.Success(response));

        _repositoryMock
            .Setup(r => r.SaveDevelopmentApplications(It.IsAny<IEnumerable<DomainDevelopmentApplication>>()))
            .Callback<IEnumerable<DomainDevelopmentApplication>>(batch => 
            {
                var savedList = batch.ToList();
                Assert.That(savedList, Has.Count.EqualTo(1));
                var saved = savedList[0];
                Assert.Multiple(() =>
                {
                    Assert.That(saved.PlanningPortalApplicationNumber, Is.EqualTo("APP-001"));
                    Assert.That(saved.ApplicationStatus, Is.EqualTo(ApplicationStatus.Determined));
                    Assert.That(saved.ApplicationType, Is.EqualTo(ApplicationType.DevelopmentApplication));
                });
                if (saved.Council != null)
                {
                    Assert.That(saved.Council.CouncilName, Is.EqualTo("Inner West Council"));
                }
            })
            .Returns(Task.CompletedTask);

        // Act
        await _service.FetchDaApplications();
    }

    private static OnlineDADevelopmentApplication CreateOnlineDAApplication(
        string appNumber, string council)
    {
        return new OnlineDADevelopmentApplication
        {
            PlanningPortalApplicationNumber = appNumber,
            ApplicationStatus = ApplicationStatus.Determined,
            ApplicationType = ApplicationType.DevelopmentApplication,
            Council = new CouncilInfo { CouncilName = council },
            DeterminationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DevelopmentType =
            [
                new ProposedDevelopmentType { DevelopmentType = "Residential" }
            ],
            Location =
            [
                new Address { FullAddress = $"Address for {appNumber}" }
            ],
            DateLastUpdated = DateTime.UtcNow,
            LodgementDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
            CostOfDevelopment = 1000000
        };
    }
}
















