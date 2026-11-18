using INuStore.Infrastructure.Authentication;
using INuStore.Infrastructure.DependencyInjection.Options;
using INuStore.Infrastructure.Repositories;
using INuStore.Infrastructure.Security;
using INuStore.UseCase.Abstractions.Repositories;
using INuStore.UseCase.Abstractions.Security;
using INuStore.UseCase.Abstractions.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace INuStore.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureSqlServerRetryOptions(configuration.GetSection(nameof(SqlServerRetryOptions)));

        services.AddSqlConfiguration();
        services.AddHealthCheckConfiguration();
        services.AddRepositoryBaseConfiguration();
        services.AddPasswordHasherConfiguration();

        return services;
    }

    public static void AddSqlConfiguration(this IServiceCollection services)
    {
        services.AddDbContextPool<DbContext, ApplicationDbContext>((provider, builder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var options = provider.GetRequiredService<IOptionsMonitor<SqlServerRetryOptions>>();

            #region ============== SQL-SERVER-STRATEGY-DbContext 1 ==============
            //builder
            //.EnableDetailedErrors(true)
            //.EnableSensitiveDataLogging(true)
            //.UseLazyLoadingProxies(true)
            //.UseSqlServer(
            //    connectionString: configuration.GetConnectionString("ConnectionStrings")
            //    ,
            //    sqlServerOptionsAction: optionsBuilder
            //            => optionsBuilder.ExecutionStrategy(
            //                    dependencies => new SqlServerRetryingExecutionStrategy(
            //                        dependencies: dependencies,
            //                        maxRetryCount: options.CurrentValue.MaxRetryCount,
            //                        maxRetryDelay: options.CurrentValue.MaxRetryDelay,
            //                        errorNumbersToAdd: options.CurrentValue.ErrorNumbersToAdd))
            //                .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));

            #endregion

            #region ============== SQL-SERVER-STRATEGY- UnitOfWork 2 ==============
            builder
            .EnableDetailedErrors(true)
            .EnableSensitiveDataLogging(true)
            .UseLazyLoadingProxies(false)
            .UseSqlServer(
                connectionString: configuration.GetConnectionString("ConnectionStrings"),
                sqlServerOptionsAction: optionsBuilder
                            => optionsBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));
            #endregion
        });
    }

    public static void AddHealthCheckConfiguration(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<DbContext>(
                name: "sqlserver",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"]);

        services.PostConfigure<HealthCheckServiceOptions>(options =>
        {
            options.Registrations.Single(registration => registration.Name == "sqlserver")
                .Timeout = TimeSpan.FromSeconds(5);
        });
    }

    public static OptionsBuilder<SqlServerRetryOptions> ConfigureSqlServerRetryOptions(this IServiceCollection services, IConfigurationSection section)
    => services
        .AddOptions<SqlServerRetryOptions>()
        .Bind(section)
        .ValidateDataAnnotations()
        .ValidateOnStart();


    public static void AddRepositoryBaseConfiguration(this IServiceCollection services)
    {
        services.AddTransient(typeof(IUnitOfWork), typeof(EFUnitOfWork));
        services.AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));

        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IRoleRepository, RoleRepository>();
        services.AddTransient<IPermissionRepository, PermissionRepository>();

    }

    public static void AddPasswordHasherConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IPasswordHasher, BCryptPasswordHasher>()
                .AddTransient<IJwtTokenService, JwtTokenService>();
    }


}
