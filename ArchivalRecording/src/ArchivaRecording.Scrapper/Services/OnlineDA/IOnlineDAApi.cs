using DevelopmentProposalScrapper.Models.OnlineDA;
using Refit;

namespace DevelopmentProposalScrapper.Services;

public interface IOnlineDAApi
{
   [Get("/eplanning/data/v0/OnlineDA")]
   Task<OnlineDAResponse> GetOnlineDARecordsAsync([Query] OnlineDARequest request);
}