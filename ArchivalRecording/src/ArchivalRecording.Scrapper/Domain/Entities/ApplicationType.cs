using System.ComponentModel;

namespace DevelopmentProposalScrapper.Infrastructure.External.Models.OnlineDA;

public enum ApplicationType
{
    [Description("Development Application")]
    DevelopmentApplication,
    [Description("Modification Application")]
    ModificationApplication,
    [Description("Review Of Determination")]
    ReviewOfDetermination
}