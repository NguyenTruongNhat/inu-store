# INuStore — .NET Backend Practice Project

Mục tiêu: Xây dựng một hệ thống Identity & User Management tương đối hoàn chỉnh, giúp bạn ôn lại kiến thức .NET từ cơ bản đến nâng cao, đồng thời thực hành những vấn đề thường gặp trong môi trường production.

Mình đề xuất phát triển INuStore theo hướng Modular Monolith, sử dụng ASP.NET Core Web API, EF Core, SQL Server và Clean Architecture.

Thay vì tạo các chức năng riêng lẻ chỉ để học công nghệ, chúng ta sẽ xây dựng những tính năng có liên kết nghiệp vụ với nhau. Ví dụ: người dùng đăng ký tài khoản → xác thực email → đăng nhập → được phân quyền → thực hiện thao tác → hệ thống ghi audit log → gửi notification thông qua queue.

## Tổng quan các phase

### Lộ trình đề xuất — 14 phase

| Phase | Chủ đề | Nội dung chính |
| :---: | --- | --- |
| **01** | Project Foundation | Kiến trúc, DI, Middleware |
| **02** | User Management | CRUD, EF Core, Pagination |
| **03** | Authentication | JWT, Refresh Token, Security |
| **04** | Authorization | Role, Permission, Policy |
| **05** | Email & Notification | SMTP, Template, OTP |
| **06** | File Management | Upload, Storage, Validation |
| **07** | Excel & PDF | Import, Export, Report |
| **08** | Caching | Redis, Invalidation, Concurrency |
| **09** | Message Queue | RabbitMQ, Consumer, Retry |
| **10** | Background Jobs | Hosted Service, Scheduling |
| **11** | Audit & Observability | Logging, Tracing, Metrics |
| **12** | Advanced Data & Reliability | Transaction, Outbox, Concurrency |
| **13** | Testing & Security | Unit, Integration, Hardening |
| **14** | DevOps & Deployment | Docker, CI/CD, Production |

## Requirements theo từng phase

Dưới đây là checklist ôn tập các vấn đề kỹ thuật qua chức năng thực tế, chưa đi vào API endpoint, database schema hay implementation. Với mỗi cơ chế kỹ thuật, ưu tiên một tình huống đại diện để thực hành; chỉ tách thêm mục khi có vấn đề kỹ thuật khác cần giải quyết.

Mỗi mục có mã dạng `1.1` (phase 1, mục 1). Đổi `[ ]` thành `[x]` khi hoàn thành; các checkbox ban đầu chưa phản ánh tiến độ triển khai thực tế của project.

### Phase 01 — Project Foundation

Mục tiêu: Thiết lập nền tảng chuẩn cho dự án.

- [x] **1.1** Khởi tạo ASP.NET Core Web API.
- [x] **1.2** Thiết kế Clean Architecture và phân chia các layer.
- [x] **1.3** Cấu hình Dependency Injection.
- [x] **1.4** Thiết lập EF Core và SQL Server.
- [x] **1.5** Quản lý database migration.
- [x] **1.6** Xây dựng Global Exception Handling.
- [x] **1.7** Chuẩn hóa API Response và Problem Details.
- [x] **1.8** Tích hợp FluentValidation.
- [x] **1.9** Tổ chức Application Service theo từng nhóm nghiệp vụ; Controller gọi Service trực tiếp.
- [x] **1.10** Tích hợp validation bằng action filter.
- [ ] **1.11** Logging và đo thời gian xử lý request bằng middleware.
- [x] **1.12** Cấu hình Options Pattern và environment settings.
- [x] **1.13** Tích hợp Swagger/OpenAPI.
- [x] **1.14** Xây dựng Health Check endpoint.

### Phase 02 — User Management

Mục tiêu: Ôn tập CRUD, Entity Relationships, LINQ và EF Core.

- [x] **2.1** Tạo tài khoản người dùng.
- [x] **2.2** Cập nhật thông tin người dùng.
- [x] **2.3** Xem chi tiết người dùng.
- [x] **2.4** Lấy danh sách người dùng.
- [ ] **2.5** Phân trang, tìm kiếm, lọc và sắp xếp.
- [ ] **2.7** Soft Delete và khôi phục tài khoản.
- [ ] **2.10** Kiểm tra trùng email và username.
- [ ] **2.11** Áp dụng optimistic concurrency khi cập nhật.
- [ ] **2.12** Ghi nhận CreatedAt, UpdatedAt, CreatedBy, UpdatedBy.

