using Factorio.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Api;

namespace ServerApp.Controllers.Api;

/// <summary>REST API: login of registered users (data from the Users table).</summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthApiController(AuthService auth) : ControllerBase
{
    /// <summary>Checks email and password and returns a JWT access token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await auth.LoginAsync(request.Email, request.Password);
        if (token is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Invalid email or password."
            });
        }

        return Ok(new LoginResponse(token.Token, token.ExpiresAtUtc));
    }
}
