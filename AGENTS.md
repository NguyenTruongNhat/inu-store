# Agent guide

## 1. Project overview

INuStore is an ASP.NET Core API for accounts, authentication, roles, permissions, and assignments.
All seven projects target `net10.0`; persistence uses EF Core 10.0.14 with SQL Server.
Authentication uses BCrypt and HS256 JWTs; Redis stores refresh sessions. Use the source as authority over `technical-docs/` and the Vietnamese companion `AGENTS-VN.md`.

## 2. Architecture

Arrows mean project references; see `INuStore.slnx` and each project's `.csproj`.

```text
API ------------> Infrastructure ----------> UseCase
 |                 |    |                      |   |
 +--> UseCase      |    +--> Contract <---------+   |
                   +-------> Domain <--------------+
Domain and Contract have no solution project references.
```

- Keep HTTP binding, authorization, filters, middleware, and composition in API.
- Keep versioned DTOs and shared Result/pagination types in Contract.
- Keep entities, domain constants, enums, and domain exceptions in Domain.
- Keep business services, validation, mappings, and persistence/security/cache interfaces in UseCase.
- Implement those interfaces, EF mappings, migrations, JWTs, hashing, and Redis access in Infrastructure.
- FORBID Domain references to Contract, UseCase, Infrastructure, or API.
- FORBID Contract references to Domain, UseCase, Infrastructure, or API.
- FORBID UseCase references to Infrastructure or API; FORBID Infrastructure references to API.
- Enforce these boundaries through `tests/INuStore.Tests/ArchitectureTests.cs`.
- Preserve existing EF dependencies in Contract (`PagedResult<T>.CreateAsync`) and UseCase; do not claim these projects are persistence-library-free.
- Use direct service calls from controllers. `Command`/`Query` are DTO containers, not MediatR handlers. No MediatR, Mapster, domain-event dispatcher, or Specification implementation exists in current source.

## 3. Repository map

| Add or change | Location / example |
| --- | --- |
| Entity or domain rule | `src/INuStore.Domain/Entities/Identity/Permission.cs`, `Exceptions/PermissionException.cs` |
| Request/response DTO | `src/INuStore.Contract/Service/V1/Permission/{Command,Query,Response}.cs` |
| Service or interface | `src/INustore.UseCase/UseCases/V1/PermissionService/PermissionService.cs`, `Abstractions/Services/IPermissionService.cs` |
| Validator | `src/INustore.UseCase/UseCases/V1/PermissionService/Validators/PermissionValidator.cs` |
| AutoMapper profile | `src/INustore.UseCase/Mappings/PermissionProfile.cs` |
| Repository interface | `src/INustore.UseCase/Abstractions/Repositories/IPermissionRepository.cs` |
| Repository implementation | `src/INuStore.Infrastructure/Repositories/PermissionRepository.cs` |
| EF model configuration | `src/INuStore.Infrastructure/Configurations/PermissionConfiguration.cs` |
| Migration | `src/INuStore.Infrastructure/Migrations/` |
| Controller | `src/INuStore.API/Controllers/PermissionController.cs` |
| HTTP examples | `src/INuStore.API/INuStore.API.http` |
| Service unit tests | `tests/INuStore.UnitTets/UseCase/PermissionService/PermissionServiceTests.cs` |
| Architecture tests | `tests/INuStore.Tests/ArchitectureTests.cs` |

Preserve the disk spellings `INustore.UseCase` and `INuStore.UnitTets`; UseCase's root namespace is `INuStore.UseCase`.
Register changes through each layer's `DependencyInjection/Extensions/ServiceCollectionExtensions.cs`.

## 4. Commands

Run PowerShell from the repository root. Use a .NET SDK supporting `net10.0` and `.slnx`.
No `global.json`, central `Directory.Packages.props`, local tool manifest, Dockerfile, Compose configuration, or CI pipeline is checked in.
Package versions live in individual `.csproj` files. Do not invent Docker or CI commands.

