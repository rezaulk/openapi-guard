namespace OpenApiGuard.Core.Entities;

public sealed class ProjectRepoLink
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;

    public string Provider { get; set; } = "github";
    public string RepoUrl { get; set; } = default!;
    public string DefaultBranch { get; set; } = "main";
    public string? OpenApiSpecHint { get; set; }

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
