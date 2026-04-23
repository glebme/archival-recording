using System.ComponentModel;

namespace DevelopmentProposalScrapper.Domain.Entities;

public enum ApplicationType
{
    [Description("Development Application")]
    DevelopmentApplication,
    [Description("Modification Application")]
    ModificationApplication,
    [Description("Review Of Determination")]
    ReviewOfDetermination
}