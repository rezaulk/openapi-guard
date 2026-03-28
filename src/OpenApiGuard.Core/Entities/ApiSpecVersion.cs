namespace OpenApiGuard.Core.Entities;

public sealed class ApiSpecVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;

    public string Label { get; set; } = default!;
    public string Source { get; set; } = "manual";
    public string RawSpecText { get; set; } = default!;
    public string Format { get; set; } = "json";
    public string OpenApiVersion { get; set; } = "3";

    public string UploadedByUserId { get; set; } = default!;
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
}