### Phase 03 — Authentication

Mục tiêu: Xây dựng hệ thống xác thực an toàn.

- [ ] **3.1** Đăng ký tài khoản.
- [ ] **3.2** Đăng nhập bằng email và password.
- [ ] **3.3** Xác thực email khi đăng ký.
- [ ] **3.4** JWT Access Token.
- [ ] **3.5** Refresh Token.
- [ ] **3.6** Refresh Token Rotation.
- [ ] **3.7** Đăng xuất khỏi thiết bị hiện tại.
- [ ] **3.14** Multi-Factor Authentication bằng TOTP.
- [ ] **3.15** Recovery Codes cho MFA.
- [ ] **3.16** Thu hồi token hoặc session khi có sự kiện bảo mật.

### Phase 04 — Authorization (RBAC & Permission)

Mục tiêu: Ôn tập authorization, policy-based authorization và quan hệ many-to-many.

- [x] **4.1** CRUD Role.
- [x] **4.2** CRUD Permission.
- [x] **4.3** Gán nhiều Role cho User.
- [x] **4.4** Thu hồi Role khỏi User.
- [ ] **4.5** Gán nhiều Permission cho Role.
- [ ] **4.6** Thu hồi Permission khỏi Role.
- [ ] **4.7** Xem danh sách Permission của User.
- [x] **4.8** Kiểm tra quyền truy cập theo Role.
- [ ] **4.9** Kiểm tra quyền truy cập theo Permission.
- [ ] **4.10** Xây dựng custom Authorization Policy.
- [ ] **4.11** Xây dựng custom Authorization Handler.
- [ ] **4.12** Dynamic Permission Authorization.
- [ ] **4.13** Phân biệt quyền của Admin và User.
- [ ] **4.14** Giới hạn truy cập tài nguyên theo quyền sở hữu.
- [ ] **4.15** Ngăn chặn privilege escalation khi thay đổi Role/Permission.

### Phase 05 — Email & Notification

Mục tiêu: Thực hành tích hợp email provider, HTML template, file đính kèm, theo dõi gửi email và in-app notification; dùng email xác thực tài khoản làm tình huống gửi email đại diện.

- [ ] **5.1** Gửi email xác thực tài khoản.
- [ ] **5.6** Xây dựng email template bằng HTML.
- [ ] **5.7** Hỗ trợ biến động trong email template.
- [ ] **5.8** Gửi email có file đính kèm.
- [ ] **5.9** Tích hợp SMTP hoặc Amazon SES.
- [ ] **5.10** Lưu lịch sử gửi email.
- [ ] **5.11** Theo dõi trạng thái gửi email.
- [ ] **5.12** Xây dựng In-App Notification.
- [ ] **5.13** Đánh dấu thông báo đã đọc/chưa đọc.
- [ ] **5.14** Quản lý email notification preferences.

### Phase 06 — File Management

Mục tiêu: Ôn tập File I/O, Stream và object storage.

- [ ] **6.1** Upload avatar.
- [ ] **6.2** Upload nhiều file cùng lúc.
- [ ] **6.3** Validate dung lượng và định dạng file.
- [ ] **6.4** Download file.
- [ ] **6.5** Xóa file.
- [ ] **6.6** Quản lý metadata của file.
- [ ] **6.7** Lưu file vào local storage.
- [ ] **6.8** Tích hợp AWS S3 hoặc MinIO.
- [ ] **6.9** Upload file bằng presigned URL.
- [ ] **6.10** Download file bằng presigned URL.
- [ ] **6.11** Streaming file dung lượng lớn.
- [ ] **6.12** Chunked Upload.
- [ ] **6.13** Phân quyền truy cập file.
- [ ] **6.14** Xử lý file không còn được tham chiếu (orphan files).

### Phase 07 — Excel & PDF

Mục tiêu: Tạo chức năng import/export và reporting.

