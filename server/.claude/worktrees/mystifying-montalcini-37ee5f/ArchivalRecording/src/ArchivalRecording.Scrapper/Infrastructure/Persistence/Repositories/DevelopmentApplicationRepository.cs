using DevelopmentProposalScrapper.Domain.Entities;
using DevelopmentProposalScrapper.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevelopmentProposalScrapper.Infrastructure.Persistence.Repositories;

public class DevelopmentApplicationRepository : IDevelopmentApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public DevelopmentApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveDevelopmentApplication(DevelopmentApplication developmentApplication)
    {
        _context.DevelopmentApplications.Add(developmentApplication);
        await _context.SaveChangesAsync();
    }

    public async Task SaveDevelopmentApplications(IEnumerable<DevelopmentApplication> developmentApplications)
    {
        var appsToSave = developmentApplications.ToList();
        var appNumbers = appsToSave.Select(a => a.PlanningPortalApplicationNumber).ToList();

        var existingNumbers = await _context.DevelopmentApplications
            .Where(x => appNumbers.Contains(x.PlanningPortalApplicationNumber))
            .Select(x => x.PlanningPortalApplicationNumber)
            .ToListAsync();

        var existingSet = existingNumbers.ToHashSet();

        foreach (var app in appsToSave.Where(a => existingSet.Contains(a.PlanningPortalApplicationNumber)))
        {
            await _context.DevelopmentApplications
                .Where(x => x.PlanningPortalApplicationNumber == app.PlanningPortalApplicationNumber)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.DateLastUpdated, app.DateLastUpdated)
                    .SetProperty(x => x.DeterminationDate, app.DeterminationDate)
                    .SetProperty(x => x.ApplicationStatus, app.ApplicationStatus)
                    .SetProperty(x => x.ApplicationType, app.ApplicationType));
        }

        var toInsert = appsToSave.Where(a => !existingSet.Contains(a.PlanningPortalApplicationNumber)).ToList();
        if (toInsert.Count > 0)
        {
            _context.DevelopmentApplications.AddRange(toInsert);
            await _context.SaveChangesAsync();
        }
    }
}
