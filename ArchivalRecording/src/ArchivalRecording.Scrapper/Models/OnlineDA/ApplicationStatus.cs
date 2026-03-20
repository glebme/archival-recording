using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DevelopmentProposalScrapper.Models.OnlineDA;

public enum ApplicationStatus
{
    [Description("Additional Information Requested")]
    AdditionalInformationRequested,
    [Description("Determined")]
    Determined,
    [Description("Pending Lodgement")]
    PendingLodgement,
    [Description("Rejected")]
    Rejected,
    [Description("Pending Court Appeal")]
    PendingCourtAppeal,
    [Description("Cancelled")]
    Cancelled
}