namespace DevelopmentProposalScrapper.Models.OnlineDA;

public record OnlineDARequest
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public required Filters Filters { get; set; }
}

public record Filters
{
    public IEnumerable<string> CouncilName { get; set; }
    public ApplicationType ApplicationType { get; set; }
    public IEnumerable<ApplicationStatus> ApplicationStatus { get; set; } = [];
    public DateOnly DeterminationDateFrom { get; set; }
    public DateOnly? DeterminationDateTo { get; set; }
    public DateOnly LodgementDateFrom { get; set; }
    public DateOnly? LodgementDateTo { get; set; }
}
