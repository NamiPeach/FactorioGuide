using Factorio.Application.Common;
using Factorio.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServerApp.Models;

namespace ServerApp.Controllers;

/// <summary>Admin CRUD for items. All logic lives in ItemService.</summary>
[Authorize(Roles = "Admin")]
public class ItemsController(ItemService items, UserService users) : Controller
{
    public async Task<IActionResult> Index(string? q)
    {
        ViewData["Query"] = q;
        return View(await items.GetAllAsync(q));
    }

    public async Task<IActionResult> Create()
    {
        return View(await WithUserOptions(new ItemFormModel()));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ItemFormModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await items.CreateAsync(ToData(model));
            if (result.Succeeded) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(result.Field ?? string.Empty, result.Error!);
        }
        return View(await WithUserOptions(model));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await items.GetAsync(id);
        if (item is null) return NotFound();

        var model = new ItemFormModel
        {
            Id = item.Id,
            InternalName = item.InternalName,
            Name = item.Name,
            Category = item.Category,
            Description = item.Description,
            StackSize = item.StackSize,
            CraftTime = item.CraftTime,
            ProductAmount = item.ProductAmount,
            Stats = item.Stats,
            UserId = item.UserId
        };
        return View(await WithUserOptions(model));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ItemFormModel model)
    {
        if (id != model.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            var result = await items.UpdateAsync(id, ToData(model));
            if (result.IsNotFound) return NotFound();
            if (result.Succeeded) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(result.Field ?? string.Empty, result.Error!);
        }
        return View(await WithUserOptions(model));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await items.GetAsync(id);
        if (item is null) return NotFound();

        return View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await items.DeleteAsync(id);
        if (result.IsNotFound) return NotFound();
        if (!result.Succeeded) TempData["Error"] = result.Error;

        return RedirectToAction(nameof(Index));
    }

    private static ItemData ToData(ItemFormModel m) =>
        new(m.InternalName, m.Name, m.Category, m.Description, m.StackSize,
            m.CraftTime, m.ProductAmount, m.Stats, m.UserId);

    private async Task<ItemFormModel> WithUserOptions(ItemFormModel model)
    {
        model.UserOptions = (await users.GetAllAsync())
            .Select(u => new SelectListItem($"{u.Name} ({u.Email})", u.Id.ToString()))
            .ToList();
        var cats = ItemCategories.All.ToList();
        if (!string.IsNullOrEmpty(model.Category) && !cats.Contains(model.Category)) cats.Add(model.Category);
        model.CategoryOptions = cats.Select(c => new SelectListItem(c, c)).ToList();
        return model;
    }
}
