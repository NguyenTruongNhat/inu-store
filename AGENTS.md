# Repository Guidelines

This guide applies to the whole repository. INuStore is a .NET 10 ASP.NET Core API with user CRUD, EF Core SQL Server persistence, and no frontend. Check the owning source before changing behavior; the notes in `Docs/` include historical examples.

## Project Map

`INuStore.slnx` contains five source projects and one test project:

| Location | Responsibility |
| --- | --- |
| `src/INuStore.API/` | Controllers, validation filters, middleware, versioned Swagger, and composition in `Program.cs`. |
| `src/INuStore.Contract/` | Nested `Command`, `Query`, and `Response` records in `Service/V1/User/`; `Result`, validation results, errors, and pagination in `Abstractions/Share/`. |
| `src/INuStore.Domain/` | `DomainEntity<T>`, `Entities/Identity/User.cs`, and domain exceptions. |
| `src/INustore.UseCase/` | Interfaces in `Abstractions/`, services and validators in `UseCases/V1/`, AutoMapper profiles in `Mappings/`, and DI extensions. Preserve the existing `INustore` spelling in paths and namespaces. |
| `src/INuStore.Infrastructure/` | `ApplicationDbContext`, repositories, `EFUnitOfWork`, entity configurations, SQL options, and migrations. |
| `tests/INuStore.Tests/` | xUnit login/middleware behavior tests and NetArchTest architecture tests. The solution folder is misspelled `/tets/`; the disk path is `tests/`. |
| `Docs/` | Vietnamese technical notes. `custom-middleware.MD` is currently empty. |

## Architecture and Dependency Injection

Keep these solution-layer boundaries, enforced by `ArchitectureTests.cs`:

- Domain and Contract must not depend on other solution layers.
- UseCase may depend on Domain and Contract, but not Infrastructure or API.
- Infrastructure may depend on Domain, Contract, and UseCase, but not API.
- API references UseCase and Infrastructure and composes their registrations.

Contract currently references EF Core for `PagedResult<T>.CreateAsync`; layer independence does not mean the project has no external dependencies. Keep repository and unit-of-work interfaces in UseCase, persistence implementations in Infrastructure, and HTTP concerns in API. Controllers call services directly; there is no mediator pipeline.

Use the existing DI extension methods. `AddAppServices` explicitly registers `IUserService` as scoped; `AddAppValidators` and `AddAppMappings` scan the UseCase assembly. `AddRepositoryBaseConfiguration` registers unit of work and repositories as transient. `AddSqlConfiguration` uses `AddDbContextPool<DbContext, ApplicationDbContext>`. Keep middleware and repositories using the request's shared context; `TransactionMiddleware` is scoped and `ExceptionHandlingMiddleware` is transient.

## Request, Validation, and Persistence Flow

- `ApiController` supplies `api/v{version:apiVersion}/[controller]`. `UserController` declares version `1`; actions use routes such as `/api/v1/User/GetUserById`. GET models bind from query strings; POST, PUT, and DELETE commands bind from JSON bodies. Maintain examples in `src/INuStore.API/INuStore.API.http` when endpoints change.
- `[Validate<TModel>]` attaches `ValidationFilter<TModel>` to an action. It validates the bound action argument asynchronously and returns HTTP 400 with a non-generic `ValidationResult` on failure. Registering a validator alone does not attach the filter. MVC model-binding errors can use the separate default `[ApiController]` response.
- `ExceptionHandlingMiddleware` wraps the transaction/controller flow. It returns `{ title, status, detail, errors }`: `InvalidCredentialsException` maps to 401, `BadRequestException` to 400, `NotFoundException` to 404, `FormatException` to 422, and other exceptions to 500. The UseCase `ValidationException` currently falls through to 500; its special status mapping is commented out.
- `TransactionMiddleware` wraps POST, PUT, PATCH, and DELETE in a database transaction before MVC validation runs. It buffers the response, rolls back for HTTP status >= 400, and otherwise awaits `SaveChangesAsync` and commit before copying the response. Exceptions leave the transaction uncommitted and reach the outer exception middleware. GET requests skip the transaction.
- Services currently stage writes through repositories and rely on middleware to save. Avoid adding extra save/transaction ownership without accounting for that flow. Calls outside HTTP middleware need an explicit persistence boundary. `FindAll` returns an `AsNoTracking` query; `FindByIdAsync` and `FindSingleAsync` opt into tracking.
- SQL retry options are bound and validated, but the retrying execution strategy is commented out. If enabling retries, review how explicit transactions are executed together with retries.

## Current Behavior to Check Before Extending

Recheck these existing behaviors when relevant to the requested work, and update this section when they change:

