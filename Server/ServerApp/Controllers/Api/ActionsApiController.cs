using Factorio.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Api;

namespace ServerApp.Controllers.Api;

/// <summary>REST API: users submit requests to change item descriptions.</summary>
[ApiController]
[Route("api/actions")]
[Authorize]
public class ActionsApiController(ActionService actions) : ControllerBase
{
    /// <summary>Submit a request to add, edit or delete an item description. It waits for admin review.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ActionAcceptedDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateActionRequest request)
    {
        var data = new ActionData(
            request.UserId!.Value, request.ItemId!.Value, request.Type!.Value, request.Details);

        var result = await actions.CreateAsync(data);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(result.Field ?? string.Empty, result.Error!);
            return ValidationProblem(ModelState);
        }

        return Accepted(new ActionAcceptedDto("Pending", "The request was saved and waits for admin review."));
    }
}
