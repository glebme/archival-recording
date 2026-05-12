using DevelopmentProposalScrapper.Domain.Repositories;
using DevelopmentProposalScrapper.Infrastructure.External.Clients.OnlineDA;
using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;
using Microsoft.Extensions.Options;
using Polly;
using Shared;
using DevelopmentApplication = DevelopmentProposalScrapper.Domain.Entities.DevelopmentApplication;

namespace DevelopmentProposalScrapper.Application;

public class DevelopmentProposalScrapperService(
    IOnlineDAClient onlineDaClient,
    IDevelopmentApplicationRepository developmentApplicationRepository,
    IOptions<DevelopmentProposalScrapperSettings> options,
    ResiliencePipeline retryPipeline) : IDevelopmentProposalScrapperService
{
    private readonly DevelopmentProposalScrapperSettings _settings = options.Value ?? throw new ArgumentNullException(nameof(options), "DevelopmentProposalScrapperSettings cannot be null.");

    public async Task<int> FetchDaApplications(ScrapeCycleEvent cycleEvent, CancellationToken cancellationToken = default)
    {
        var savedRecords = 0;
        var batchNum = 0;

        await foreach (var records in GetAllDeterminedApplicationsAfterCertainDate(
                           councils: _settings.Councils,
                           startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-_settings.LookbackDays)),
                           pageSize: 100,
                           cycleEvent: cycleEvent).WithCancellation(cancellationToken))
        {
            batchNum++;

            if (!records.Any())
                continue;

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
                ProposedDevelopmentTypes = da.DevelopmentType,
                Addresses = da.Location
            }).ToList();

            try
            {
                await retryPipeline.ExecuteAsync(
                    async _ => await developmentApplicationRepository.SaveDevelopmentApplications(developmentApplications),
                    cancellationToken);

                var successfullySaved = developmentApplications.Count;
                cycleEvent.TotalRecordsSaved += successfullySaved;
                savedRecords += successfullySaved;
            }
            catch (Exception ex)
            {
                cycleEvent.Errors.Add(new PageError(batchNum, "DatabaseSaveFailure", ex.Message));
            }
        }

        return savedRecords;
    }

    private async IAsyncEnumerable<IEnumerable<Infrastructure.External.Models.OnlineDA.DevelopmentApplication>> GetAllDeterminedApplicationsAfterCertainDate(
        IReadOnlyList<string> councils, DateOnly startDate, int pageSize, ScrapeCycleEvent cycleEvent)
    {
        int? totalPages = null;
        var currentPage = 1;

        do
        {
            if (currentPage > totalPages) yield break;

            cycleEvent.PagesAttempted++;

            Result<OnlineDAResponse>? result;
            try
            {
                result = await onlineDaClient.GetDeterminedApplications(councils, startDate, pageSize, currentPage);
            }
            catch (Exception ex)
            {
                cycleEvent.PagesFailed++;
                cycleEvent.Errors.Add(new PageError(currentPage, "FetchException", ex.Message));

                currentPage++;
                if (!totalPages.HasValue) yield break;
                continue;
            }

            if (!result.IsSuccess)
            {
                cycleEvent.PagesFailed++;
                cycleEvent.Errors.Add(new PageError(currentPage, "FetchFailure", result.ErrorMessage ?? "Unknown error"));

                currentPage++;
                if (!totalPages.HasValue) yield break;
                continue;
            }

            if (result.Model is null)
                yield break;

            totalPages ??= result.Model.TotalPages;
            currentPage++;

            if (result.Model.DevelopmentApplications is null) yield break;

            cycleEvent.PagesSucceeded++;
            cycleEvent.TotalRecordsFetched += result.Model.DevelopmentApplications.Count();

            yield return result.Model.DevelopmentApplications;

        } while (currentPage <= totalPages);
    }
}
