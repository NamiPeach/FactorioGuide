using Factorio.Domain.Entities;
using Factorio.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers;

/// <summary>Admin CRUD for registered users.</summary>
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _db.Users.AsNoTracking().OrderBy(u => u.Name).ToListAsync();
        return View(users);
    }

    public IActionResult Create()
    {
        return View(new UserFormModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Password))
            ModelState.AddModelError(nameof(model.Password), "Password is required.");

        var email = model.Email.Trim().ToLower();
        if (ModelState.IsValid && await _db.Users.AnyAsync(u => u.Email.ToLower() == email))
            ModelState.AddModelError(nameof(model.Email), "A user with this email already exists.");

        if (!ModelState.IsValid)
            return View(model);

        _db.Users.Add(new User
        {
            Name = model.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        return View(new UserFormModel { Id = user.Id, Name = user.Name, Email = user.Email });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserFormModel model)
    {
        if (id != model.Id) return BadRequest();

        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        var email = model.Email.Trim().ToLower();
        if (ModelState.IsValid && await _db.Users.AnyAsync(u => u.Id != id && u.Email.ToLower() == email))
            ModelState.AddModelError(nameof(model.Email), "A user with this email already exists.");

        if (!ModelState.IsValid)
            return View(model);

        user.Name = model.Name.Trim();
        user.Email = email;
        if (!string.IsNullOrWhiteSpace(model.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        return View(user);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is not null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
