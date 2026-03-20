using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;
using Refit;

namespace DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;

public interface IOnlineDAApi
{
    [Get("/eplanning/data/v0/OnlineDA")]
    Task<IApiResponse<OnlineDAResponse>> GetOnlineDARecordsAsync([Header("PageSize")] int pageSize, [Header("PageNumber")] int pageNumber, [Header("Filters")] string filters);
}