- [ ] **7.1** Export danh sách User ra Excel.
- [ ] **7.2** Export danh sách Role và Permission ra Excel.
- [ ] **7.3** Import nhiều User từ Excel.
- [ ] **7.4** Validate dữ liệu theo từng dòng khi import.
- [ ] **7.5** Xuất danh sách lỗi import.
- [ ] **7.6** Hỗ trợ Excel Template.
- [ ] **7.7** Xử lý duplicate data khi import.
- [ ] **7.8** Export dữ liệu theo bộ lọc và phân trang phù hợp.
- [ ] **7.9** Tạo PDF User Profile.
- [ ] **7.10** Tạo PDF Audit Report.
- [ ] **7.11** Tạo PDF từ HTML Template.
- [ ] **7.12** Thêm logo, header, footer vào PDF.
- [ ] **7.13** Tạo báo cáo theo khoảng thời gian.
- [ ] **7.14** Download báo cáo đã tạo.
- [ ] **7.15** Xử lý export dữ liệu lớn bằng background job.

### Phase 08 — Caching & Redis

Mục tiêu: Ôn tập distributed caching và cache consistency.

- [ ] **8.1** Tích hợp Redis Distributed Cache.
- [ ] **8.2** Cache thông tin User Profile.
- [ ] **8.3** Cache danh sách Role và Permission.
- [ ] **8.4** Áp dụng Cache-Aside Pattern.
- [ ] **8.5** Xóa cache khi dữ liệu thay đổi.
- [ ] **8.6** Cấu hình TTL cho từng loại cache.
- [ ] **8.7** Xây dựng Cache Abstraction.
- [ ] **8.8** Áp dụng cache cho các thao tác đọc dữ liệu trong Service.
- [ ] **8.9** Chống Cache Stampede.
- [ ] **8.10** Áp dụng Distributed Lock cho tác vụ phù hợp.
- [ ] **8.11** Giải quyết vấn đề cache consistency.
- [ ] **8.12** Xử lý khi Redis không khả dụng.
- [ ] **8.13** Theo dõi Cache Hit/Miss.

### Phase 09 — Message Queue & Event-Driven

Mục tiêu: Tích hợp RabbitMQ và xử lý bất đồng bộ.

- [ ] **9.1** Tích hợp RabbitMQ.
- [ ] **9.2** Xây dựng Message Publisher.
- [ ] **9.3** Xây dựng Message Consumer.
- [ ] **9.4** Publish event khi User đăng ký.
- [ ] **9.5** Publish event khi User thay đổi mật khẩu.
- [ ] **9.6** Consumer xử lý gửi email.
- [ ] **9.7** Consumer xử lý tạo notification.
- [ ] **9.8** Thiết lập Message Retry.
- [ ] **9.9** Thiết lập Dead Letter Queue (DLQ).
- [ ] **9.10** Xử lý duplicate message bằng Idempotency.
- [ ] **9.11** Xử lý lỗi và reconnect khi RabbitMQ mất kết nối.
- [ ] **9.12** Quản lý acknowledgment và prefetch.
- [ ] **9.13** Xây dựng Transactional Outbox Pattern.
- [ ] **9.14** Theo dõi trạng thái xử lý message.
- [ ] **9.15** Quản lý version của event.

### Phase 10 — Background Jobs & Scheduling

Mục tiêu: Ôn tập Hosted Service, Worker Service, scheduling và vòng đời job; dùng job dọn dẹp file tạm làm tình huống thực hành đại diện.

- [ ] **10.1** Xây dựng BackgroundService.
- [ ] **10.4** Xây dựng job dọn dẹp file tạm.
- [ ] **10.7** Tích hợp Hangfire hoặc Quartz.NET.
- [ ] **10.8** Tạo Recurring Jobs.
- [ ] **10.9** Tạo Delayed Jobs.
- [ ] **10.10** Quản lý trạng thái thực thi job.
- [ ] **10.11** Retry job khi thất bại.
- [ ] **10.12** Hủy job đang chạy bằng CancellationToken.
- [ ] **10.13** Ngăn job chạy trùng trên nhiều instance.
- [ ] **10.14** Theo dõi job execution history.
- [ ] **10.15** Xử lý graceful shutdown.

### Phase 11 — Audit Logging & Observability

Mục tiêu: Theo dõi hoạt động hệ thống, hỗ trợ debugging và production monitoring.

