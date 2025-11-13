using Microsoft.EntityFrameworkCore;
using Serilog;
using TestVue.Server.Data;
using TestVue.Server.Middleware;
using TestVue.Server.Services.FormSubmission;
using TestVue.Server.Stores.FormSubmission;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine(AppContext.BaseDirectory, "Logs", "app-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        fileSizeLimitBytes: 10485760,
        rollOnFileSizeLimit: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting TestVue Server application");

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Add health checks
builder.Services.AddHealthChecks();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "TestVue Form Submission API",
        Version = "v1",
        Description = "API for managing dynamic form submissions"
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register Entity Framework DbContext with In-Memory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("FormSubmissionsDb"));

// Register the data access store
builder.Services.AddScoped<IFormSubmissionStore, FormSubmissionStore>();

// Register the form submission service as scoped (EF Core best practice)
builder.Services.AddScoped<IFormSubmissionService, FormSubmissionService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.WithOrigins("https://localhost:5173", "http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Add error handling middleware (must be first)
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

// Enable CORS (before authentication/authorization)
app.UseCors("AllowVueApp");

app.UseAuthentication();
app.UseAuthorization();

// Map API endpoints first (before static files)
app.MapHealthChecks("/health");
app.MapControllers();

// Configure Swagger (available in all environments for API testing)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TestVue API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "TestVue API Documentation";
    });
}

// Static files and SPA fallback last
app.UseDefaultFiles();
app.MapStaticAssets();

// Fallback to index.html only for non-API routes
app.MapFallbackToFile("/index.html").Add(endpointBuilder =>
{
    ((Microsoft.AspNetCore.Routing.RouteEndpointBuilder)endpointBuilder).Order = int.MaxValue;
});

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
