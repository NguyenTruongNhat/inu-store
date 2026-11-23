# Hướng dẫn dành cho agent

## 1. Tổng quan dự án

INuStore là ASP.NET Core API quản lý tài khoản, xác thực, role, permission và các quan hệ gán quyền.
Cả bảy project đều target `net10.0`; tầng lưu trữ dùng EF Core 10.0.14 với SQL Server.
Ứng dụng dùng BCrypt, JWT ký bằng HS256 và Redis để lưu phiên refresh token. Đây là bản tiếng Việt của [AGENTS.md](AGENTS.md); khi tài liệu khác với code, lấy code làm căn cứ.

## 2. Kiến trúc

Mũi tên biểu thị project reference; kiểm tra `INuStore.slnx` và `.csproj` của từng project.

```text
API ------------> Infrastructure ----------> UseCase
 |                 |    |                      |   |
 +--> UseCase      |    +--> Contract <---------+   |
                   +-------> Domain <--------------+
Domain và Contract không tham chiếu project khác trong solution.
```

- Đặt HTTP binding, authorization, filter, middleware và cấu hình ghép ứng dụng trong API.
- Đặt DTO theo phiên bản, Result và kiểu phân trang dùng chung trong Contract.
- Đặt entity, hằng số nghiệp vụ, enum và domain exception trong Domain.
- Đặt business service, validation, mapping và interface cho persistence/security/cache trong UseCase.
- Triển khai các interface đó, EF mapping, migration, JWT, băm mật khẩu và truy cập Redis trong Infrastructure.
- CẤM Domain tham chiếu Contract, UseCase, Infrastructure hoặc API.
- CẤM Contract tham chiếu Domain, UseCase, Infrastructure hoặc API.
- CẤM UseCase tham chiếu Infrastructure hoặc API; CẤM Infrastructure tham chiếu API.
- Kiểm tra các ranh giới bằng `tests/INuStore.Tests/ArchitectureTests.cs`.
- Giữ dependency EF hiện có trong Contract (`PagedResult<T>.CreateAsync`) và UseCase; không mô tả hai project này là hoàn toàn độc lập với thư viện persistence.
- Gọi service trực tiếp từ controller. `Command`/`Query` là các kiểu chứa DTO, không phải MediatR handler. Code hiện tại không có MediatR, Mapster, bộ điều phối domain event hay triển khai Specification.

## 3. Bản đồ repository

| Nội dung cần thêm/sửa | Vị trí / ví dụ |
| --- | --- |
| Entity hoặc quy tắc nghiệp vụ | `src/INuStore.Domain/Entities/Identity/Permission.cs`, `Exceptions/PermissionException.cs` |
| DTO request/response | `src/INuStore.Contract/Service/V1/Permission/{Command,Query,Response}.cs` |
| Service hoặc interface | `src/INustore.UseCase/UseCases/V1/PermissionService/PermissionService.cs`, `Abstractions/Services/IPermissionService.cs` |
| Validator | `src/INustore.UseCase/UseCases/V1/PermissionService/Validators/PermissionValidator.cs` |
| AutoMapper profile | `src/INustore.UseCase/Mappings/PermissionProfile.cs` |
| Interface repository | `src/INustore.UseCase/Abstractions/Repositories/IPermissionRepository.cs` |
| Triển khai repository | `src/INuStore.Infrastructure/Repositories/PermissionRepository.cs` |
| Cấu hình EF model | `src/INuStore.Infrastructure/Configurations/PermissionConfiguration.cs` |
| Migration | `src/INuStore.Infrastructure/Migrations/` |
| Controller | `src/INuStore.API/Controllers/PermissionController.cs` |
| Ví dụ HTTP | `src/INuStore.API/INuStore.API.http` |
| Unit test cho service | `tests/INuStore.UnitTets/UseCase/PermissionService/PermissionServiceTests.cs` |
| Test kiến trúc | `tests/INuStore.Tests/ArchitectureTests.cs` |

Giữ nguyên cách viết đường dẫn `INustore.UseCase` và `INuStore.UnitTets`; root namespace của UseCase là `INuStore.UseCase`.
Đăng ký dependency qua `DependencyInjection/Extensions/ServiceCollectionExtensions.cs` của từng tầng.