```powershell
# Restore, build, and run all tests in order
dotnet restore INuStore.slnx
dotnet build INuStore.slnx --no-restore
dotnet test INuStore.slnx --no-build

# One test project / one test
dotnet test tests/INuStore.UnitTets/INuStore.UnitTets.csproj --no-build
dotnet test tests/INuStore.Tests/INuStore.Tests.csproj --no-build --filter "FullyQualifiedName~Domain_Should_Not_Depend_On_Other_Layers"

# Read-only formatting check; review baseline findings before any formatting write
dotnet format INuStore.slnx --verify-no-changes --no-restore

# API: Development; requires configured SQL Server and Redis
dotnet run --project src/INuStore.API/INuStore.API.csproj --launch-profile https

# EF tooling: match the installed tool to EF Core 10.0.14
dotnet ef --version
# Run only when installing/updating the tool is authorized:
dotnet tool install --global dotnet-ef --version 10.0.14

# Generate only after schema approval; replace DescribeSchemaChange with a meaningful name
dotnet ef migrations add DescribeSchemaChange --project src/INuStore.Infrastructure --startup-project src/INuStore.API --context ApplicationDbContext -- --environment Development
# Apply only after confirming the configured target database
dotnet ef database update --project src/INuStore.Infrastructure --startup-project src/INuStore.API --context ApplicationDbContext -- --environment Development
```

- Verify command outcomes locally; command syntax and paths do not prove a successful build or runtime check.
- Both launch profiles use Development. HTTPS binds `https://localhost:7005` and `http://localhost:6000`; HTTP binds port 6000.
- Swagger UI is `/swagger/index.html`; `/` redirects there (`SwaggerExtensions.cs`). Paste the raw access token into Authorize.
- Development startup runs `ApplyMigrationsAsync` before serving requests; launching can create/update the configured database. Confirm the target first.
- Other environments require explicit schema deployment. Review generated migrations and snapshot together; deployment SQL lives in `deployment/sql/`.

## 5. Coding conventions

- When writing C# code, keep method signatures, argument lists, and expressions on one line if they fit within 120 characters, including indentation. When a line exceeds 120 characters, wrap it at a natural break to keep the code easy to read. Follow the existing brace layout.
- Follow `.editorconfig`: four spaces, CRLF for C#, file-scoped namespaces, System usings first, and existing brace layout.
- Use PascalCase types/public members, `I`-prefixed interfaces, camelCase parameters/locals, and `_camelCase` private fields. Prefer descriptive domain names and readable intermediate values.
- Keep nullable reference types and implicit usings enabled, as in every `.csproj`.
- Await I/O; propagate `CancellationToken` from the controller through services and repositories. Never block with `.Result` or `.Wait()`.
- Await all operations sharing a DbContext sequentially, including no-tracking queries. Do not copy the contrary comment in `UserService.ReplaceUserRoles`.
- Keep request validation in UseCase FluentValidation validators; apply `[Validate<TModel>]` for each bound route/query/body model. Avoid controller checks and request DataAnnotations.
- Preserve MVC's default `[ApiController]` binding-error response. `ValidationFilter.cs` returns 400 with deduplicated errors and uses `RequestAborted`.
- Use Contract DTOs at the HTTP boundary; map through AutoMapper profiles. Never return entity navigation graphs, passwords, or password hashes.
- Preserve existing success `Result` envelopes. Map a failure to an appropriate HTTP status; failed `Result<T>.Value` throws on access (`ResultT.cs`).
- Use domain exceptions for existing business-rule failure patterns. `ExceptionHandlingMiddleware.cs` maps bad requests to 400, not found to 404, conflicts/concurrency to 409, invalid credentials/tokens to 401, and format errors to 422.
- Treat unmapped exceptions, including UseCase `ValidationException`, as current 500 behavior. Do not silently change error contracts. Keep secrets out of exception messages.
- Use injected `ILogger<T>` and structured message templates. `Program.cs` configures Serilog; appsettings configure console and daily file sinks under `logs/`.
- Preserve registration lifetimes: User/Role/Permission services scoped; Auth, repositories, unit of work, hash/token/cache services transient; permission authorization handler singleton.
- Keep `TransactionMiddleware` scoped and `ExceptionHandlingMiddleware` transient. Do not inject scoped dependencies into the singleton permission handler.
- Let assembly scanning discover validators (including internal types) and AutoMapper profiles in UseCase.
- Preserve the existing project rules: omit XML `summary`/`inheritdoc` comments; use `[ProducesResponseType]` only for HTTP 200.
- Respect `.editorconfig` diagnostic settings. No StyleCop/SonarAnalyzer package is directly referenced; configured rule names do not prove an analyzer is installed.

