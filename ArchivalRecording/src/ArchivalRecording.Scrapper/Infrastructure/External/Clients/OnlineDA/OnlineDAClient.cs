using System.Text.Json;
using System.Text.Json.Serialization;
using DevelopmentProposalScrapper.Domain.Entities;
using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;
using Shared;
using Shared.JsonConverters;

namespace DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;


public class OnlineDAClient(IOnlineDAApi daApi) : IOnlineDAClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new DescriptionEnumConverter<ApplicationType>(),
            new DescriptionEnumConverter<ApplicationStatus>(),
        }
    };

    public async Task<Result<OnlineDAResponse>> GetDeterminedApplications(IReadOnlyList<string> councils, DateOnly startDate, int pageSize, int pageNumber)
    {
        var filterRequest = new FilterRequest
        {
            Filters = new Filters
            {
                CouncilName = councils,
                ApplicationType = ApplicationType.DevelopmentApplication,
                ApplicationStatus = [ApplicationStatus.Determined],
                DeterminationDateFrom = startDate
            }
        };

        var filtersRequestString = JsonSerializer.Serialize(filterRequest, JsonSerializerOptions);
        var response = await daApi.GetOnlineDARecordsAsync(pageSize, pageNumber, filtersRequestString);

        return response.IsSuccessful ? Result<OnlineDAResponse>.Success(response.Content!) :
            Result<OnlineDAResponse>.Failure($"API call failed with status code {response.StatusCode} and message: {response.Error.InnerException}");
    }
}
