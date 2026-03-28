namespace OpenApiGuard.Contracts.Diffs;

public sealed record CreateDiffRequest(Guid FromSpecId, Guid ToSpecId);

public sealed record DiffReportDto(
    Guid Id,
    Guid ProjectId,
    Guid FromSpecId,
    Guid ToSpecId,
    string Result,
    int BreakingCount,
    int WarningCount,
    int InfoCount,
    IReadOnlyList<DiffItemDto> Items,
    DateTimeOffset CreatedAt
);

public sealed record DiffItemDto(
    string Severity,
    string Kind,
    string Target,
    string Message
);

public sealed record DiffReportSummaryDto(
    Guid Id,
    Guid ProjectId,
    Guid FromSpecId,
    Guid ToSpecId,
    string? FromLabel,
    string? ToLabel,
    string Result,
    int BreakingCount,
    int WarningCount,
    int InfoCount,
    DateTimeOffset CreatedAt
);
