namespace OpenApiGuard.Core.Entities;

public sealed class RuleSet
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;

    public string JsonConfig { get; set; } = "{}";

    public string UpdatedByUserId { get; set; } = default!;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
