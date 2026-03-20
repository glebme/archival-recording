using System.Text.Json.Serialization;

namespace DevelopmentProposalScrapper.Models.OnlineDA;

public enum ApplicationStatus
{
    [JsonPropertyName("Additional Information Requested")]
    AdditionalInformationRequested,
    Determined,
    [JsonPropertyName("Pending Lodgement")]
    PendingLodgement,
    Rejected,
    [JsonPropertyName("Pending Court Appeal")]
    PendingCourtAppeal,
    Cancelled
}