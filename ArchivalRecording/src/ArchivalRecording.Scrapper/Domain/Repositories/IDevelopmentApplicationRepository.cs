using DevelopmentProposalScrapper.Domain.Entities;

namespace DevelopmentProposalScrapper.Domain.Repositories;

public interface IDevelopmentApplicationRepository
{
    public Task<DevelopmentApplication> GetDevelopmentApplication(string planningPortalApplicationNumber);
    public Task SaveDevelopmentApplication(DevelopmentApplication developmentApplication);
}
