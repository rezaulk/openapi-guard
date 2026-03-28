namespace OpenApiGuard.Contracts.Specs;

public sealed record ApiSpecVersionDto(
    Guid Id,
    Guid ProjectId,
    string Label,
    string Source,
    string Format,
    string UploadedByUserId,
    DateTimeOffset UploadedAt
);

public sealed record UploadSpecRequest(
    string Label,
    string RawSpecText,
    string Format  // "json" | "yaml"
);
