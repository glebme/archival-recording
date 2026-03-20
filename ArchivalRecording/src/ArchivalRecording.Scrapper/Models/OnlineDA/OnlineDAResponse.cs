using System.Text.Json.Serialization;

namespace DevelopmentProposalScrapper.Models.OnlineDA;

public record OnlineDAResponse
{
    public int PageSize { get; init; }
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }
    [JsonPropertyName("Application")]
    public required IEnumerable<DevelopmentApplication> DevelopmentApplications { get; init; }
}

public record DevelopmentApplication
{
    public required string PlanningPortalApplicationNumber { get; init; }
    public DateTime? DateLastUpdated { get; init; }
    public DateOnly? SubmissionDate { get; init; }
    public DateOnly LodgementDate { get; init; }
    public DateOnly DeterminationDate { get; init; }
    public DateTime? AssessmentExhibitionEndDate { get; init; }
    public DateTime? AssessmentExhibitionStartDate { get; init; }
    public decimal CostOfDevelopment { get; init; }
    public string? CouncilApplicationNumber { get; init; }
    public string? ApplicationStatus { get; init; }
    public string? ApplicationType { get; init; }
    public string? AccompaniedByVPAFlag { get; init; }
    public string? DevelopmentSubjectToSICFlag { get; init; }
    public string? EPIVariationProposedFlag { get; init; }
    public string? DeterminationAuthority { get; init; }
    public string? VariationToDevelopmentStandardsApprovedFlag { get; init; }
    public int? NumberOfNewDwellings { get; init; }
    public int? NumberOfStoreys { get; init; }
    public CouncilInfo? Council { get; init; }
    public IEnumerable<ProposedDevelopmentType>? DevelopmentType { get; init; }
    public IEnumerable<Address>? Location { get; init; }
}

public record CouncilInfo
{
    public required string CouncilName { get; init; }
}

public record Address
{
    public string? FullAddress { get; init; }
    public string? X { get; init; }
    public string? Y { get; init; }
    public string? StreetNumber1 { get; init; }
    public string? StreetNumber2 { get; init; }
    public string? StreetNumber3 { get; init; }
    public string? StreetName { get; init; }
    public string? StreetType { get; init; }
    public string? Suburb { get; init; }
    public string? Postcode { get; init; }
    public string? State { get; init; }
    public IEnumerable<Lot>? Lot { get; init; }
}

public record Lot
{
    [JsonPropertyName("Lot")]
    public string? LotNo { get; init; }
    public string? PlanLabel { get; init; }
}

public record ProposedDevelopmentType
{
    public string? DevelopmentType { get; init; }
}
