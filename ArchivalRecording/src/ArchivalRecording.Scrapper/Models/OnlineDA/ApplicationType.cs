using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DevelopmentProposalScrapper.Models.OnlineDA;

public enum ApplicationType
{
    [Description("Development Application")]
    DevelopmentApplication,
    [Description("Modification Application")]
    ModificationApplication,
    [Description("Review Of Determination")]
    ReviewOfDetermination
}