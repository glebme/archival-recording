using DevelopmentProposalScrapper.Models.OnlineDA;
using Shared;

namespace DevelopmentProposalScrapper.Services;

public interface IOnlineDAClient
{
    Task<Result<OnlineDAResponse>> GetOnlineDARecordsAsync();
}