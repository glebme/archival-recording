namespace DevelopmentProposalScrapper.Application;

public interface IDevelopmentProposalScrapperService
{
    public Task<int> FetchDaApplications(CancellationToken cancellationToken = default);
}
