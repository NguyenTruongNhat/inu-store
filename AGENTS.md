# Repository Guidelines

INuStore is a .NET 10 ASP.NET Core API for authentication, users, roles, and user-role assignments. It uses EF Core with SQL Server. This guide applies to the whole repository. [AGENTS-VN.md](AGENTS-VN.md) is the Vietnamese companion. Check the owning source before relying on examples in [technical-docs](technical-docs/).

## Project map and dependencies

[INuStore.slnx](INuStore.slnx) contains five source projects and one test project.

| Location | Responsibility |
| --- | --- |
| `src/INuStore.API/` | HTTP endpoints, JWT Bearer authentication and policies, validation filters, middleware, health probes, versioned Swagger, and service composition. Start with `Program.cs`. |
| `src/INuStore.Contract/` | Versioned request and response records under `Service/V1/{Auth,User,Role}/`. Shared `Result`, validation errors, and pagination types live in `Abstractions/Share/`. |
| `src/INuStore.Domain/` | `User`, `Role`, `UserRole`, `RoleType`, and domain exceptions. |
| `src/INustore.UseCase/` | Service, repository, unit-of-work, and security interfaces in `Abstractions/`. Business services and validators live in `UseCases/V1/`. Mapping profiles live in `Mappings/`. Keep this spelling for filesystem paths and use `INuStore.UseCase` for namespaces. |
| `src/INuStore.Infrastructure/` | EF context, repository and unit-of-work implementations, SQL Server exception translation, entity mappings, migrations, BCrypt, and JWT generation. |
| `tests/INuStore.Tests/` | xUnit tests with Moq and NetArchTest. The Visual Studio solution folder is `/tets/` and the disk folder is `tests/`. |

Domain and Contract have no references to other solution layers. UseCase depends on Domain and Contract. Infrastructure implements UseCase abstractions and may depend on Domain and Contract. API composes UseCase and Infrastructure. `ArchitectureTests.cs` checks these boundaries. Contract already references EF Core for `PagedResult<T>.CreateAsync`.

Keep HTTP concerns in API and persistence details in Infrastructure. Controllers call UseCase services directly. Register services through the existing `AddInfrastructure(configuration)`, `AddUseCase()`, and `AddApi(configuration)` extensions. `IUserService` and `IRoleService` are scoped. `IAuthService`, repositories, unit of work, password hashing, and token services are transient. Validators and AutoMapper profiles are discovered in the UseCase assembly. `TransactionMiddleware` is scoped and `ExceptionHandlingMiddleware` is transient.

## Request and database flow

- `ApiController` uses `api/v{version:apiVersion}/[controller]`. Auth, User, and Role declare version 1. Use the action's binding attributes to distinguish query, route, and JSON body parameters. Update `src/INuStore.API/INuStore.API.http` when changing an endpoint.
- `[Validate<TModel>]` attaches `ValidationFilter<TModel>` to run the registered FluentValidation validators on the bound argument. It uses `RequestAborted`, deduplicates errors, and returns HTTP 400 with `ValidationResult.WithErrors`. Apply the attribute on actions that need these rules. MVC binding and data-annotation errors can use the default `[ApiController]` response.
- `ExceptionHandlingMiddleware` is registered before authorization and transactions in `Program.cs`. It maps exceptions to JSON `{ title, status, detail, errors }`. Invalid credentials map to 401, domain bad requests to 400, not-found exceptions to 404, and `FormatException` to 422. Other exceptions, including UseCase `ValidationException`, currently map to 500. The middleware includes `exception.Message` in `detail`. Keep sensitive data out of exception messages returned by the API.
- After authorization, `TransactionMiddleware` opens a transaction for POST, PUT, PATCH, and DELETE before MVC binding and validation. This includes POST login. It buffers the response and rolls back on status >= 400. For other statuses it awaits `IUnitOfWork.SaveChangesAsync`, commits, then copies the response. Exceptions propagate to the outer middleware after disposal of the uncommitted transaction. GET bypasses the transaction.
- Repository writes stage EF changes. Keep a single owner for saving and committing. Application writes save through `IUnitOfWork.SaveChangesAsync`. A caller outside this HTTP pipeline needs an explicit persistence boundary. Controllers currently return `Ok(result)`. An unsuccessful `Result` alone does not trigger rollback. Map failures to an appropriate HTTP status and avoid serializing a failed `Result<T>` directly because its `Value` getter throws.
- `AddDbContextPool<DbContext, ApplicationDbContext>` supplies the request's shared context to middleware, repositories, and unit of work. Await operations on that instance sequentially, including `AsNoTracking()` queries. `FindAll` returns an untracked query. `FindByIdAsync` and `FindSingleAsync` enable tracking. `GetUserWithRolesAsync` uses `Include` and `ThenInclude` with no tracking. Lazy-loading proxies are disabled. [EF Core guidance on DbContext concurrency](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#avoiding-dbcontext-threading-issues)
- `EFUnitOfWork` passes `DbUpdateException` to `SqlServerExceptionTranslator`. The translator recognizes duplicate role names and attempts to delete assigned roles using SQL Server error numbers, constraint names in message text, and entity state. Preserve the original exception as `InnerException` and propagate unrecognized errors unchanged. Direct context saves, including role seeding, bypass translation.
- `SqlServerRetryOptions` is bound and validated at startup. The retry execution strategy is commented out. Review explicit transaction ownership before enabling retries.

