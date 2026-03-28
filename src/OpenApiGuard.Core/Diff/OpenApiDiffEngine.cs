using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace OpenApiGuard.Core.Diff;

public static class OpenApiDiffEngine
{
    public static DiffResult Compare(string oldSpecText, string newSpecText)
    {
        var oldDoc = Parse(oldSpecText);
        var newDoc = Parse(newSpecText);

        var items = new List<DiffItem>();

        CompareEndpoints(oldDoc, newDoc, items);

        int breaking = items.Count(i => i.Severity == "breaking");
        int warning = items.Count(i => i.Severity == "warning");
        int info = items.Count(i => i.Severity == "info");

        return new DiffResult(items, breaking, warning, info);
    }

    public static OpenApiDocument Parse(string specText)
    {
        var reader = new OpenApiStringReader();
        var doc = reader.Read(specText, out var diagnostic);

        if (doc is null || diagnostic.Errors.Count > 0)
        {
            var errors = string.Join("; ", diagnostic.Errors.Select(e => e.Message));
            throw new InvalidOperationException($"Failed to parse OpenAPI spec: {errors}");
        }

        return doc;
    }

    public static void ValidateV3Only(string specText)
    {
        // Quick check for Swagger v2
        if (specText.Contains("\"swagger\"") || specText.Contains("swagger:"))
        {
            var trimmed = specText.TrimStart();
            if (trimmed.StartsWith("{"))
            {
                // JSON — check for "swagger": "2.
                if (specText.Contains("\"swagger\""))
                    throw new InvalidOperationException(
                        "Only OpenAPI v3 is supported. Detected Swagger/OpenAPI v2.");
            }
            else
            {
                // YAML
                if (specText.Contains("swagger:"))
                    throw new InvalidOperationException(
                        "Only OpenAPI v3 is supported. Detected Swagger/OpenAPI v2.");
            }
        }

        // Parse to validate it's actually v3
        var reader = new OpenApiStringReader();
        var doc = reader.Read(specText, out var diagnostic);

        if (doc is null || diagnostic.Errors.Count > 0)
        {
            var errors = string.Join("; ", diagnostic.Errors.Select(e => e.Message));
            throw new InvalidOperationException($"Invalid OpenAPI spec: {errors}");
        }
    }

    private static void CompareEndpoints(
        OpenApiDocument oldDoc,
        OpenApiDocument newDoc,
        List<DiffItem> items)
    {
        var oldOps = FlattenOperations(oldDoc);
        var newOps = FlattenOperations(newDoc);

        // Removed endpoints
        foreach (var (key, _) in oldOps)
        {
            if (!newOps.ContainsKey(key))
            {
                items.Add(new DiffItem(
                    "breaking",
                    "endpoint_removed",
                    key,
                    $"Endpoint removed: {key}"));
            }
        }

        // Added endpoints
        foreach (var (key, _) in newOps)
        {
            if (!oldOps.ContainsKey(key))
            {
                items.Add(new DiffItem(
                    "info",
                    "endpoint_added",
                    key,
                    $"Endpoint added: {key}"));
            }
        }

        // Changed endpoints
        foreach (var (key, oldOp) in oldOps)
        {
            if (newOps.TryGetValue(key, out var newOp))
            {
                CompareParameters(key, oldOp, newOp, items);
                CompareResponseSchema(key, oldOp, newOp, items);
                CompareRequestBody(key, oldOp, newOp, items);
            }
        }
    }

    private static void CompareParameters(
        string opKey,
        OpenApiOperation oldOp,
        OpenApiOperation newOp,
        List<DiffItem> items)
    {
        var oldParams = oldOp.Parameters
            .ToDictionary(p => $"{p.In}:{p.Name}", p => p);
        var newParams = newOp.Parameters
            .ToDictionary(p => $"{p.In}:{p.Name}", p => p);

        foreach (var (pKey, oldParam) in oldParams)
        {
            if (!newParams.ContainsKey(pKey))
            {
                items.Add(new DiffItem(
                    "breaking",
                    "parameter_removed",
                    opKey,
                    $"Parameter removed: {oldParam.Name} (in {oldParam.In})"));
            }
            else
            {
                var newParam = newParams[pKey];

                if (oldParam.Required != newParam.Required)
                {
                    if (newParam.Required)
                    {
                        items.Add(new DiffItem(
                            "breaking",
                            "parameter_required_changed",
                            opKey,
                            $"Parameter '{oldParam.Name}' changed from optional to required"));
                    }
                    else
                    {
                        items.Add(new DiffItem(
                            "info",
                            "parameter_required_changed",
                            opKey,
                            $"Parameter '{oldParam.Name}' changed from required to optional"));
                    }
                }

                var oldType = oldParam.Schema?.Type ?? "unknown";
                var newType = newParam.Schema?.Type ?? "unknown";
                if (oldType != newType)
                {
                    items.Add(new DiffItem(
                        "breaking",
                        "parameter_type_changed",
                        opKey,
                        $"Parameter '{oldParam.Name}' type changed: {oldType} → {newType}"));
                }
            }
        }

        foreach (var (pKey, newParam) in newParams)
        {
            if (!oldParams.ContainsKey(pKey))
            {
                var severity = newParam.Required ? "breaking" : "info";
                items.Add(new DiffItem(
                    severity,
                    "parameter_added",
                    opKey,
                    $"Parameter added: {newParam.Name} (in {newParam.In}, required={newParam.Required})"));
            }
        }
    }

