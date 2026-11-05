**Đăng ký DI ở layer sở hữu implementation; inject qua constructor ở lớp cần sử dụng dependency đó.** Đây là hai việc khác nhau.

Ví dụ trong project của bạn:

| Interface → Implementation | Đăng ký ở đâu? | Lý do |
|---|---|---|
| `IUserService → UserService` | UseCase | Điều phối nghiệp vụ user |
| `IAuthService → AuthService` | UseCase | Điều phối nghiệp vụ login |
| `IUserRepository → UserRepository` | Infrastructure | Truy cập database bằng EF Core |
| `IUnitOfWork → EFUnitOfWork` | Infrastructure | Làm việc với DbContext |
| `IPasswordHasher → BCryptPasswordHasher` | Infrastructure | Thực hiện hash bằng BCrypt |
| `IJwtTokenService → JwtTokenService` | Infrastructure | Tạo, ký và xử lý JWT |
| Middleware, controllers, Swagger | API | Xử lý HTTP |

Trong [DI của UseCase](/C:/NhatNguyen/my-project/INuStore/src/INustore.UseCase/DependencyInjection/Extensions/ServiceCollectionExtensions.cs), bạn đang đăng ký:

```csharp
services.AddScoped<IUserService, UserService>();
services.AddTransient<IAuthService, AuthService>();
```

Trong [DI của Infrastructure](/C:/NhatNguyen/my-project/INuStore/src/INuStore.Infrastructure/DependencyInjection/Extensions/ServiceCollectionExtensions.cs):

```csharp
services.AddTransient<IPasswordHasher, BCryptPasswordHasher>();
services.AddTransient<IJwtTokenService, JwtTokenService>();
```

Các đăng ký này đang nằm đúng layer.

**Vậy tại sao `AuthService` trong UseCase vẫn inject `IJwtTokenService`?**

Vì `AuthService` cần chức năng tạo token để hoàn thành login:

```csharp
public AuthService(
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
{
    _jwtTokenService = jwtTokenService;
    _unitOfWork = unitOfWork;
    _passwordHasher = passwordHasher;
}
```

UseCase chỉ biết các interface. Khi ứng dụng chạy, DI container dựa vào đăng ký trong Infrastructure để cung cấp `JwtTokenService`, `EFUnitOfWork` và `BCryptPasswordHasher`.

**Các project cùng đóng góp vào một DI container do API tạo:**

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddUseCase();
builder.Services.AddApi();
```

Vì thế, dependency được đăng ký từ Infrastructure vẫn có thể được inject vào service trong UseCase. UseCase không cần tham chiếu Infrastructure.

Khi thêm một dependency mới, hãy xác định hai điều:

1. **Implementation thuộc trách nhiệm layer nào?** Đặt implementation và đăng ký DI tại layer đó.
2. **Lớp nào cần dùng chức năng này?** Inject interface vào constructor của lớp đó.

`Scoped`, `Transient`, `Singleton` quyết định vòng đời object; đó là lựa chọn riêng với việc đặt đăng ký ở layer nào.