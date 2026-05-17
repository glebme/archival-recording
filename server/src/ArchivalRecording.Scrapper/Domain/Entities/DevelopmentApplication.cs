using DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;

namespace DevelopmentProposalScrapper.Domain.Entities;

public record DevelopmentApplication
{
    public required string PlanningPortalApplicationNumber { get; init; }
    public DateTime? DateLastUpdated { get; set; }
    public DateOnly DeterminationDate { get; set; }
    public ApplicationStatus? ApplicationStatus { get; set; }
    public ApplicationType? ApplicationType { get; set; }
    public CouncilInfo? Council { get; init; }
    public IEnumerable<ProposedDevelopmentType>? ProposedDevelopmentTypes { get; init; }
    public IEnumerable<Address>? Addresses { get; init; }
}