    private static void CompareResponseSchema(
        string opKey,
        OpenApiOperation oldOp,
        OpenApiOperation newOp,
        List<DiffItem> items)
    {
        var oldSchema = GetResponseSchema(oldOp, "200");
        var newSchema = GetResponseSchema(newOp, "200");

        if (oldSchema is null && newSchema is null) return;

        if (oldSchema is not null && newSchema is null)
        {
            items.Add(new DiffItem(
                "breaking",
                "response_schema_removed",
                opKey,
                "200 response schema removed"));
            return;
        }

        if (oldSchema is null) return;

        CompareSchemaProperties(opKey, "response", oldSchema, newSchema!, items);
    }

    private static void CompareRequestBody(
        string opKey,
        OpenApiOperation oldOp,
        OpenApiOperation newOp,
        List<DiffItem> items)
    {
        var oldSchema = GetRequestBodySchema(oldOp);
        var newSchema = GetRequestBodySchema(newOp);

        if (oldSchema is null && newSchema is null) return;

        if (oldSchema is not null && newSchema is null)
        {
            items.Add(new DiffItem(
                "breaking",
                "request_body_removed",
                opKey,
                "Request body removed"));
            return;
        }

        if (oldSchema is null && newSchema is not null)
        {
            items.Add(new DiffItem(
                "breaking",
                "request_body_added",
                opKey,
                "Request body added (may break existing clients)"));
            return;
        }

        CompareSchemaProperties(opKey, "request", oldSchema!, newSchema!, items);
    }

    private static void CompareSchemaProperties(
        string opKey,
        string context,
        OpenApiSchema oldSchema,
        OpenApiSchema newSchema,
        List<DiffItem> items)
    {
        var oldProps = oldSchema.Properties ?? new Dictionary<string, OpenApiSchema>();
        var newProps = newSchema.Properties ?? new Dictionary<string, OpenApiSchema>();

        var oldRequired = new HashSet<string>(oldSchema.Required ?? (IEnumerable<string>)[]);
        var newRequired = new HashSet<string>(newSchema.Required ?? (IEnumerable<string>)[]);

        foreach (var (name, oldProp) in oldProps)
        {
            if (!newProps.ContainsKey(name))
            {
                items.Add(new DiffItem(
                    "breaking",
                    "schema_field_removed",
                    opKey,
                    $"{context} field removed: {name} ({oldProp.Type})"));
            }
            else
            {
                var newProp = newProps[name];
                var oldType = oldProp.Type ?? "unknown";
                var newType = newProp.Type ?? "unknown";

                if (oldType != newType)
                {
                    items.Add(new DiffItem(
                        "breaking",
                        "schema_field_type_changed",
                        opKey,
                        $"{context} field '{name}' type changed: {oldType} → {newType}"));
                }

                bool wasRequired = oldRequired.Contains(name);
                bool isRequired = newRequired.Contains(name);
                if (!wasRequired && isRequired)
                {
                    items.Add(new DiffItem(
                        "breaking",
                        "schema_field_required_changed",
                        opKey,
                        $"{context} field '{name}' changed from optional to required"));
                }
                else if (wasRequired && !isRequired)
                {
                    items.Add(new DiffItem(
                        "info",
                        "schema_field_required_changed",
                        opKey,
                        $"{context} field '{name}' changed from required to optional"));
                }
            }
        }

        foreach (var (name, newProp) in newProps)
        {
            if (!oldProps.ContainsKey(name))
            {
                bool isRequired = newRequired.Contains(name);
                var severity = (context == "response" || !isRequired) ? "info" : "breaking";
                items.Add(new DiffItem(
                    severity,
                    "schema_field_added",
                    opKey,
                    $"{context} field added: {name} ({newProp.Type}, required={isRequired})"));
            }
        }
    }

    private static OpenApiSchema? GetResponseSchema(OpenApiOperation op, string statusCode)
    {
        if (op.Responses is null) return null;
        if (!op.Responses.TryGetValue(statusCode, out var response)) return null;
        if (response.Content is null) return null;
        if (response.Content.TryGetValue("application/json", out var mediaType))
            return mediaType.Schema;
        return response.Content.Values.FirstOrDefault()?.Schema;
    }

    private static OpenApiSchema? GetRequestBodySchema(OpenApiOperation op)
    {
        if (op.RequestBody?.Content is null) return null;
        if (op.RequestBody.Content.TryGetValue("application/json", out var mediaType))
            return mediaType.Schema;
        return op.RequestBody.Content.Values.FirstOrDefault()?.Schema;
    }

    private static Dictionary<string, OpenApiOperation> FlattenOperations(OpenApiDocument doc)
    {
        var ops = new Dictionary<string, OpenApiOperation>();
        if (doc.Paths is null) return ops;

        foreach (var (path, pathItem) in doc.Paths)
        {
            foreach (var (method, operation) in pathItem.Operations)
            {
                var key = $"{method.ToString().ToUpperInvariant()} {path}";
                ops[key] = operation;
            }
        }

        return ops;
    }
}
