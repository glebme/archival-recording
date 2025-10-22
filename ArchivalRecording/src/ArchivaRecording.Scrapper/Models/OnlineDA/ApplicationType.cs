using System.Text.Json.Serialization;

namespace DevelopmentProposalScrapper.Models.OnlineDA;

public enum ApplicationType
{
    [JsonPropertyName("Development Application")]
    DevelopmentApplication,
    [JsonPropertyName("Modification Application")]
    ModificationApplication,
    [JsonPropertyName("Review Of Determination")]
    ReviewOfDetermination
}