## 4. Các lệnh làm việc

Chạy PowerShell từ thư mục gốc repository. Dùng .NET SDK hỗ trợ `net10.0` và `.slnx`.
Repository chưa có `global.json`, `Directory.Packages.props` tập trung, local tool manifest, Dockerfile, Compose hay CI pipeline được đưa vào quản lý phiên bản.
Phiên bản package nằm trong từng `.csproj`. Không tự đặt ra lệnh Docker hoặc CI khi chưa có cấu hình tương ứng.

```powershell
# Restore, build rồi chạy toàn bộ test theo thứ tự
dotnet restore INuStore.slnx
dotnet build INuStore.slnx --no-restore
dotnet test INuStore.slnx --no-build

# Chạy một test project / một test
dotnet test tests/INuStore.UnitTets/INuStore.UnitTets.csproj --no-build
dotnet test tests/INuStore.Tests/INuStore.Tests.csproj --no-build --filter "FullyQualifiedName~Domain_Should_Not_Depend_On_Other_Layers"

# Chỉ kiểm tra định dạng; xem các lỗi sẵn có trước khi chạy lệnh sửa định dạng
dotnet format INuStore.slnx --verify-no-changes --no-restore

# API chạy Development; cần cấu hình SQL Server và Redis
dotnet run --project src/INuStore.API/INuStore.API.csproj --launch-profile https

# Dùng phiên bản công cụ tương ứng EF Core 10.0.14
dotnet ef --version
# Chỉ chạy khi việc cài/cập nhật công cụ đã được cho phép:
dotnet tool install --global dotnet-ef --version 10.0.14

# Chỉ tạo sau khi thay đổi schema được duyệt; thay DescribeSchemaChange bằng tên có ý nghĩa
dotnet ef migrations add DescribeSchemaChange --project src/INuStore.Infrastructure --startup-project src/INuStore.API --context ApplicationDbContext -- --environment Development
# Chỉ áp dụng sau khi xác nhận đúng database đích trong cấu hình
dotnet ef database update --project src/INuStore.Infrastructure --startup-project src/INuStore.API --context ApplicationDbContext -- --environment Development
```

- Kiểm tra kết quả thực tế của từng lệnh; cú pháp và đường dẫn đúng không chứng minh build hoặc kiểm tra runtime đã thành công.
- Cả hai launch profile đều dùng Development. HTTPS dùng `https://localhost:7005` và `http://localhost:6000`; HTTP dùng cổng 6000.
- Swagger UI ở `/swagger/index.html`; `/` chuyển hướng đến đó (`SwaggerExtensions.cs`). Dán access token nguyên bản vào Authorize.
- Development chạy `ApplyMigrationsAsync` trước khi nhận request; khởi động có thể tạo/cập nhật database được cấu hình. Xác nhận database đích trước.
- Các môi trường khác cần triển khai schema riêng. Review migration cùng snapshot; script SQL triển khai nằm trong `deployment/sql/`.

## 5. Quy ước viết code

