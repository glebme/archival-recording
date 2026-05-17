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
        
        // Get existing application numbers to determine insert vs update
        var existingNumbers = await _context.DevelopmentApplications
            .Where(x => appsToSave.Select(y => y.PlanningPortalApplicationNumber).Contains(x.PlanningPortalApplicationNumber))
            .Select(x => x.PlanningPortalApplicationNumber)
            .ToListAsync();

        foreach (var app in appsToSave)
        {
            if (existingNumbers.Contains(app.PlanningPortalApplicationNumber))
            {
                // Update existing record
                var existing = await _context.DevelopmentApplications
                    .FirstAsync(x => x.PlanningPortalApplicationNumber == app.PlanningPortalApplicationNumber);
                
                existing.DateLastUpdated = app.DateLastUpdated;
                existing.DeterminationDate = app.DeterminationDate;
                existing.ApplicationStatus = app.ApplicationStatus;
                existing.ApplicationType = app.ApplicationType;
                
                _context.DevelopmentApplications.Update(existing);
            }
            else
            {
                // Insert new record
                _context.DevelopmentApplications.Add(app);
            }
        }

        await _context.SaveChangesAsync();
    }
}
