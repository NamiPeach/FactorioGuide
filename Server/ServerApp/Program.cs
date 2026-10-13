using Factorio.Application.Services;
using System.Text;
using Factorio.Infrastructure;
using Factorio.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Connection string comes from appsettings.json (not committed to git, see appsettings.Example.json)
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' was not found in appsettings.json.");

builder.Services.AddInfrastructure(connectionString);

// Application layer: use cases
builder.Services.AddScoped<UserService>();

// JWT settings: Key, Issuer, Audience
var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName);
var jwtSettings = jwtSection.Get<JwtSettings>()
    ?? throw new InvalidOperationException("Section 'Jwt' was not found in appsettings.json.");
builder.Services.Configure<JwtSettings>(jwtSection);

// JWT authentication: the server accepts only tokens signed with our key
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // The admin panel (browser) keeps the token in a cookie.
                // If there is no cookie, the Authorization header is used (API clients, Postman).
                var cookieToken = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(cookieToken))
                {
                    context.Token = cookieToken;
                }

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                // Browser pages: send the visitor to the login page. API clients (/api) get a plain 401.
                if (!context.Request.Path.StartsWithSegments("/api"))
                {
                    context.HandleResponse();
                    var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
                    context.Response.Redirect($"/auth/login?returnUrl={returnUrl}");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// MVC: controllers and Razor views (admin panel)
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();