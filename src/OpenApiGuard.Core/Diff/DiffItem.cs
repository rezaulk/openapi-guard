namespace OpenApiGuard.Core.Diff;

public sealed record DiffItem(
    string Severity,   // "breaking" | "warning" | "info"
    string Kind,       // "endpoint_removed" | "schema_field_removed" | etc.
    string Target,     // e.g. "DELETE /api/users/{id}"
    string Message
);