## Current endpoint behavior

| Endpoint or feature | Behavior to verify before changing it |
| --- | --- |
| `POST /api/v1/Auth/Login` | Looks up the username and verifies the stored BCrypt hash. Unknown user and incorrect password produce the same 401 and generate no tokens. Success returns an HS256 access token with user ID, username, and email claims, plus a randomly generated refresh token. The current login flow returns the refresh token without persisting it, and the API has no refresh endpoint. |
| User CRUD | `CreateUser` maps one user, hashes the password through `IPasswordHasher`, and stages the insert. `UserProfile` temporarily maps the input password to `PasswordHash`, which the service overwrites before `Add`. Keep passwords and hashes out of response DTOs. `GetAllUsers` currently loads the full list despite accepting `PageIndex` and `PageSize`. `GetUserById` requires `UserId > 7` and throws when the user is missing. Update requires a positive ID. Update and delete return success for missing users. The delete lookup currently omits the cancellation token. |
| `/api/v1/Role/{GetAllRoles,GetRoleById,CreateRole,UpdateRole,DeleteRole}` | All actions require authentication and successful actions return HTTP 200 with a `Result` envelope. The list is ordered by ID and role DTOs omit assignments. A missing role on get, update, or delete returns 404. Invalid input and role-rule violations return 400. |
| `GET /api/v1/User/{userId}/roles` and `PUT` or `DELETE /api/v1/User/{userId}/roles/{roleId}` | Require authentication. Route IDs are positive integers. The read returns roles ordered by name. Assignment and removal look up the user and role, return 404 for missing entities, and succeed without staging another change when the requested assignment state already holds. |
| `PUT /api/v1/User/{userId}/roles` | Requires authentication. Separate filters validate a positive route ID and a nonempty body `roleIds` list of positive integers. The service queries existing roles for the IDs and loads the user sequentially. A missing user returns 404. Missing role IDs are currently skipped for additions, while removals use the submitted list, so a request with unknown IDs can partially replace or clear the assignments and still return success. Concurrent replacements are not serialized. |
| `/health/live` and `/health/ready` | Anonymous and unversioned. Liveness excludes all dependency checks. Readiness runs the `sqlserver` connectivity check against the EF context, tagged `ready` with a five-second timeout. Expected default results are plaintext `200 Healthy` or `503 Unhealthy`. Readiness checks connectivity without applying migrations or checking schema compatibility. |

Role names are trimmed, required, and limited to 256 characters. Uniqueness follows SQL Server collation. `RoleType` names are reserved case-insensitively. A built-in role can change description but cannot be renamed or deleted. Assigned roles cannot be deleted. The composite `UserId`/`RoleId` key prevents duplicate assignments. The unique role-name index and non-cascading role foreign key require migration `20261109132818_ProtectRoleNamesAndAssignments`. Resolve existing duplicate role names before applying it.

