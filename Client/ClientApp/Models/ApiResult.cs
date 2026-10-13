namespace ClientApp.Models;

/// <summary>Outcome of an API call, ready to show to the user.</summary>
public record ApiResult(bool Success, string Message)
{
    public static ApiResult Ok(string message = "") => new(true, message);
    public static ApiResult Fail(string message) => new(false, message);
}
