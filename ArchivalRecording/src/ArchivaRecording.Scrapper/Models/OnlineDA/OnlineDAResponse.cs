namespace DevelopmentProposalScrapper.Models.OnlineDA;

public record OnlineDAResponse
{
    public int PageSize { get; init; }
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }
    public IEnumerable<DevelopmentApplication> DevelopmentApplications { get; init; }
}

// TODO Check if any fields can be null
public record DevelopmentApplication
{
    public string PlanningPortalApplicationNumber { get; init; }
    public ApplicationType ApplicationType { get; init; }
    public ApplicationStatus ApplicationStatus { get; init; }
    public string CouncilName { get; init; }
    public IEnumerable<ProposedDevelopmentType> DevelopmentType { get; init; }
    public DateOnly LodgementDate { get; init; }
    public DateOnly DeterminationDate { get; init; }
    public int CostOfDevelopment { get; init; }
    public IEnumerable<Address> Location { get; init; }
}

public record Address
{
    public string FullAddress { get; init; }
    public string X { get; init; }
    public string Y { get; init; }
    public string StreetNumber1 { get; init; }
    public string StreetNumber2 { get; init; }
    public string StreetNumber3 { get; init; }
    public string StreetName { get; init; }
    public string StreetType { get; init; }
    public string Suburb { get; init; }
    public string Postcode { get; init; }
    public string State { get; init; }
}

public record ProposedDevelopmentType
{
    public string DevelopmentType { get; init; }
}
