namespace DevelopmentProposalScrapper;

public class DevelopmentProposalScrapperSettings
{
    public bool IsEnabled { get; set; }
    public string CronSchedule { get; set; } = "0 0 * * *"; // Default to run daily at midnight
}