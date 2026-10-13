namespace ClientApp.Models;

// Request and response bodies of the REST API (client-side copies of the server DTOs)
public record LoginRequestDto(string Email, string Password);
public record LoginResponseDto(string Token, DateTime ExpiresAtUtc);
public record CreateActionRequestDto(int ItemId, string Type, string? Details);
public record ActionAcceptedDto(string? Status, string? Message);
