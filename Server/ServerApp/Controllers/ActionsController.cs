using Factorio.Application.Services;
using Factorio.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServerApp.Models;

namespace ServerApp.Controllers;

/// <summary>Admin panel for user actions: list, create, edit, delete and review (approve/reject).</summary>
[Authorize(Roles = "Admin")]
public class ActionsController(ActionService actions, UserService users, ItemService items) : Controller
{
    public async Task<IActionResult> Index(ActionStatus? status)
    {
        ViewData["Status"] = status;
        return View(await actions.GetAllAsync(status));
    }

    public async Task<IActionResult> Details(int id)
    {
        var action = await actions.GetAsync(id);
        if (action is null) return NotFound();

        return View(action);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Approve(int id, string? comment) => Review(id, true, comment);

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Reject(int id, string? comment) => Review(id, false, comment);

    public async Task<IActionResult> Create()
    {
        return View(await WithOptions(new ActionFormModel()));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActionFormModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await actions.CreateAsync(ToData(model));
            if (result.Succeeded) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(result.Field ?? string.Empty, result.Error!);
        }
        return View(await WithOptions(model));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var action = await actions.GetAsync(id);
        if (action is null) return NotFound();

        if (action.Status != ActionStatus.Pending)
        {
            TempData["Error"] = "Only pending actions can be edited.";
            return RedirectToAction(nameof(Index));
        }

        var model = new ActionFormModel
        {
            Id = action.Id,
            UserId = action.UserId,
            ItemId = action.ItemId,
            Type = action.Type,
            Details = action.ActionDetails
        };
        return View(await WithOptions(model));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ActionFormModel model)
    {
        if (id != model.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            var result = await actions.UpdateAsync(id, ToData(model));
            if (result.IsNotFound) return NotFound();
            if (result.Succeeded) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(result.Field ?? string.Empty, result.Error!);
        }
        return View(await WithOptions(model));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var action = await actions.GetAsync(id);
        if (action is null) return NotFound();

        return View(action);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await actions.DeleteAsync(id);
        if (result.IsNotFound) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> Review(int id, bool approve, string? comment)
    {
        var result = await actions.ReviewAsync(id, approve, comment);
        if (result.IsNotFound) return NotFound();

        if (!result.Succeeded)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Details), new { id });
        }
        return RedirectToAction(nameof(Index), new { status = ActionStatus.Pending });
    }

    private static ActionData ToData(ActionFormModel m) =>
        new(m.UserId!.Value, m.ItemId!.Value, m.Type, m.Details);

    private async Task<ActionFormModel> WithOptions(ActionFormModel model)
    {
        model.UserOptions = (await users.GetAllAsync())
            .Select(u => new SelectListItem($"{u.Name} ({u.Email})", u.Id.ToString()))
            .ToList();
        model.ItemOptions = (await items.GetAllAsync())
            .Select(i => new SelectListItem($"{i.Name} ({i.InternalName})", i.Id.ToString()))
            .ToList();
        return model;
    }
}
