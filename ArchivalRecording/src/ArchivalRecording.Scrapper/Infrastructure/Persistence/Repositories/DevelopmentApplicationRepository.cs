using DevelopmentProposalScrapper.Domain.Entities;
using DevelopmentProposalScrapper.Domain.Repositories;

namespace DevelopmentProposalScrapper.Infrastructure.Persistence.Repositories;

public class DevelopmentApplicationRepository : IDevelopmentApplicationRepository
{
    public Task<DevelopmentApplication> GetDevelopmentApplication(string planningPortalApplicationNumber)
    {
        throw new NotImplementedException();
    }

    public Task SaveDevelopmentApplication(DevelopmentApplication developmentApplication)
    {
        throw new NotImplementedException();
    }
}