- Theo `.editorconfig`: thụt bốn dấu cách, CRLF cho C#, file-scoped namespace, System using đứng trước và cách đặt dấu ngoặc hiện có.
- Dùng PascalCase cho type/public member, tiền tố `I` cho interface, camelCase cho tham số/biến cục bộ và `_camelCase` cho private field. Đặt tên theo nghiệp vụ, dùng biến trung gian dễ hiểu.
- Giữ nullable reference type và implicit using được bật như các `.csproj` hiện tại.
- Await thao tác I/O; truyền `CancellationToken` từ controller qua service đến repository. Không chặn luồng bằng `.Result` hoặc `.Wait()`.
- Await tuần tự mọi thao tác dùng chung DbContext, kể cả query không tracking. Không làm theo comment trái với quy tắc này trong `UserService.ReplaceUserRoles`.
- Đặt validation request trong FluentValidation validator của UseCase; gắn `[Validate<TModel>]` cho từng model route/query/body được bind. Tránh kiểm tra trực tiếp trong controller và DataAnnotations trên request.
- Giữ response lỗi binding mặc định của `[ApiController]`. `ValidationFilter.cs` trả 400 với lỗi đã loại trùng và dùng `RequestAborted`.
- Dùng Contract DTO ở biên HTTP; mapping qua AutoMapper profile. Không trả đồ thị navigation của entity, mật khẩu hoặc password hash.
- Giữ Result envelope cho response thành công. Ánh xạ thất bại sang HTTP status phù hợp; đọc `Result<T>.Value` khi thất bại sẽ ném exception (`ResultT.cs`).
- Dùng domain exception theo cách xử lý lỗi nghiệp vụ hiện có. `ExceptionHandlingMiddleware.cs` ánh xạ bad request thành 400, không tìm thấy thành 404, conflict/concurrency thành 409, thông tin đăng nhập/token không hợp lệ thành 401 và lỗi định dạng thành 422.
- Exception chưa được ánh xạ, gồm UseCase `ValidationException`, hiện trả 500. Không tự ý đổi hợp đồng lỗi. Không đưa bí mật vào thông báo exception.
- Dùng `ILogger<T>` qua DI và message template có cấu trúc. `Program.cs` cấu hình Serilog; appsettings cấu hình ghi console và file hằng ngày trong `logs/`.
- Giữ lifetime: service User/Role/Permission là scoped; Auth, repository, unit of work, service hash/token/cache là transient; permission authorization handler là singleton.
- Giữ `TransactionMiddleware` là scoped và `ExceptionHandlingMiddleware` là transient. Không inject dependency scoped vào permission handler singleton.
- Để cơ chế quét assembly tự tìm validator, kể cả kiểu internal, và AutoMapper profile trong UseCase.
- Giữ quy tắc dự án: không viết XML comment `summary`/`inheritdoc`; chỉ dùng `[ProducesResponseType]` cho HTTP 200.
- Tuân thủ cấu hình diagnostic trong `.editorconfig`. Chưa có package StyleCop/SonarAnalyzer được tham chiếu trực tiếp; tên rule trong cấu hình không chứng minh analyzer đã được cài.

## 6. Quy trình triển khai tính năng

Lấy tính năng Permission làm ví dụ cho mô hình service hiện có; chỉ sửa những tầng cần thiết.

1. Xác định hành vi yêu cầu, quyền truy cập, contract và phạm vi schema trước khi sửa.
2. Domain: thêm entity/quy tắc/exception cạnh `Entities/Identity/Permission.cs` và `Exceptions/PermissionException.cs`.
3. Contract/UseCase: định nghĩa DTO cạnh `Service/V1/Permission/Command.cs`; thêm service interface, validator, mapping và luồng nghiệp vụ theo `PermissionService.cs`.
4. Infrastructure: triển khai thao tác repository cạnh `PermissionRepository.cs`; cập nhật `ApplicationDbContext`/cấu hình entity khi cần. Chỉ ghi nhận thay đổi vào EF, không lưu ngay trong repository.
5. Sau khi thay đổi schema được duyệt, tạo migration mới bằng lệnh ở trên; review các thao tác và snapshot được sinh. Không viết lại migration cũ.
6. API: thêm binding và validation attribute theo `PermissionController.cs`; chỉ định authorization và giữ route theo phiên bản qua `ApiController.cs`.
7. Đăng ký interface qua DI extension hiện có; cập nhật ví dụ request/response trong `.http` và tài liệu liên quan.
8. Test: chạy các test hiện có liên quan. Chỉ thêm/sửa file test khi người dùng yêu cầu rõ; khi được phép, theo `PermissionServiceTests.cs`.
9. Build, chạy test, kiểm tra diff và báo giới hạn xác minh. Kiểm tra runtime/database khi hành vi thay đổi cần đến chúng và database đích đã được cho phép.

## 7. Quy tắc kiểm thử

