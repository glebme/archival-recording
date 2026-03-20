using DevelopmentProposalScrapper.Models.OnlineDA;
using Refit;

namespace DevelopmentProposalScrapper.Services.OnlineDA;

public interface IOnlineDAApi
{
    [Get("/eplanning/data/v0/OnlineDA")]
    Task<IApiResponse<OnlineDAResponse>> GetOnlineDARecordsAsync([Header("PageSize")] int pageSize, [Header("PageNumber")] int pageNumber, [Header("Filters")] Filters filters);
}
