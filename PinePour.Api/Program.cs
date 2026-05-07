using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using PinePour.Api.Data;
using PinePour.Api.Features.Auth;
using PinePour.Api.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var spaDevServerUrl = builder.Configuration["Spa:DevServerUrl"] ?? "http://localhost:5173";
var isRunningInContainer = string.Equals(
    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
    "true",
    StringComparison.OrdinalIgnoreCase);

var databaseProvider = builder.Configuration["Database:Provider"] ?? "SqlServer";
var connectionString = builder.Configuration.GetConnectionString("DataContext");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Missing database connection string. Set ConnectionStrings__DataContext as an environment variable.");
}

builder.Services.AddDbContext<DataContext>(options =>
{
    if (databaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase)
        || databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
    {
        options.UseNpgsql(PostgresConnectionStringNormalizer.Normalize(connectionString));
        return;
    }

    if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
        || databaseProvider.Equals("Sql", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(connectionString);
        return;
    }

    throw new InvalidOperationException($"Unsupported Database:Provider '{databaseProvider}'.");
});

var configuredCorsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();
var allowedCorsOrigins = configuredCorsOrigins
    .Concat(new[]
    {
        "http://localhost:5173",
        "http://localhost:5174",
        "http://localhost:8081",
        "http://localhost:19006",
        "http://10.0.2.2:8081",
    })
    .Distinct()
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(allowedCorsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<DataContext>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<PushNotificationService>();
builder.Services.AddScoped<StarEarningService>();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("api", config =>
    {
        config.PermitLimit = 120;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueLimit = 0;
        config.AutoReplenishment = true;
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await SeedHelper.MigrateAndSeed(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!isRunningInContainer && !app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontend");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseWhen(
        context =>
            !context.Request.Path.StartsWithSegments("/api")
            && !context.Request.Path.StartsWithSegments("/swagger"),
        spaApp =>
        {
            spaApp.UseSpa(x =>
            {
                x.UseProxyToSpaDevelopmentServer(spaDevServerUrl);
            });
        });
}
else
{
    app.MapFallbackToFile("/index.html");
}

app.Run();

public partial class Program { }
