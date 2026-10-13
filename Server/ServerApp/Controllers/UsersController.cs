using Factorio.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;

namespace ServerApp.Controllers;

/// <summary>Admin CRUD for registered users. All logic lives in UserService.</summary>
[Authorize(Roles = "Admin")]
public class UsersController(UserService users) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await users.GetAllAsync());
    }

    public IActionResult Create()
    {
        return View(new UserFormModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await users.CreateAsync(model.Name, model.Email, model.Password);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(result.Field ?? string.Empty, result.Error!);
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var user = await users.GetAsync(id);
        if (user is null) return NotFound();

        return View(new UserFormModel { Id = user.Id, Name = user.Name, Email = user.Email });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserFormModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
            return View(model);

        var result = await users.UpdateAsync(id, model.Name, model.Email, model.Password);
        if (result.IsNotFound) return NotFound();
        if (!result.Succeeded)
        {
            ModelState.AddModelError(result.Field ?? string.Empty, result.Error!);
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var user = await users.GetAsync(id);
        if (user is null) return NotFound();

        return View(user);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await users.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
