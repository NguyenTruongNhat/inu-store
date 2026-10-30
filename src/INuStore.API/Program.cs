using INustore.UseCase.DependencyInjection.Extensions;
using INuStore.API.Middlewares;
using INuStore.Infrastructure.DependencyInjection.Extensions;
using INuStore.Infrastructure.DependencyInjection.Options;
using Microsoft.AspNetCore.Mvc.Versioning;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog configuration
Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

// Register custom middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<TransactionMiddleware>();

// Infrastructure Dependency Injection
// Configure Options and SQL Server Retry Options
builder.Services.ConfigureSqlServerRetryOptions(builder.Configuration.GetSection(nameof(SqlServerRetryOptions)));
builder.Services.AddSqlConfiguration();
builder.Services.AddRepositoryBaseConfiguration();


// UseCase Dependency Injection
builder.Services.AddAppServices();
builder.Services.AddAppValidators();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Custom Middleware for Global transaction
app.UseMiddleware<TransactionMiddleware>();

app.MapControllers();

app.Run();
