namespace ArchivalRecording.Api.Domain.Entities;

public class AllowedUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
}
