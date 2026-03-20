using System.Text.Json;
using System.Text.Json.Serialization;
using DevelopmentProposalScrapper.Models.OnlineDA;
using Shared;
using Shared.JsonConverters;

namespace DevelopmentProposalScrapper.Services.OnlineDA;

public class OnlineDaClient : IOnlineDAClient
{
    private readonly IOnlineDAApi _daApi;

    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters =
        {
            new DescriptionEnumConverter<ApplicationType>(),
            new DescriptionEnumConverter<ApplicationStatus>(),
        }
    };

    public OnlineDaClient(IOnlineDAApi daApi)
    {
        _daApi = daApi;
    }

    public async Task<Result<OnlineDAResponse>> GetOnlineDARecordsAsync(int pageSize, int pageNumber)
    {
        var filters = new Filters
            {
                CouncilName = ["Council of the City of Sydney"],
                ApplicationType = ApplicationType.DevelopmentApplication,
                ApplicationStatus = [ApplicationStatus.Determined],
                DeterminationDateFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                LodgementDateFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1))
            };

        var filtersString = JsonSerializer.Serialize(filters, _jsonSerializerOptions);
        var response = await _daApi.GetOnlineDARecordsAsync(pageSize, pageNumber, filtersString);

        return response.IsSuccessful ? Result<OnlineDAResponse>.Success(response.Content!) :
            Result<OnlineDAResponse>.Failure($"API call failed with status code {response.StatusCode} and message: {response.Error.InnerException}");
    }
}
