using DevelopmentProposalScrapper.Domain.Entities;
using DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;
using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;
using Moq;
using Refit;
using OnlineDAResponseModel = DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA.DevelopmentApplication;

namespace DevelopmentProposalScrapperTests;

[TestFixture]
public class OnlineDaClientTests
{
    private Mock<IOnlineDAApi> _apiMock;
    private OnlineDaClient _client;

    [SetUp]
    public void Setup()
    {
        _apiMock = new Mock<IOnlineDAApi>();
        _client = new OnlineDaClient(_apiMock.Object);
    }

    [Test]
    public async Task GetOnlineDARecordsAsync_ReturnsSuccessResult_WhenApiCallIsSuccessful()
    {
        // Arrange
        var mockResponse = new OnlineDAResponse
        {
            PageSize = 1,
            PageNumber = 1,
            TotalPages = 1,
            TotalCount = 1,
            DevelopmentApplications =
            [
                new OnlineDAResponseModel
                {
                    PlanningPortalApplicationNumber = "APP-001",
                    DateLastUpdated = DateTime.Now.AddDays(-1),
                    DeterminationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
                    ApplicationStatus = ApplicationStatus.Determined,
                    ApplicationType = ApplicationType.DevelopmentApplication,
                    Council = new CouncilInfo { CouncilName = "Council of the City of Sydney" },
                    DevelopmentType = new[] { new ProposedDevelopmentType { DevelopmentType = "Residential" } },
                    Location = new[]
                    {
                        new Address
                        {
                            FullAddress = "123 Main St, Sydney NSW 2000",
                            Suburb = "Sydney",
                            Postcode = "2000",
                            State = "NSW"
                        }
                    }
                }
            ]
        };

        var mockApiResponse = new Mock<IApiResponse<OnlineDAResponse>>();
        mockApiResponse.Setup(x => x.IsSuccessful).Returns(true);
        mockApiResponse.Setup(x => x.Content).Returns(mockResponse);
        mockApiResponse.Setup(x => x.StatusCode).Returns(System.Net.HttpStatusCode.OK);

        _apiMock.Setup(x => x.GetOnlineDARecordsAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
            .ReturnsAsync(mockApiResponse.Object);

        // Act
        var result = await _client.GetOnlineDARecordsAsync(1, 1);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Model, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.Model.TotalCount, Is.EqualTo(1));
            Assert.That(result.Model.DevelopmentApplications?.Count(), Is.EqualTo(1));
            Assert.That(result.Model.DevelopmentApplications?.First().PlanningPortalApplicationNumber, Is.EqualTo("APP-001"));
        });
    }

    [Test]
    public async Task GetOnlineDARecordsAsync_ReturnsFailureResult_WhenApiCallFails()
    {
        // Arrange
        var mockApiResponse = new Mock<IApiResponse<OnlineDAResponse>>();
        mockApiResponse.Setup(x => x.IsSuccessful).Returns(false);
        mockApiResponse.Setup(x => x.StatusCode).Returns(System.Net.HttpStatusCode.InternalServerError);
        var mockException = ApiException.Create("Internal Server Error", new HttpRequestMessage(), HttpMethod.Get, new HttpResponseMessage(), new RefitSettings()).Result;
        mockApiResponse.Setup(x => x.Error).Returns(mockException);

        _apiMock.Setup(x => x.GetOnlineDARecordsAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
            .ReturnsAsync(mockApiResponse.Object);

        // Act
        var result = await _client.GetOnlineDARecordsAsync(1, 1);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorMessage, Is.Not.Null);
        Assert.That(result.ErrorMessage, Does.Contain("API call failed with status code InternalServerError and message:"));
    }

    [Test]
    public async Task GetOnlineDARecordsAsync_ReturnsEmptyDevelopmentApplicationsList_WhenNoRecordsReturned()
    {
        // Arrange
        var mockResponse = new OnlineDAResponse
        {
            PageSize = 1,
            PageNumber = 1,
            TotalPages = 0,
            TotalCount = 0,
            DevelopmentApplications = new List<OnlineDAResponseModel>()
        };

        var mockApiResponse = new Mock<IApiResponse<OnlineDAResponse>>();
        mockApiResponse.Setup(x => x.IsSuccessful).Returns(true);
        mockApiResponse.Setup(x => x.Content).Returns(mockResponse);
        mockApiResponse.Setup(x => x.StatusCode).Returns(System.Net.HttpStatusCode.OK);

        _apiMock.Setup(x => x.GetOnlineDARecordsAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
            .ReturnsAsync(mockApiResponse.Object);

        // Act
        var result = await _client.GetOnlineDARecordsAsync(1, 1);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Model?.DevelopmentApplications?.Count(), Is.EqualTo(0));
            Assert.That(result.Model?.TotalCount, Is.EqualTo(0));
        });
    }
}









