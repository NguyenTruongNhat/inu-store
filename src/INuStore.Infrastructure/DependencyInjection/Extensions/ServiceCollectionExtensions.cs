using Amazon;
using Amazon.SimpleEmailV2;
using INuStore.Infrastructure.Authentication;
using INuStore.Infrastructure.Caching;
using INuStore.Infrastructure.DependencyInjection.Options;
using INuStore.Infrastructure.Notifications;
using INuStore.Infrastructure.Repositories;
using INuStore.Infrastructure.Security;
using INuStore.Infrastructure.Storage;
using INuStore.UseCase.Abstractions.Caching;
using INuStore.UseCase.Abstractions.Notifications;
using INuStore.UseCase.Abstractions.Repositories;
using INuStore.UseCase.Abstractions.Security;
using INuStore.UseCase.Abstractions.Storage;
using INuStore.UseCase.Abstractions.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
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
        services.AddInfrastructureService(configuration);
        services.AddRedisInfrastructure(configuration);

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
        services.AddTransient<INotificationRepository, NotificationRepository>();
        services.AddTransient<IApplicationParameterRepository, ApplicationParameterRepository>();
        services.AddTransient<IStoredFileRepository, StoredFileRepository>();

    }

    public static void AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IPasswordHasher, BCryptPasswordHasher>()
                .AddTransient<IJwtTokenService, JwtTokenService>()
                .AddTransient<ICacheService, CacheService>()
                .AddTransient<IFileStorage>(serviceProvider =>
                {
                    var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
                    var relativePath = configuration["FileStorage:Local:RootPath"]
                        ?? "App_Data/uploads";

                    var rootPath = Path.GetFullPath(relativePath, environment.ContentRootPath);
                    return new LocalFileStorage(rootPath);
                });
    }

    public static void AddRedisInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            redisOptions.Configuration = connectionString;
        });
    }


    public static IServiceCollection AddNotificationWorkerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureSqlServerRetryOptions(configuration.GetSection(nameof(SqlServerRetryOptions)));

        services.AddSqlConfiguration();
        services.AddRepositoryBaseConfiguration();


        services.AddTransient<ResendEmailSender>();
        services.AddTransient<SesEmailSender>();
        services.AddTransient<IEmailSender, ConfigurableEmailSender>();

        services.AddOptions<ResendOptions>()
                .Bind(configuration.GetSection(ResendOptions.SectionName))
                .ValidateDataAnnotations();

        services.AddOptions<SesOptions>()
                .Bind(configuration.GetSection(SesOptions.SectionName))
                .ValidateDataAnnotations();

        services.AddSingleton<IAmazonSimpleEmailServiceV2>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<SesOptions>>().Value;

            return new AmazonSimpleEmailServiceV2Client(
                options.AccessKeyId,
                options.SecretAccessKey,
                RegionEndpoint.GetBySystemName(options.Region));
        });
        return services;
    }


}
