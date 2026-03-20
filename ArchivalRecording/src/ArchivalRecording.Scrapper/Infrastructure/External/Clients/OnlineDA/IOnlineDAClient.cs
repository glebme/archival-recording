using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;
using Shared;

namespace DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;

public interface IOnlineDAClient
{
    Task<Result<OnlineDAResponse>> GetOnlineDARecordsAsync(int pageSize, int pageNumber);
}