- Dùng stack xUnit 2.9.3 và Moq 4.20.72 hiện có. Cả hai test project dùng Microsoft.NET.Test.Sdk và coverlet; `INuStore.Tests` còn dùng NetArchTest.
- Đặt kiểm tra kiến trúc trong `INuStore.Tests`; đặt test hành vi service độc lập trong `INuStore.UnitTets/UseCase/`.
- Đặt tên như `CreatePermission_WhenNameExists_ShouldThrowException`; test kiến trúc dùng dạng `Domain_Should_Not_Depend_On_Other_Layers`.
- Khi được yêu cầu sửa test, bao phủ nhánh thành công bị ảnh hưởng, lỗi nghiệp vụ liên quan và tác động mong đợi lên repository; dùng mock theo test service hiện có.
- Code hiện có bốn test `[Fact]` cho kiến trúc và bốn test `[Fact]` cho service. Chưa cấu hình Testcontainers, WebApplicationFactory hay integration-test host.
- Không khẳng định unit test đã xác minh SQL constraint, transaction middleware, startup, Redis hoặc health endpoint. Kiểm tra với dependency thực khi cần.
- Build trước khi dùng `--no-build`; chạy toàn bộ test solution trước khi hoàn tất thay đổi triển khai. Với tài liệu, kiểm tra đường dẫn, lệnh và đối chiếu code; báo kết quả build/test hoặc nguyên nhân bị chặn.

## 8. Ranh giới thao tác

**Luôn thực hiện**

- Đọc code chịu trách nhiệm, tuân thủ ranh giới tầng và mô hình service hiện có, giữ thay đổi không liên quan và giới hạn diff.
- Chạy build và test hiện có khi thay đổi triển khai; báo đầy đủ lỗi và cảnh báo.
- Giữ thao tác ghi trong ranh giới lưu dữ liệu hiện có và validate mọi request model mới.

**Hỏi trước**

- Xin phép trước khi thêm NuGet package, đổi public API contract, đổi schema hoặc quy tắc kiến trúc, trừ khi yêu cầu hiện tại đã cho phép thay đổi đó.
- Xác nhận database đích trước khi áp dụng migration hoặc chạy Development với connection chưa quen thuộc.
- Chỉ tạo, sửa hoặc xóa file test khi người dùng cho phép rõ ràng.

**Không bao giờ**

- Commit bí mật, bỏ qua ranh giới tầng, tắt analyzer, mở rộng audit suppression hoặc force-push.
- Sửa tay output được sinh (`bin/`, `obj/`, migration designer/snapshot) hay migration cũ; để EF sinh các file migration mới.
- Sửa `.agents/`, `.codex/`, `.aws/`, cấu hình không liên quan, script triển khai hoặc migration lịch sử như một phần dọn dẹp ngoài phạm vi.
- Dùng ví dụ lỗi thời trong `technical-docs/` hoặc `AGENTS-VN.md` làm bằng chứng cho hành vi hiện tại.

## 9. Bảo mật và cấu hình

- Cấp SQL qua `ConnectionStrings__ConnectionStrings`, Redis qua `ConnectionStrings__Redis`, JWT qua `JwtOption__SecretKey`, `JwtOption__Issuer`, `JwtOption__Audience`, `JwtOption__ExpireMin`.
- Không đưa credential, key, token hay hash thật vào appsettings được commit, ví dụ, log hoặc output của agent. Chưa cấu hình `UserSecretsId`; xin phép trước khi thêm cấu hình project cho user-secrets.
- Giữ BCrypt qua `IPasswordHasher` và sinh HS256 qua `IJwtTokenService`. AuthController có các action anonymous Register/Login/RefreshToken/RevokeToken kèm validation.
- Đọc authorization metadata của từng action: PermissionController chỉ dành cho Admin; RoleController có các yêu cầu role khác nhau; UserController kết hợp permission, Gmail policy, yêu cầu đăng nhập và action chưa bảo vệ. Không có fallback policy bảo vệ action thiếu attribute.
- JWT validation hiện kiểm tra thời hạn/chữ ký, clock skew bằng 0 nhưng tắt kiểm tra issuer/audience (`JwtExtensions.cs`). Review rõ ràng các thay đổi bảo mật.
- Permission handler so khớp chính xác claim và chấp nhận khi có bất kỳ permission nào được yêu cầu (`PermissionAuthorizationHandler.cs`). Giữ tên đồng bộ với `Domain/Constants/Permissions.cs`.
- Phiên refresh được cache theo username, hết hạn sau năm phút. Revoke xóa trạng thái Redis nhưng không đưa access JWT đã phát hành vào blacklist. Refresh giữ authorization claim từ JWT được gửi lên.
- Infrastructure đang bật EF sensitive-data logging và detailed errors. Review trước khi triển khai production; không để lộ nội dung nhạy cảm qua exception/log.
- Giữ suppression giới hạn cho advisory AutoMapper trong `Directory.Build.props`; giữ các cảnh báo dependency/bảo mật khác.
- Khi cần tài liệu .NET chính thức, dùng Microsoft Learn MCP đã cấu hình trong `.codex/config.toml`; giữ cấu hình công cụ trong phạm vi repository này.

