using Factorio.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Connection string comes from appsettings.json (not committed to git, see appsettings.Example.json)
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' was not found in appsettings.json.");

builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
