namespace DevelopmentProposalScrapper.Models.OnlineDA;

public record OnlineDAResponse
{
    public int PageSize { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public IEnumerable<DevelopmentApplication> DevelopmentApplications { get; }
}

// TODO Check if any fields can be null
public record DevelopmentApplication
{
    public string PlanningPortalApplicationNumber { get; }
    public ApplicationType ApplicationType { get; }
    public ApplicationStatus ApplicationStatus { get; }
    public string CouncilName { get; }
    public IEnumerable<ProposedDevelopmentType> DevelopmentType { get; }
    public DateOnly LodgementDate { get; }
    public DateOnly DeterminationDate { get; }
    public int CostOfDevelopment { get; }
    public IEnumerable<Address> Location { get; }
}

public record Address
{
    public string FullAddress { get; }
    public string X { get; }
    public string Y { get; }
    public string StreetNumber1 { get; }
    public string StreetNumber2 { get; }   
    public string StreetNumber3 { get; }   
    public string StreetName { get; }
    public string StreetType { get; }
    public string Suburb { get; }
    public string Postcode { get; }
    public string State { get; }
}

public record ProposedDevelopmentType
{
    public string DevelopmentType { get; }
}