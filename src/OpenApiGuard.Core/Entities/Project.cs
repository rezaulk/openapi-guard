namespace OpenApiGuard.Core.Entities;

public sealed class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerUserId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ProjectRepoLink? RepoLink { get; set; }
    public ICollection<ApiSpecVersion> Specs { get; set; } = [];
    public ICollection<DiffReport> Diffs { get; set; } = [];
    public RuleSet? RuleSet { get; set; }
}