`AddApi` registers JWT Bearer authentication and the `allow-gmail` policy. `WebApplication` adds authentication middleware automatically with this registration. `Program.cs` calls `UseAuthorization()` explicitly. `GetUserById` requires an authenticated user with an email claim ending in `@gmail.com`. `GetAllUsers`, role CRUD, and user-role endpoints require authentication. User create, update, delete, and login have no authorization attribute or configured fallback policy. JWT validation checks lifetime and the signing key. Issuer and audience validation are disabled in the current configuration. [ASP.NET Core middleware registration](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0#middleware-added-automatically-by-webapplication)

Swagger defines a shared HTTP Bearer scheme. Paste the raw access token into Authorize. Endpoint access follows the actual authorization metadata. Development calls `MapOpenApi()`, and the current service registrations lack `AddOpenApi()`. Verify Development startup when working on API documentation or startup.

## Local development and migrations

Run from the repository root with the .NET 10 SDK. The repository does not pin the SDK with `global.json`.

```powershell
dotnet restore INuStore.slnx
dotnet build INuStore.slnx --no-restore
dotnet test INuStore.slnx --no-build
dotnet run --project src/INuStore.API --launch-profile https
```

Both launch profiles use Development. The HTTPS profile declares `https://localhost:7005` and `http://localhost:6000`. The `.http` file targets HTTPS. Swagger UI is at `/swagger/index.html` and `/` redirects there. For optional coverage, run `dotnet test tests/INuStore.Tests --collect:"XPlat Code Coverage"`.

Configure an accessible SQL Server instance through `ConnectionStrings:ConnectionStrings` or `ConnectionStrings__ConnectionStrings`. JWT settings are under `JwtOption`. Keep real credentials, signing keys, tokens, and hashes out of source, logs, and examples. EF detailed errors and sensitive-data logging are enabled in Infrastructure. Account for those settings when preparing non-development configuration. Serilog writes to the console and daily files under `logs/`.

Use a `dotnet-ef` version matching the EF Core 10 packages and check `dotnet ef --version` before generating migrations. There is no repository-local tool manifest. Infrastructure owns migrations, API supplies startup configuration, and `ApplicationDbContext` owns the model. API startup leaves schema updates to explicit migration commands. See the [EF migration guide](technical-docs/5.ef-core-migrations.MD) for CLI and Package Manager Console.

```powershell
dotnet ef migrations add DescribeSchemaChange --project src/INuStore.Infrastructure --startup-project src/INuStore.API --context ApplicationDbContext -- --environment Development
dotnet ef database update --project src/INuStore.Infrastructure --startup-project src/INuStore.API --context ApplicationDbContext -- --environment Development
```

Generate migrations for requested schema changes. Review the migration and snapshot together. Confirm the target database before applying the update. `Directory.Build.props` contains a temporary, advisory-specific AutoMapper audit suppression. Treat it as a tooling exception and retain all other dependency and security warnings.

## Editing, testing, and delivery

- Follow [the repository EditorConfig](.editorconfig), including four-space indentation, file-scoped namespaces, and braces on separate lines. Use PascalCase for types and public members, `I` prefixes for interfaces, camelCase for locals and parameters, and `_camelCase` for private fields. Nullable and implicit usings are enabled. Await I/O and propagate cancellation tokens.
- Keep edits focused and preserve unrelated work. For features, update the owning contract, service, validation, mapping, controller, and DI registration as needed. Keep generated build output, logs, coverage, and test results out of source control.
- Create, edit, or delete test files only when the user explicitly requests test changes. Verify implementation changes with a build and relevant existing tests. Build before running with `--no-build`. For documentation changes, check content, paths, and solution references.
- The current test source has four architecture tests, three login tests, and one exception-middleware test. It uses mocks and an in-memory HTTP context. API startup, SQL Server connectivity, health endpoints, and transaction behavior need separate verification when relevant.
- Report commands run, warnings, failures, and checks blocked by the environment. Use imperative commit subjects consistent with the repository. PRs should explain behavior, verification, and any configuration or migration changes. Include request and response examples for API changes.

## Microsoft Learn MCP

[The project MCP configuration](.codex/config.toml) registers `microsoft-learn` at `https://learn.microsoft.com/api/mcp`. Use it for official C#, .NET, ASP.NET Core, and EF Core references. Keep the configuration with the repository. After cloning, trust the project and reopen Codex to load it. Run `codex mcp list` from the repository root to check registration.
