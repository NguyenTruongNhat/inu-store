using INustore.UseCase.DependencyInjection.Extensions;
using INuStore.API.DependencyInjection.Extensions;
using INuStore.API.Middlewares;
using INuStore.Infrastructure.DependencyInjection.Extensions;
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

// Infrastructure Dependency Injection
builder.Services.AddInfrastructure(builder.Configuration);

// UseCase Dependency Injection
builder.Services.AddUseCase();

// API Dependency Injection
builder.Services.AddApi(builder.Configuration);


var app = builder.Build();
app.ConfigureSwagger();
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
