namespace OpenApiGuard.Core.Entities;

public sealed class DiffReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;

    public Guid FromSpecId { get; set; }
    public Guid ToSpecId { get; set; }

    public int BreakingCount { get; set; }
    public int WarningCount { get; set; }
    public int InfoCount { get; set; }

    public string SummaryJson { get; set; } = default!;

    public string CreatedByUserId { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
