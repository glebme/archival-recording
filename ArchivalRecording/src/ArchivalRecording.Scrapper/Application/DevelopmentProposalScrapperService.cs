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
        // TODO fix unit tests, and run
        var savedRecords = 0;
        await foreach (var records in GetAllDeterminedApplicationsAfterCertainDate(
                     councils: ["Inner West Council", "City of Sydney Council", "Waverley Council", "Randwick City Council"],
                     startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
                     pageSize: 100))
        {
            if (records.Count() == 0)
            {
                logger.LogInformation("No records fetched from OnlineDA API");
                continue;
            }
            
            var developmentApplications = records.Select(da => new DevelopmentApplication
            {
                PlanningPortalApplicationNumber = da.PlanningPortalApplicationNumber,
                DateLastUpdated = da.DateLastUpdated.HasValue ? TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(da.DateLastUpdated.Value, DateTimeKind.Unspecified), TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney")) : null,
                DeterminationDate = da.DeterminationDate,
                ApplicationStatus = da.ApplicationStatus,
                ApplicationType = da.ApplicationType,
                Council = da.Council,
                ProposedDevelopmentTypes = da.DevelopmentType,
                Addresses = da.Location
            }).ToList();

            try
            {
                // TODO fix council column field naming in migration so that it matches the PropertiesToUpdate
                await developmentApplicationRepository.SaveDevelopmentApplications(developmentApplications);

                var successfullySaved = developmentApplications.Count;
                logger.LogInformation("Saved {count} records to database", successfullySaved);
                savedRecords +=  successfullySaved;
            }
            catch (Exception ex)
            {
                // TODO retry mechanism?
                logger.LogError(ex, "Failed to save records to database");
            }
        }

        return savedRecords;
    }
    
    
    // TODO think about progressing if one page fails - currently it will stop the entire process, but maybe we want to continue with the next page?
    private async IAsyncEnumerable<IEnumerable<DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA.DevelopmentApplication>> GetAllDeterminedApplicationsAfterCertainDate(
        IReadOnlyList<string> councils, DateOnly startDate, int pageSize)
    {
        int? totalPages = null;
        var currentPage = 1;

        do
        {
            if (currentPage > totalPages) yield break;
            
            Result<OnlineDAResponse>? result;
            try
            {
                 result =
                    await onlineDaClient.GetDeterminedApplications(councils, startDate, pageSize, currentPage);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch records from OnlineDA API due to exception");
            
                yield break; 
            }

            if (!result.IsSuccess)
            {
                logger.LogError("Failed to fetch records from OnlineDA API. Error: {ErrorMessage}", result.ErrorMessage);
                
                yield break;
            } 
            
            if (result.Model is null)
            {
                logger.LogWarning("No records to fetch");
                
                yield break;
            }
            
            totalPages ??= result.Model.TotalPages;
            currentPage++;

            if (result.Model.DevelopmentApplications is null) yield break;
            
            yield return result.Model.DevelopmentApplications;
        } while (currentPage < totalPages);
    }
}
