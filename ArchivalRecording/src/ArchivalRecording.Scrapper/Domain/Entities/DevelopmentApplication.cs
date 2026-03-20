using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;

namespace DevelopmentProposalScrapper.Domain.Entities;

public record DevelopmentApplication
{
    public required string PlanningPortalApplicationNumber { get; init; }
    public DateTime? DateLastUpdated { get; init; }
    public DateOnly DeterminationDate { get; init; }
    public ApplicationStatus? ApplicationStatus { get; init; }
    public ApplicationType? ApplicationType { get; init; }
    public CouncilInfo? Council { get; init; }
    public IEnumerable<ProposedDevelopmentType>? DevelopmentType { get; init; }
    public IEnumerable<Address>? Location { get; init; } 
}
