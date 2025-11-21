using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using EFCore.DBContext;
using EFCore.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
    // In Program.cs
    builder.Services.AddDbContext<StudentDbContext>(options =>
    {
        //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        // Or for SQLite:
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
            .LogTo(Console.WriteLine, LogLevel.Information);

        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
        }
    });
    builder.Services.AddAuthentication()
        .AddJwtBearer(options =>
        {
            options.Authority = "https://login.microsoftonline.com/d1977dc2-66b5-4d78-ba80-11aa9bc03829/v2.0";
            //options.RequireHttpsMetadata = false;
            options.Audience = "315c42e7-d68c-44f3-a64a-86c01d60011c";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuers = new[]
                {
                    "https://sts.windows.net/d1977dc2-66b5-4d78-ba80-11aa9bc03829/",
                    "https://login.microsoftonline.com/d1977dc2-66b5-4d78-ba80-11aa9bc03829/v2.0"
                }
            };

        });
    builder.Services.AddAuthorizationBuilder();
   

    // In Program.cs
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0); // Set a default version
        options.AssumeDefaultVersionWhenUnspecified = true; // Use default if no version is provided
        options.ReportApiVersions = true; // Include supported versions in response headers
        // Configure how the API version is read from the request (e.g., URL segment)
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("X-Api-Version"));
    });
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 10,
                    QueueLimit = 0,
                    Window = TimeSpan.FromMinutes(1)
                }));
    });
    builder.Services.AddCors();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSingleton<ExceptionHandlingMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRateLimiter();
app.UseHttpsRedirection();
app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHsts();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