- [ ] **11.1** Tích hợp Serilog và Structured Logging.
- [ ] **11.2** Ghi log HTTP Request/Response an toàn.
- [ ] **11.3** Tạo Correlation ID cho mỗi request.
- [ ] **11.4** Ghi log thao tác nghiệp vụ trong Service.
- [ ] **11.5** Ghi lại các thao tác thay đổi dữ liệu.
- [ ] **11.6** Xây dựng Audit Log cho User, Role và Permission.
- [ ] **11.7** Theo dõi lịch sử đăng nhập thành công/thất bại.
- [ ] **11.8** Theo dõi thay đổi thông tin nhạy cảm.
- [ ] **11.9** Xây dựng API truy vấn Audit Log.
- [ ] **11.10** Tích hợp OpenTelemetry.
- [ ] **11.11** Distributed Tracing giữa API, Queue và Worker.
- [ ] **11.12** Thu thập application metrics.
- [ ] **11.13** Tích hợp Prometheus và Grafana.
- [ ] **11.14** Xây dựng Health Checks cho SQL Server, Redis và RabbitMQ.
- [ ] **11.15** Cấu hình cảnh báo khi hệ thống xảy ra lỗi nghiêm trọng.

### Phase 12 — Advanced Database & Reliability

Mục tiêu: Thực hành những vấn đề thường gặp trong hệ thống production.

- [ ] **12.1** Áp dụng Database Transaction.
- [ ] **12.2** Xây dựng Transaction Middleware và phối hợp với Unit of Work.
- [ ] **12.3** Tìm hiểu EF Core SaveChanges và transaction mặc định.
- [ ] **12.4** Áp dụng Optimistic Concurrency.
- [ ] **12.5** Xử lý concurrent update.
- [ ] **12.6** Thiết lập Unique Constraint và Database Index.
- [ ] **12.7** Tối ưu các truy vấn LINQ.
- [ ] **12.8** Phát hiện và xử lý N+1 Query.
- [ ] **12.9** Phân biệt Tracking và No-Tracking Query.
- [ ] **12.10** Tối ưu truy vấn bằng Projection.
- [ ] **12.11** Phân tích SQL Execution Plan.
- [ ] **12.12** Áp dụng Retry và Timeout Policy.
- [ ] **12.13** Áp dụng Circuit Breaker với Polly.
- [ ] **12.14** Thiết kế Idempotent API.
- [ ] **12.15** Xử lý transaction kết hợp Database và Message Queue.
- [ ] **12.16** Xử lý lỗi khi các external services không khả dụng.
- [ ] **12.17** Thiết lập Rate Limiting.

### Phase 13 — Testing & Security

Mục tiêu: Đảm bảo chất lượng code và bảo mật ứng dụng.

- [ ] **13.1** Viết Unit Test với xUnit.
- [ ] **13.2** Sử dụng Moq hoặc NSubstitute.
- [ ] **13.3** Unit Test Authentication Service.
- [ ] **13.4** Unit Test Authorization Handler.
- [ ] **13.5** Unit Test FluentValidation.
- [ ] **13.6** Unit Test các Application Service xử lý nghiệp vụ.
- [ ] **13.7** Integration Test API.
- [ ] **13.8** Integration Test với SQL Server.
- [ ] **13.9** Integration Test với Redis và RabbitMQ bằng Testcontainers.
- [ ] **13.10** Kiểm thử luồng đăng ký và đăng nhập.
- [ ] **13.11** Kiểm thử Refresh Token Rotation.
- [ ] **13.12** Kiểm thử quyền truy cập Role/Permission.
- [ ] **13.13** Kiểm thử concurrent requests.
- [ ] **13.14** Kiểm thử các lỗi bảo mật phổ biến theo OWASP.
- [ ] **13.15** Kiểm tra và bảo vệ thông tin nhạy cảm trong log.
- [ ] **13.16** Quản lý Secrets an toàn.
- [ ] **13.17** Kiểm thử hiệu năng bằng k6 hoặc NBomber.

### Phase 14 — DevOps & Production Deployment

Mục tiêu: Triển khai hệ thống và quản lý ứng dụng trong môi trường production.

