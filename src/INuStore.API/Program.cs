using INustore.UseCase.DependencyInjection.Extensions;
using INuStore.API.Middlewares;
using INuStore.Infrastructure.DependencyInjection.Extensions;
using INuStore.Infrastructure.DependencyInjection.Options;
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

builder.Services.AddScoped<TransactionMiddleware>();
// Infrastructure Dependency Injection
// Configure Options and SQL Server Retry Options
builder.Services.ConfigureSqlServerRetryOptions(builder.Configuration.GetSection(nameof(SqlServerRetryOptions)));
builder.Services.AddSqlConfiguration();
builder.Services.AddRepositoryBaseConfiguration();


// UseCase Dependency Injection
builder.Services.AddAppServices();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

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
