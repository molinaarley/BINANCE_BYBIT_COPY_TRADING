using CleanArchitecture.WEB.Middleware;
using CleanArchitecture.Application;
using CleanArchitecture.Identity;
using CleanArchitecture.Infrastructure;
using Microsoft.OpenApi.Models;
using System.Reflection;
using MediatR;
using CleanArchitecture.Application.Features.Binance.Queries.GetTraderUrlForUpdatePositionBinance;
using CleanArchitecture.Application.Models;
using Microsoft.Extensions.ML;

var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

// Construcción explícita para compatibilidad Docker
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

// Configuración de archivos de configuración
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Docker.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Kestrel solo para Docker y uso directo
builder.WebHost.ConfigureKestrel(options =>
{
    // Solo usar Kestrel cuando se ejecuta directamente, no desde IISExpress
    if (isDocker)
    {
        options.ListenAnyIP(80);
    }
});

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TAINO SOLUTIONS API BINANCE leaderboard",
        Description = "API TO UPDATE BYBIT WITH THE POSITIONS OF THE BINANCE leaderboard",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "TAINO SOLUTIONS",
            Url = new Uri("https://www.societe.com/societe/taino-solutions-891442444.html")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

Console.WriteLine("***** Program.cs init *****");
Console.WriteLine($"IS_DOCKER: {isDocker}");
Console.WriteLine($"ENV: {env}");

builder.Services.AddMediatR(
    typeof(CleanArchitecture.Application.AssemblyReference).Assembly,
    typeof(CleanArchitecture.Infrastructure.AssemblyReference).Assembly
);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.ConfigureIdentityServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment() && !isDocker)
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || isDocker)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();
