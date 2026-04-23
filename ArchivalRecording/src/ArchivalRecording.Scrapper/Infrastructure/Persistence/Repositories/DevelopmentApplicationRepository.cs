using DevelopmentProposalScrapper.Domain.Entities;
using DevelopmentProposalScrapper.Domain.Repositories;
using EFCore.BulkExtensions;

namespace DevelopmentProposalScrapper.Infrastructure.Persistence.Repositories;

public class DevelopmentApplicationRepository : IDevelopmentApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public DevelopmentApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<DevelopmentApplication> GetDevelopmentApplication(string planningPortalApplicationNumber)
    {
        throw new NotImplementedException();
    }

    public async Task SaveDevelopmentApplication(DevelopmentApplication developmentApplication)
    {
        _context.DevelopmentApplications.Add(developmentApplication);
        await _context.SaveChangesAsync();
    }

    public async Task SaveDevelopmentApplications(IEnumerable<DevelopmentApplication> developmentApplications)
    {
        await _context.BulkInsertOrUpdateAsync(developmentApplications.ToList(),
            options => options.PropertiesToIncludeOnUpdate =
            [
                "DateLastUpdated", "DeterminationDate", "ApplicationStatus", "ApplicationType", "Council",
                "ProposedDevelopmentTypes", "Addresses"
            ]);
    }
}
