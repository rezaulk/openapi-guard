namespace OpenApiGuard.Core.Diff;

public sealed record DiffResult(
    IReadOnlyList<DiffItem> Items,
    int BreakingCount,
    int WarningCount,
    int InfoCount
)
{
    public string Result => BreakingCount > 0 ? "FAIL" : "PASS";
}
