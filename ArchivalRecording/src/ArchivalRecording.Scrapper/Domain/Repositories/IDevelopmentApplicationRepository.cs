using DevelopmentProposalScrapper.Domain.Entities;

namespace DevelopmentProposalScrapper.Domain.Repositories;

public interface IDevelopmentApplicationRepository
{
    public Task SaveDevelopmentApplication(DevelopmentApplication developmentApplication);
    public Task SaveDevelopmentApplications(IEnumerable<DevelopmentApplication> developmentApplications);
}
