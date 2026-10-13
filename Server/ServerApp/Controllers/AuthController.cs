using Factorio.Application.Abstractions;
using Factorio.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;

namespace ServerApp.Controllers;

/// <summary>Admin panel login: GET /auth/login shows the form, POST /auth/login issues the JWT.</summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Route("auth")]
public class AuthController : Controller
{
    private const string TokenCookieName = "access_token";

    private readonly IJwtTokenService _tokenService;

    public AuthController(IJwtTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginModel { ReturnUrl = returnUrl });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!AdminAccount.IsValid(model.Login, model.Password))
        {
            ModelState.AddModelError(string.Empty, "Invalid login or password.");
            return View(model);
        }

        // Valid admin: issue the JWT and keep it in an HttpOnly cookie (JavaScript cannot read it)
        var token = _tokenService.CreateToken(AdminAccount.Login, "Admin");
        Response.Cookies.Append(TokenCookieName, token.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = token.ExpiresAtUtc
        });

        var target = Url.IsLocalUrl(model.ReturnUrl) ? model.ReturnUrl! : "/";
        return LocalRedirect(target);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(TokenCookieName);
        return RedirectToAction(nameof(Login));
    }
}
