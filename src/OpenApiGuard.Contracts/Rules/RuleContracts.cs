namespace OpenApiGuard.Contracts.Rules;

public sealed record RuleSetDto(
    Guid ProjectId,
    RuleConfig Config,
    DateTimeOffset UpdatedAt
);

public sealed record RuleConfig
{
    public bool BlockRemovedEndpoints { get; init; } = true;
    public bool BlockRemovedResponseFields { get; init; } = true;
    public bool BlockTypeChanges { get; init; } = true;
    public bool BlockOptionalToRequired { get; init; } = true;
    public bool RequireAuthScheme { get; init; } = true;
    public bool Require4xx5xxResponses { get; init; } = true;
    public bool RequireOperationId { get; init; }
    public string NamingConvention { get; init; } = "none"; // "none" | "camelCase" | "snake_case"
}

public sealed record SaveRuleSetRequest(RuleConfig Config);