- [ ] **14.1** Dockerize ASP.NET Core Web API.
- [ ] **14.2** Dockerize Background Worker.
- [ ] **14.3** Thiết lập Docker Compose.
- [ ] **14.4** Chạy SQL Server, Redis và RabbitMQ bằng Docker.
- [ ] **14.5** Cấu hình Development/Staging/Production.
- [ ] **14.6** Thiết lập CI Pipeline bằng GitHub Actions.
- [ ] **14.7** Tự động Build và Test.
- [ ] **14.8** Tích hợp Code Quality Check.
- [ ] **14.9** Tự động Build Docker Image.
- [ ] **14.10** Thiết lập CD Pipeline.
- [ ] **14.11** Deploy lên AWS EC2.
- [ ] **14.12** Cấu hình Nginx Reverse Proxy.
- [ ] **14.13** Thiết lập HTTPS.
- [ ] **14.14** Quản lý Environment Variables và Secrets.
- [ ] **14.15** Cấu hình Database Migration khi triển khai.
- [ ] **14.16** Triển khai nhiều instance API.
- [ ] **14.17** Thực hành Zero-Downtime Deployment.
- [ ] **14.18** Thiết lập Database Backup và Restore.
- [ ] **14.19** Xây dựng quy trình Rollback khi deploy thất bại.

## Các milestone nên đạt được

| Milestone                   | Hoàn thành  | Kết quả                                 |
| --------------------------- | ----------- | --------------------------------------- |
| M1 — Core Identity          | Phase 01–04 | User Management, JWT, RBAC hoạt động    |
| M2 — Business Features      | Phase 05–07 | Email, File, Excel, PDF                 |
| M3 — Distributed Processing | Phase 08–10 | Redis, RabbitMQ, Background Jobs        |
| M4 — Production Readiness   | Phase 11–13 | Logging, Reliability, Testing, Security |
| M5 — Deployment             | Phase 14    | Hệ thống chạy trên server thực tế       |

## Stack công nghệ gợi ý

| Thành phần          | Công nghệ                             |
| ------------------- | ------------------------------------- |
| Backend             | ASP.NET Core Web API                  |
| Architecture        | Modular Monolith + Clean Architecture |
| Application Pattern | Service Layer (Controller → Service)  |
| Database            | SQL Server + EF Core                  |
| Authentication      | ASP.NET Core Identity, JWT            |
| Authorization       | Policy-based + Permission-based       |
| Validation          | FluentValidation                      |
| Cache               | Redis                                 |
| Message Broker      | RabbitMQ                              |
| Background Jobs     | Hangfire, BackgroundService           |
| Email               | SMTP hoặc Amazon SES                  |
| File Storage        | Local, AWS S3 / MinIO                 |
| Excel               | ClosedXML                             |
| PDF                 | QuestPDF                              |
| Logging             | Serilog                               |
| Observability       | OpenTelemetry, Prometheus, Grafana    |
| Testing             | xUnit, Testcontainers                 |
| Resilience          | Polly                                 |
| Deployment          | Docker, GitHub Actions, AWS EC2       |

## Một vài nguyên tắc để project thực sự giúp bạn ôn tập

- Không cần Microservices ngay từ đầu. Modular Monolith đủ để thực hành hầu hết các chủ đề, tránh mất thời gian quản lý hạ tầng.
- Mỗi phase nên có chức năng chạy được. Không chỉ tạo interface hay viết code mẫu.
- Không áp dụng Design Pattern chỉ vì muốn dùng nó. Nên hiểu vấn đề trước, sau đó mới chọn pattern.
- Ưu tiên framework có sẵn. Ví dụ, nên tìm hiểu ASP.NET Core Identity trước khi quyết định tự xây dựng toàn bộ authentication.
- Mỗi phase hoàn thành nên có test và tài liệu API. Không cần đợi Phase 13 mới bắt đầu testing.
- Chú trọng concurrency, transaction, security và error handling. Đây là những phần tạo ra sự khác biệt khi chuyển từ code chạy được sang code đủ tin cậy cho production.

Đề xuất: Giữ 14 phase làm roadmap chính thức cho INuStore. Khi triển khai từng phase, bạn có thể yêu cầu mình phân tích tiếp thành Functional Requirements, Business Rules, API Endpoints, Database Design và Acceptance Criteria. Như vậy bạn vẫn được tự suy nghĩ và implement thay vì nhận sẵn toàn bộ lời giải.