## 6. How to implement a feature

Follow the Permission feature as a concrete service-based example; change only layers required by the task.

1. Confirm requested behavior, authorization, contracts, and schema scope before editing.
2. Domain: add the entity/rule/exception beside `Entities/Identity/Permission.cs` and `Exceptions/PermissionException.cs`.
3. Contract/UseCase: define DTOs beside `Service/V1/Permission/Command.cs`; add the service abstraction, validator, mapping, and business flow following `PermissionService.cs`.
4. Infrastructure: implement repository operations beside `PermissionRepository.cs`; update `ApplicationDbContext`/entity configurations when needed. Stage writes without saving inside repositories. For every new concrete repository, expose its interface through a read-only property on `IUnitOfWork`, inject it into `EFUnitOfWork`, and register it in Infrastructure DI.
5. After schema approval, generate a new migration with the command above; review its operations and generated model snapshot. Never rewrite an existing migration.
6. API: add binding and validation attributes following `PermissionController.cs`; select explicit authorization and preserve versioned routing through `ApiController.cs`.
7. Wire interfaces through existing DI extensions; update `.http` request/response examples and relevant docs.
8. Tests: run relevant existing tests. Add/change test files only when the user explicitly requests test changes; follow `PermissionServiceTests.cs` when authorized.
9. Build, run tests, inspect the diff, and report verification limits. Perform runtime/database checks when the changed behavior requires them and the target is authorized.

## 7. Testing rules

- Use the existing xUnit 2.9.3 and Moq 4.20.72 stack. Both test projects use Microsoft.NET.Test.Sdk and coverlet; `INuStore.Tests` also uses NetArchTest.
- Keep architecture assertions in `INuStore.Tests`; keep isolated service behavior tests in `INuStore.UnitTets/UseCase/`.
- Follow service-test names such as `CreatePermission_WhenNameExists_ShouldThrowException`; architecture tests use `Domain_Should_Not_Depend_On_Other_Layers`.
- When test changes are explicitly requested, cover the changed success path, relevant business failures, and expected repository effects; use mocks as the existing service tests do.
- Current source contains four architecture facts and four service facts. No Testcontainers, WebApplicationFactory, or integration-test host is configured.
- Do not claim unit tests verify SQL constraints, transaction middleware, startup, Redis, or health endpoints. Verify those against configured dependencies when relevant.
- Build before using `--no-build`; run the full solution suite before finishing implementation. For docs, validate paths, commands, and source claims; report build/test outcomes or blockers.

## 8. Boundaries

**Always**
- Read owning source, follow the layer rules and existing service pattern, preserve unrelated work, and keep diffs focused.
- Run build and existing tests for implementation changes; report failures and warnings without hiding them.
- Keep writes within the established persistence boundary and validate every new request model.

**Ask first**
- Obtain approval before adding NuGet packages, changing public API contracts, altering schema, or changing architecture rules, unless the current request already authorizes that change.
- Confirm the database target before applying migrations or starting Development against an unfamiliar connection.
- Obtain explicit authorization before creating, editing, or deleting test files.

**Never**
- Commit secrets, bypass layers, disable analyzers, broaden audit suppressions, or force-push.
- Hand-edit generated output (`bin/`, `obj/`, migration designers/snapshot) or existing migrations; let EF generate new migration artifacts.
- Modify `.agents/`, `.codex/`, `.aws/`, unrelated configuration, deployment scripts, or historical migrations as incidental cleanup.
- Treat stale `technical-docs/` examples or `AGENTS-VN.md` as proof of current behavior.

## 9. Security and config

