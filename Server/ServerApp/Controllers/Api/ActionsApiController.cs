using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Factorio.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Api;

namespace ServerApp.Controllers.Api;

/// <summary>REST API: users submit requests to change item descriptions.</summary>
[ApiController]
[Route("api/actions")]
[Authorize(Roles = AuthService.UserRole)]
public class ActionsApiController(ActionService actions) : ControllerBase
{
    /// <summary>Submit a request to add, edit or delete an item description. It waits for admin review.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ActionAcceptedDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(CreateActionRequest request)
    {
        // The user id is the "sub" claim of the access token (set at login)
        var userIdText = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                         ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdText, out var userId))
            return Unauthorized();

        var data = new ActionData(userId, request.ItemId!.Value, request.Type!.Value, request.Details);

        var result = await actions.CreateAsync(data);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(result.Field ?? string.Empty, result.Error!);
            return ValidationProblem(ModelState);
        }

        return Accepted(new ActionAcceptedDto("Pending", "The request was saved and waits for admin review."));
    }
}
