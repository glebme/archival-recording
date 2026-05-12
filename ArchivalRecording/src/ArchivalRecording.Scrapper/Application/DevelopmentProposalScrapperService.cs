using DevelopmentProposalScrapper.Domain.Repositories;
using DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;
using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;
using Microsoft.Extensions.Options;
using Polly;
using Shared;
using DevelopmentApplication = DevelopmentProposalScrapper.Domain.Entities.DevelopmentApplication;

namespace DevelopmentProposalScrapper.Application;

public class DevelopmentProposalScrapperService(
    ILogger<IDevelopmentProposalScrapperService> logger,
    IOnlineDAClient onlineDaClient,
    IDevelopmentApplicationRepository developmentApplicationRepository,
    IOptions<DevelopmentProposalScrapperSettings> options,
    ResiliencePipeline retryPipeline) : IDevelopmentProposalScrapperService
{
    private readonly DevelopmentProposalScrapperSettings _settings = options.Value ?? throw new ArgumentNullException(nameof(options), "DevelopmentProposalScrapperSettings cannot be null.");

    public async Task<int> FetchDaApplications(CancellationToken cancellationToken = default)
    {
        var savedRecords = 0;
        await foreach (var records in GetAllDeterminedApplicationsAfterCertainDate(
                           councils: _settings.Councils,
                           startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-_settings.LookbackDays)),
                           pageSize: 100).WithCancellation(cancellationToken))
        {
            if (!records.Any())
            {
                logger.LogInformation("No records fetched from OnlineDA API");
                continue;
            }

            var developmentApplications = records.Select(da => new DevelopmentApplication
            {
                PlanningPortalApplicationNumber = da.PlanningPortalApplicationNumber,
                DateLastUpdated = da.DateLastUpdated.HasValue
                    ? TimeZoneInfo.ConvertTimeToUtc(
                        DateTime.SpecifyKind(da.DateLastUpdated.Value, DateTimeKind.Unspecified),
                        TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney"))
                    : null,
                DeterminationDate = da.DeterminationDate,
                ApplicationStatus = da.ApplicationStatus,
                ApplicationType = da.ApplicationType,
                Council = da.Council,
                CouncilApplicationNumber = da.CouncilApplicationNumber,
                ProposedDevelopmentTypes = da.DevelopmentType,
                Addresses = da.Location
            }).ToList();

            try
            {
                await retryPipeline.ExecuteAsync(
                    async _ => await developmentApplicationRepository.SaveDevelopmentApplications(developmentApplications),
                    cancellationToken);

                var successfullySaved = developmentApplications.Count;
                logger.LogInformation("Saved {count} records to database", successfullySaved);
                savedRecords += successfullySaved;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save records to database after retries");
            }
        }

        return savedRecords;
    }

    private async IAsyncEnumerable<IEnumerable<Infrastructure.External.Models.OnlineDA.DevelopmentApplication>> GetAllDeterminedApplicationsAfterCertainDate(
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
                result = await onlineDaClient.GetDeterminedApplications(councils, startDate, pageSize, currentPage);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch page {Page} from OnlineDA API. Skipping to next page", currentPage);
                
                currentPage++;
                if (!totalPages.HasValue) yield break;
                continue;
            }

            if (!result.IsSuccess)
            {
                logger.LogError("Failed to fetch page {Page} from OnlineDA API. Error: {ErrorMessage}", currentPage, result.ErrorMessage);
                
                currentPage++;
                if (!totalPages.HasValue) yield break;
                continue;
            }

            if (result.Model is null)
            {
                logger.LogWarning("No records returned for page {Page}", currentPage);
                yield break;
            }

            totalPages ??= result.Model.TotalPages;
            currentPage++;

            if (result.Model.DevelopmentApplications is null) yield break;

            yield return result.Model.DevelopmentApplications;

        } while (currentPage <= totalPages);
    }
}
