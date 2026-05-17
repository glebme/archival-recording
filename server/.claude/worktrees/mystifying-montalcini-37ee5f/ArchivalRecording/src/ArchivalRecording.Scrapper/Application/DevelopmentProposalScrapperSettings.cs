namespace DevelopmentProposalScrapper.Application;

public class DevelopmentProposalScrapperSettings
{
    public bool IsEnabled { get; set; }
    public string CronSchedule { get; set; } = "0 0 * * *"; // Default to run daily at midnight
    public string[] Councils { get; set; }
    public int LookbackDays { get; set; } = 7;
}