## 10. Điều kiện hoàn tất

- [ ] Chỉ triển khai hành vi được cho phép; giữ ranh giới dependency và các thay đổi không liên quan.
- [ ] Build thành công, các test liên quan và toàn bộ test solution đều đạt đối với thay đổi triển khai.
- [ ] Không thêm cảnh báo mới; báo chính xác cảnh báo có sẵn, lỗi và kiểm tra bị môi trường chặn.
- [ ] Sinh/review migration mới cùng snapshot nếu thay đổi schema đã duyệt cần đến; xác nhận database đích trước khi áp dụng.
- [ ] Cập nhật ví dụ `.http` và tài liệu liên quan khi đổi endpoint/cấu hình.
- [ ] Kiểm tra diff để loại bí mật, output build, log và thay đổi định dạng ngoài ý muốn.
- [ ] Báo các lệnh đã chạy; phân biệt đối chiếu source, kiểm tra bằng unit test và kiểm tra runtime.

## 11. Các điểm dễ mắc lỗi

- `Program.cs` đặt exception handling trước authorization và transaction middleware; Development cũng đăng ký/map OpenAPI. Không lặp lại thông tin thiếu `AddOpenApi` của hướng dẫn cũ.
- `AddDbContextPool<DbContext, ApplicationDbContext>` đăng ký context dùng chung. Repository, transaction middleware và EFUnitOfWork nhận concrete context; kiểm tra DI resolution khi kiểm tra startup, không coi build thành công là bằng chứng khởi tạo được các dependency.
- Transaction middleware mở SQL transaction cho POST/PUT/PATCH/DELETE, kể cả login/refresh/revoke, trước MVC binding/validation. Middleware giữ response trong bộ đệm, rollback khi HTTP >=400; trường hợp khác sẽ save và commit trước khi chép response ra ngoài.
- Result thất bại nhưng trả HTTP 200 vẫn dẫn đến commit. Để middleware sở hữu việc save/commit; caller ngoài HTTP cần ranh giới lưu dữ liệu rõ ràng.
- `EFUnitOfWork.cs` hiện gọi SaveChangesAsync trực tiếp; không có SQL exception translator. Không dựa vào tài liệu dịch lỗi cũ để suy ra response cho lỗi trùng dữ liệu/khóa ngoại.
- `RepositoryBase.FindAll` không tracking; `FindByIdAsync` và `FindSingleAsync` bật tracking. Lazy-loading proxy bị tắt; chủ động tải navigation cần dùng.
- `UserService` vẫn tải toàn bộ user dù nhận tham số phân trang; validation lấy theo ID yêu cầu ID >7. Update/delete user không tồn tại vẫn thành công; delete chưa truyền cancellation token vào lookup. Xác nhận ý định trước khi đổi hành vi.
- `ReplaceUserRoles` bỏ qua ID không tồn tại khi thêm, nhưng xóa role dựa trên danh sách gửi lên; có thể thay một phần hoặc xóa hết assignment mà vẫn thành công. Các yêu cầu thay thế đồng thời chưa được tuần tự hóa.
- `AuthService` truy vấn permission claim hai lần khi login. Thao tác cache refresh/login/revoke chưa nguyên tử; TODO trong source đề cập Redis transaction/Lua. Không khẳng định xoay phiên đồng thời đã an toàn.
- Xóa cache theo prefix chỉ theo dõi key trong static dictionary của từng process (`Caching/CacheService.cs`); không liệt kê được toàn bộ key giữa nhiều instance.
- Readiness chỉ kiểm tra kết nối SQL với timeout năm giây; liveness loại dependency (`HealthCheckExtensions.cs`). Readiness không kiểm tra Redis hoặc độ tương thích schema.
