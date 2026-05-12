namespace DevelopmentProposalScrapper.Application;

public sealed class ScrapeCycleEvent
{
    public string RunId { get; init; } = Guid.NewGuid().ToString("N")[..8];
    private DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? FinishedAt { get; set; }
    public long DurationMs => FinishedAt.HasValue ? (long)(FinishedAt.Value - StartedAt).TotalMilliseconds : 0;

    public string[] Councils { get; init; } = [];
    public DateOnly LookbackFrom { get; init; }
    public int PageSize { get; init; }

    public int PagesAttempted { get; set; }
    public int PagesSucceeded { get; set; }
    public int PagesFailed { get; set; }
    public int TotalRecordsFetched { get; set; }
    public int TotalRecordsSaved { get; set; }

    public List<PageError> Errors { get; } = [];
    public string Outcome { get; set; } = "success";
}

public sealed record PageError(int Page, string Kind, string Message);
