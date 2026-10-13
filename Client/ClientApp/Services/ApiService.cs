using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ClientApp.Models;

namespace ClientApp.Services;

/// <summary>Adapter: IApiService implemented with HttpClient and a JWT in the Authorization header.</summary>
public class ApiService(HttpClient http, ITokenStore tokens) : IApiService
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public bool IsLoggedIn => tokens.GetToken() is not null;

    public async Task<ApiResult> LoginAsync(string email, string password)
    {
        using var request = Create(HttpMethod.Post, "api/auth/login", new LoginRequestDto(email, password), withToken: false);
        using var response = await SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>(Json);
            if (body is null || string.IsNullOrEmpty(body.Token))
                return ApiResult.Fail("The server returned an empty token.");

            tokens.Save(body.Token, body.ExpiresAtUtc);
            return ApiResult.Ok();
        }

        return ApiResult.Fail(response.StatusCode == HttpStatusCode.Unauthorized
            ? "Invalid email or password."
            : await ReadErrorAsync(response));
    }

    public async Task<IReadOnlyList<ItemModel>> GetItemsAsync()
    {
        using var request = Create(HttpMethod.Get, "api/items");
        using var response = await SendAsync(request);
        EnsureAuthorized(response);

        if (!response.IsSuccessStatusCode)
            throw new ApiException(await ReadErrorAsync(response));

        return await response.Content.ReadFromJsonAsync<List<ItemModel>>(Json) ?? new List<ItemModel>();
    }

    public async Task<ApiResult> CreateActionAsync(int itemId, string type, string? details)
    {
        using var request = Create(HttpMethod.Post, "api/actions", new CreateActionRequestDto(itemId, type, details));
        using var response = await SendAsync(request);
        EnsureAuthorized(response);

        if (!response.IsSuccessStatusCode)
            return ApiResult.Fail(await ReadErrorAsync(response));

        var accepted = await response.Content.ReadFromJsonAsync<ActionAcceptedDto>(Json);
        return ApiResult.Ok(accepted?.Message ?? "Your request was sent and waits for admin review.");
    }

    public void Logout() => tokens.Clear();

    private HttpRequestMessage Create(HttpMethod method, string url, object? body = null, bool withToken = true)
    {
        var request = new HttpRequestMessage(method, url);

        var token = withToken ? tokens.GetToken() : null;
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (body is not null)
            request.Content = JsonContent.Create(body, body.GetType(), null, Json);

        return request;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        try
        {
            return await http.SendAsync(request);
        }
        catch (HttpRequestException)
        {
            throw new ApiException("Cannot reach the server. Check that it is running.");
        }
        catch (TaskCanceledException)
        {
            throw new ApiException("The server did not respond in time.");
        }
    }

    /// <summary>A 401 on a protected call means the token is missing, wrong or expired.</summary>
    private void EnsureAuthorized(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            tokens.Clear();
            throw new ApiException("Your session has expired. Sign out and sign in again.");
        }
    }

    /// <summary>Pulls a readable message out of a ProblemDetails / validation error body.</summary>
    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                foreach (var field in errors.EnumerateObject())
                {
                    if (field.Value.ValueKind != JsonValueKind.Array) continue;
                    foreach (var message in field.Value.EnumerateArray())
                        return message.GetString() ?? "Validation error.";
                }
            }

            foreach (var name in new[] { "detail", "title", "message" })
            {
                if (root.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String)
                    return value.GetString()!;
            }
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            // fall through to the generic message
        }
        return $"Server error ({(int)response.StatusCode}).";
    }
}