- Supply SQL through `ConnectionStrings__ConnectionStrings`, Redis through `ConnectionStrings__Redis`, and JWT values through `JwtOption__SecretKey`, `JwtOption__Issuer`, `JwtOption__Audience`, `JwtOption__ExpireMin`.
- Keep real credentials, keys, tokens, and hashes out of committed appsettings, examples, logs, and agent output. No `UserSecretsId` is configured; obtain approval before adding project configuration for user-secrets.
- Preserve BCrypt via `IPasswordHasher` and HS256 generation via `IJwtTokenService`. AuthController exposes anonymous Register/Login/RefreshToken/RevokeToken actions with validation.
- Read each action's actual authorization metadata: PermissionController is Admin-only; RoleController mixes role requirements; UserController mixes permission, Gmail policy, authenticated, and unprotected actions. No fallback policy protects unannotated actions.
- JWT validation currently checks lifetime/signature with zero clock skew but disables issuer/audience validation (`JwtExtensions.cs`). Review security changes explicitly.
- Permission checks use exact permission claims and match any requested permission (`PermissionAuthorizationHandler.cs`). Keep names aligned with `Domain/Constants/Permissions.cs`.
- Refresh sessions are cached by username with five-minute expiry. Revoking a session removes Redis state; it does not blacklist issued access JWTs. Refresh preserves authorization claims from the supplied JWT.
- EF sensitive-data logging and detailed errors are enabled in Infrastructure. Review these settings before production deployment; do not expose sensitive exception/log content.
- Preserve the narrow AutoMapper advisory suppression in `Directory.Build.props`; retain other dependency/security warnings.
- Use the configured Microsoft Learn MCP in `.codex/config.toml` for official .NET references when needed; keep project tool configuration scoped to this repository.

## 10. Definition of done

- [ ] Implement only authorized behavior and preserve dependency boundaries and unrelated changes.
- [ ] Build successfully and pass relevant existing tests plus the solution suite for implementation changes.
- [ ] Introduce no new warnings; report baseline warnings, failures, and environment-blocked checks accurately.
- [ ] Generate/review a new migration and snapshot if an approved schema change requires it; confirm the target before applying.
- [ ] Update `.http` examples and relevant docs for endpoint/configuration changes.
- [ ] Inspect the diff for secrets, generated build output, logs, and unintended formatting changes.
- [ ] Report commands run and distinguish source inspection, unit verification, and runtime verification.

## 11. Common pitfalls

- `Program.cs` registers exception handling before authorization and transaction middleware; Development also registers/maps OpenAPI. Do not repeat the old guide's missing-`AddOpenApi` claim.
- `AddDbContextPool<DbContext, ApplicationDbContext>` registers the shared context. Repositories, transaction middleware, and EFUnitOfWork request the concrete context; verify DI resolution during startup checks instead of assuming a build proves activation.
- Transaction middleware starts SQL transactions for POST/PUT/PATCH/DELETE, including login/refresh/revoke, before MVC binding/validation. It buffers output, rolls back for HTTP >=400, otherwise saves and commits before copying output.
- A failed Result returned with HTTP 200 still commits. Keep saving/committing owned by the middleware; non-HTTP callers need an explicit persistence boundary.
- `EFUnitOfWork.cs` currently calls SaveChangesAsync directly; no SQL exception translator exists. Do not rely on stale translation documentation for duplicate/FK error responses.
- `RepositoryBase.FindAll` is untracked; `FindByIdAsync` and `FindSingleAsync` enable tracking. Lazy-loading proxies are disabled; load required navigation data explicitly.
- `UserService` still loads all users despite paging inputs; get-by-ID validation requires ID >7. Missing-user update/delete return success; delete omits its cancellation token on lookup. Confirm intent before changing these behaviors.
- `ReplaceUserRoles` skips unknown IDs on additions but removes roles against the submitted list; it can partially replace/clear assignments successfully. Concurrent replacements are not serialized.
- `AuthService` queries permission claims twice during login. Refresh/login/revoke cache operations are not atomic; source TODOs call for Redis transactions/Lua. Do not claim concurrent session rotation is safe.
- Cache prefix removal tracks keys only in a process-local static dictionary (`Caching/CacheService.cs`); it cannot enumerate all keys across instances.
- Readiness checks SQL connectivity only, with a five-second timeout; liveness excludes dependencies (`HealthCheckExtensions.cs`). Readiness does not verify Redis or schema compatibility.