- Swagger declares a shared HTTP Bearer security scheme and requirement. Use `Authorize` with the raw access token; Swagger adds the `Bearer` prefix. Actual endpoint access still follows the existing `[Authorize]` attributes.
- `LoginAsync` throws the UseCase `InvalidCredentialsException` for either a missing user or an invalid password. Both return HTTP 401 with title `Authentication Failure` and detail `Invalid username or password.`; no token is generated on either failure.
- `CreateUser` also adds a hard-coded second user. `UserProfile` copies `Password` directly into `PasswordHash`; hashing is not implemented. Never expose passwords or hashes in response DTOs.
- `GetAllUsers` ignores `PageIndex` and `PageSize` and maps the entire query to a list. The existence of `PagedResult<T>` does not mean this endpoint paginates.
- `GetUserByIdValidator` requires `UserId > 7`; update validation only checks a positive ID, and create validation checks username/email. Get-all and delete actions have no custom validation filter.
- Get-by-ID throws `UserNotFoundException` for a missing user. Update and delete silently succeed when no user exists. Delete currently omits the cancellation token on its repository lookup.
- All controller actions use `Ok(result)`. A failed `Result` alone does not cause transaction rollback. Do not serialize failed `Result<T>` objects directly: their `Value` getter throws. Account for HTTP status, serialization, and transaction behavior when introducing failure results.
- `Program.cs` calls `MapOpenApi` in Development while `AddOpenApi` is commented out. Verify startup when changing API documentation setup. `UseAuthorization` is present, but authentication is not configured.

## Build, Run, and Database Commands

Run from the repository root with the .NET 10 SDK; there is no `global.json` pin:

```powershell
dotnet restore INuStore.slnx
dotnet build INuStore.slnx --no-restore
dotnet test INuStore.slnx --no-build
dotnet run --project src/INuStore.API --launch-profile https
```

The HTTPS profile declares `https://localhost:7005` and `http://localhost:5157`; both launch profiles use Development. Swagger is configured at `/swagger/index.html`, and `/` redirects there. Coverage is optional:

```powershell
dotnet test tests/INuStore.Tests --collect:"XPlat Code Coverage"
```

For requested schema changes, use a `dotnet-ef` tool matching the EF Core 10 packages and target Infrastructure with API as the startup project. Replace `DescribeSchemaChange` with a descriptive migration name:

```powershell
dotnet ef migrations add DescribeSchemaChange --project src/INuStore.Infrastructure --startup-project src/INuStore.API --context ApplicationDbContext
dotnet ef database update --project src/INuStore.Infrastructure --startup-project src/INuStore.API --context ApplicationDbContext
```

There is no repository-local tool manifest. Check `dotnet ef --version` before generating migrations. Review generated migrations and the model snapshot together; apply database updates only to the intended configured database. The API does not apply migrations at startup.

## Configuration and Security

- Configure an accessible local SQL Server/LocalDB instance. The connection key is `ConnectionStrings:ConnectionStrings`; override it with `ConnectionStrings__ConnectionStrings`. The development settings contain a local example; keep real credentials out of source, logs, and examples.
- `SqlServerRetryOptions` is validated on startup; retain valid settings when changing configuration.
- EF detailed errors and sensitive-data logging are currently enabled in Infrastructure. Consider this when handling logs or preparing non-development configuration.
- Serilog writes to console and daily files under `logs/`. Keep `bin/`, `obj/`, `.vs/`, logs, coverage, and test results untracked.

## Editing, Testing, and Delivery

Use four-space indentation, braces on separate lines, and the surrounding file's namespace style. Use PascalCase for types/public members, `I`-prefixed interfaces, camelCase locals/parameters, and `_camelCase` fields. Nullable and implicit usings are enabled. Await asynchronous I/O and pass cancellation tokens through new or changed operations. No repository-specific formatter or linter is configured.

For a feature, update the versioned contracts, service interface/implementation, validators, mappings, controller, and DI registrations as needed. Put persistence/model changes in Infrastructure and Domain as appropriate. Keep edits focused and preserve unrelated user work.

The existing suite contains four architecture tests, three `AuthService` login tests, one exception-middleware test, and one Swagger Bearer configuration test. The behavior tests use mocks and an in-memory HTTP context; they do not verify API startup, SQL Server, or transaction behavior. Add xUnit `[Fact]` behavior tests for service/API changes and architecture tests for new boundaries. Check success, invalid input, missing records, and persistence/failure behavior relevant to the change. No coverage percentage is enforced. Documentation-only edits need content/path checks rather than new behavior tests.

Build before using `--no-build` tests. Report commands run, warnings/failures, and any checks blocked by the environment. Compilation and architecture tests do not establish working API startup or SQL Server behavior. Do not suppress dependency/security warnings to make validation look clean.

Follow recent imperative commit subjects, such as `Add architecture tests to enforce layer boundaries`; Conventional Commit prefixes are not established. PRs should explain behavior changes, link relevant issues, report validation, identify configuration/migration changes, and include request/response examples for API changes.
