namespace OpenApiGuard.Contracts.Projects;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset UpdatedAt,
    RepoLinkDto? RepoLink
);

public sealed record RepoLinkDto(
    string Provider,
    string RepoUrl,
    string DefaultBranch,
    string? OpenApiSpecHint,
    DateTimeOffset UpdatedAt
);

public sealed record CreateProjectRequest(
    string Name,
    string? Description
);

public sealed record UpdateProjectRequest(
    string? Name,
    string? Description
);

public sealed record SaveRepoLinkRequest(
    string Provider,
    string RepoUrl,
    string DefaultBranch,
    string? OpenApiSpecHint
);
