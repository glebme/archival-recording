using DevelopmentProposalScrapper.Domain.Repositories;
using DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;
using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;
using Shared;
using DevelopmentApplication = DevelopmentProposalScrapper.Domain.Entities.DevelopmentApplication;

namespace DevelopmentProposalScrapper.Application;

public class DevelopmentProposalScrapperService(ILogger<IDevelopmentProposalScrapperService> logger, IOnlineDAClient onlineDaClient, IDevelopmentApplicationRepository developmentApplicationRepository) : IDevelopmentProposalScrapperService
{
    public async Task<int> FetchDaApplications()
    {
        Result<OnlineDAResponse>? result;

        try
        {
            //TODO: fetch using IEnumerable pattern to grab all documents as needed and save them as needed
            result = await onlineDaClient.GetDeterminedApplications(["Council of the City of Sydney"], DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)), 5, 14);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch records from OnlineDA API due to exception");
            
            return 0;
        }

        if (result is { IsSuccess: true, Model: not null })
        {
            var records = result.Model!;
            logger.LogInformation("Fetched {count} records.", records.TotalCount);

            var developmentApplications = records.DevelopmentApplications?
                .Select(da => new DevelopmentApplication()
                {
                    PlanningPortalApplicationNumber = da.PlanningPortalApplicationNumber,
                    DateLastUpdated = da.DateLastUpdated.HasValue ? TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(da.DateLastUpdated.Value, DateTimeKind.Unspecified), TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney")) : null,
                    DeterminationDate = da.DeterminationDate,
                    ApplicationStatus = da.ApplicationStatus,
                    ApplicationType = da.ApplicationType,
                    Council = da.Council,
                    ProposedDevelopmentTypes = da.DevelopmentType,
                    Addresses = da.Location
                })
                .ToList() ?? [];

            if (developmentApplications.Count == 0) return developmentApplications.Count;
            
            try
            {
                // TODO fix council column field naming in migration so that it matches the PropertiesToUpdate 
                await developmentApplicationRepository.SaveDevelopmentApplications(developmentApplications);
                logger.LogInformation("Saved {count} records to database.", developmentApplications.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save records to database.");
            }

            return developmentApplications.Count;
        }

        logger.LogError("Failed to fetch records: {error}", result?.ErrorMessage);

        return 0;
    }
}
