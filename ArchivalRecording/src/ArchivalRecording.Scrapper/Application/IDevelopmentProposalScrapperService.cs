namespace DevelopmentProposalScrapper.Application;

public interface IDevelopmentProposalScrapperService
{
    Task<int> FetchDaApplications(ScrapeCycleEvent cycleEvent, CancellationToken cancellationToken = default);
}
