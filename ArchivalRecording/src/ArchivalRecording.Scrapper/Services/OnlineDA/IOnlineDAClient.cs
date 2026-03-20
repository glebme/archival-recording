using DevelopmentProposalScrapper.Models.OnlineDA;
using Shared;

namespace DevelopmentProposalScrapper.Services.OnlineDA;

public interface IOnlineDAClient
{
    Task<Result<OnlineDAResponse>> GetOnlineDARecordsAsync(int pageSize, int pageNumber);
}
