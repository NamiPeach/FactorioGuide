namespace Factorio.Application.Common;

/// <summary>Outcome of a use case: success, "not found", or a validation error tied to an input field.</summary>
public class Result
{
    public bool Succeeded => Error is null && !IsNotFound;
    public bool IsNotFound { get; private init; }
    public string? Field { get; private init; }
    public string? Error { get; private init; }

    public static Result Ok() => new();
    public static Result NotFound() => new() { IsNotFound = true };
    public static Result Fail(string field, string error) => new() { Field = field, Error = error };
}
