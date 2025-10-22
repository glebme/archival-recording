
using DevelopmentProposalScrapper.Models.OnlineDA;
using Shared;

namespace DevelopmentProposalScrapper.Services;

public class OnlineDaClient : IOnlineDAClient
{
    private readonly IOnlineDAApi _daApi;

    public OnlineDaClient(IOnlineDAApi daApi)
    {
        _daApi = daApi;
    }
    public Task<Result<OnlineDAResponse>> GetOnlineDARecordsAsync()
    {
        throw new NotImplementedException();
    }
}