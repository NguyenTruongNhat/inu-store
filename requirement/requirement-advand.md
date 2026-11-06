# INustore — Advanced Requirements (Senior Level)

Mình bổ sung các technical challenges dành cho Senior .NET Developer, tập trung vào Concurrency, Distributed Systems, Security, Performance, Reliability và System Design.

Các requirement mới sẽ được đánh dấu `*`, ví dụ `14.20*`, và tiếp tục đánh số từ danh sách trước. Một số mục là technical issue cần giải quyết, không chỉ đơn thuần là xây dựng chức năng.

## Phase 02 — Advanced User Management

- [ ] 2.13\* Xử lý trường hợp hai request đồng thời cập nhật cùng một User, đảm bảo không xảy ra Lost Update.
- [ ] 2.14\* Thiết kế Bulk Update cho 100.000 User, tối ưu memory và database performance.
- [ ] 2.15\* Xử lý Race Condition khi hai request đồng thời tạo tài khoản với cùng email.
- [ ] 2.16\* Thiết kế Keyset Pagination cho bảng User có hàng triệu bản ghi.
- [ ] 2.17\* Xử lý thay đổi email người dùng trong khi các request khác vẫn đang sử dụng email cũ.

## Phase 03 — Advanced Authentication & Security

- [ ] 3.17\* Xử lý hai request Refresh Token đồng thời nhằm ngăn Token Reuse và Race Condition.
- [ ] 3.18\* Thiết kế cơ chế thu hồi Access Token trước khi hết hạn trong môi trường nhiều instance.
- [ ] 3.19\* Xử lý sự cố Redis không khả dụng trong quá trình xác thực hoặc Refresh Token.
- [ ] 3.20\* Thiết kế Session Management hỗ trợ nhiều thiết bị và đăng xuất một thiết bị cụ thể.
- [ ] 3.21\* Triển khai Signing Key Rotation mà không làm gián đoạn phiên đăng nhập hiện tại.
- [ ] 3.22\* Phát hiện hành vi đăng nhập bất thường và đưa ra cơ chế xác thực bổ sung.
- [ ] 3.23\* Thiết kế chống Brute Force và Credential Stuffing trong môi trường nhiều API instance.

## Phase 04 — Advanced Authorization

- [ ] 4.16\* Giải quyết Permission Cache Stale khi Admin thu hồi quyền nhưng User vẫn đang sử dụng Access Token cũ.
- [ ] 4.17\* Thiết kế Dynamic Authorization Policy không cần restart ứng dụng khi Permission thay đổi.
- [ ] 4.18\* Xử lý đồng thời khi nhiều Admin thay đổi Role và Permission của cùng User.
- [ ] 4.19\* Thiết kế Multi-Tenant Authorization, ngăn User truy cập dữ liệu thuộc Tenant khác.
- [ ] 4.20\* Xây dựng Resource-Based Authorization kết hợp RBAC, Ownership và Business Rules.
- [ ] 4.21\* Thiết kế cơ chế bảo vệ quyền Super Admin, ngăn tự thu hồi quyền quản trị cuối cùng.

## Phase 05 — Advanced Email & Notification

- [ ] 5.15\* Đảm bảo không làm mất yêu cầu gửi email khi Database lưu thành công nhưng hệ thống gửi email gặp lỗi.
- [ ] 5.16\* Thiết kế Email Delivery hỗ trợ retry nhưng hạn chế gửi trùng email.
- [ ] 5.17\* Xử lý Email Provider Rate Limit khi cần gửi hàng chục nghìn email.
- [ ] 5.18\* Thiết kế Failover giữa hai Email Providers khi một provider gặp sự cố.
- [ ] 5.19\* Xử lý trạng thái Email Delivery bất đồng bộ thông qua Webhook và chống webhook trùng lặp.
- [ ] 5.20\* Thiết kế Notification Preferences đảm bảo thay đổi của người dùng được áp dụng trong khi message đang nằm trong queue.

## Phase 06 — Advanced File Management

- [ ] 6.15\* Thiết kế Resumable Upload cho file nhiều GB khi kết nối mạng bị gián đoạn.
- [ ] 6.16\* Xử lý đồng thời nhiều request upload các chunk của cùng một file.
- [ ] 6.17\* Đảm bảo File Metadata trong Database và Object Storage không bị mất đồng bộ.
- [ ] 6.18\* Thiết kế cơ chế kiểm tra virus và xác thực nội dung file trước khi cho phép download.
- [ ] 6.19\* Xử lý tình huống Presigned URL bị lộ hoặc tiếp tục có hiệu lực sau khi quyền truy cập của User bị thu hồi.
- [ ] 6.20\* Thiết kế File Deduplication bằng Content Hash và xử lý upload trùng đồng thời.

## Phase 07 — Advanced Excel & PDF

- [ ] 7.16\* Export một triệu bản ghi ra Excel mà không gây OutOfMemoryException.
- [ ] 7.17\* Thiết kế Import Pipeline xử lý file Excel lớn theo batch.
- [ ] 7.18\* Xây dựng cơ chế Resume Import khi hệ thống bị restart giữa chừng.
- [ ] 7.19\* Thiết kế Partial Success cho import: bản ghi hợp lệ được lưu, bản ghi lỗi được ghi nhận.
- [ ] 7.20\* Đảm bảo dữ liệu báo cáo nhất quán trong khi nhiều User đang cập nhật dữ liệu.
- [ ] 7.21\* Xử lý Formula Injection khi export dữ liệu do người dùng nhập sang Excel.

## Phase 08 — Advanced Caching & Redis

- [ ] 8.14\* Xử lý Cache Stampede khi hàng nghìn request cùng truy cập một cache key vừa hết hạn.
- [ ] 8.15\* Giải quyết Race Condition khi request cũ ghi dữ liệu stale vào cache sau request mới.
- [ ] 8.16\* Thiết kế Cache Invalidation trong hệ thống có nhiều API instance.
- [ ] 8.17\* Triển khai Stale-While-Revalidate để giảm độ trễ khi cache hết hạn.
- [ ] 8.18\* Thiết kế Distributed Lock kết hợp Fencing Token để ngăn worker giữ lock hết hạn ghi đè dữ liệu mới.
- [ ] 8.19\* Xử lý Redis Memory Eviction và Cache Key Explosion.
- [ ] 8.20\* Thiết kế Cache Warming và tránh tăng tải đột ngột lên Database khi Redis restart.
- [ ] 8.21\* Giải quyết vấn đề consistency khi cập nhật Database thành công nhưng xóa cache thất bại.

## Phase 09 — Advanced Message Queue & Event-Driven

- [ ] 9.16\* Xử lý trường hợp Consumer xử lý message thành công nhưng crash trước khi gửi ACK.
- [ ] 9.17\* Thiết kế cơ chế xử lý message đúng thứ tự cho từng User khi có nhiều Consumer chạy song song.
- [ ] 9.18\* Xử lý Poison Message gây vòng lặp retry vô hạn.
- [ ] 9.19\* Giải quyết tình huống Database commit thành công nhưng Message Publisher gửi event thất bại.
- [ ] 9.20\* Thiết kế Inbox Pattern để đảm bảo idempotent processing khi Consumer nhận message trùng.
- [ ] 9.21\* Xử lý Message Backpressure khi tốc độ Publisher lớn hơn khả năng xử lý của Consumer.
- [ ] 9.22\* Thiết kế Event Versioning khi nhiều Consumer đang sử dụng các phiên bản event khác nhau.
- [ ] 9.23\* Xử lý lỗi khi một message đã được xử lý một phần nhưng các bước tiếp theo thất bại.
- [ ] 9.24\* Thiết kế cơ chế Replay Message từ DLQ an toàn, tránh tạo duplicate side effects.
- [ ] 9.25\* Xử lý tình huống RabbitMQ mất kết nối, restart hoặc failover trong khi Publisher/Consumer đang hoạt động.

## Phase 10 — Advanced Background Jobs

- [ ] 10.16\* Ngăn cùng một scheduled job chạy đồng thời trên nhiều server.
- [ ] 10.17\* Xử lý worker crash giữa quá trình thực thi job kéo dài nhiều giờ.
- [ ] 10.18\* Thiết kế Job Checkpointing để tiếp tục xử lý từ vị trí gần nhất sau khi restart.
- [ ] 10.19\* Xây dựng cơ chế Job Prioritization và giới hạn số lượng job chạy đồng thời.
- [ ] 10.20\* Xử lý Job Starvation khi các job lớn chiếm hết tài nguyên xử lý.
- [ ] 10.21\* Thiết kế Graceful Shutdown đảm bảo job đang chạy được hoàn tất hoặc có thể tiếp tục an toàn.
- [ ] 10.22\* Thiết kế Job Timeout và Cancellation Propagation.
- [ ] 10.23\* Xử lý Distributed Job Scheduling khi hệ thống có nhiều instance và thay đổi số lượng worker.

## Phase 11 — Advanced Observability & Production Troubleshooting

- [ ] 11.16\* Điều tra Memory Leak trong ASP.NET Core khiến memory tăng liên tục sau nhiều giờ hoạt động.
- [ ] 11.17\* Phân tích ThreadPool Starvation khi API có lượng request lớn.
- [ ] 11.18\* Phát hiện và xử lý tình trạng HTTP Connection Pool hoặc Database Connection Pool Exhaustion.
- [ ] 11.19\* Thiết kế Distributed Tracing xuyên suốt API → Database → RabbitMQ → Worker.
- [ ] 11.20\* Điều tra API có P99 Latency tăng cao trong khi Average Latency vẫn thấp.
- [ ] 11.21\* Thiết kế Logging Strategy cho hệ thống có hàng triệu request mỗi ngày, cân bằng chi phí và khả năng điều tra.
- [ ] 11.22\* Xây dựng cơ chế phát hiện và cảnh báo Consumer Lag, Queue Backlog và Job Failures.
- [ ] 11.23\* Thực hành Production Incident Investigation bằng Logs, Metrics, Traces và Correlation ID.

## Phase 12 — Advanced Database & Distributed Reliability

- [ ] 12.18\* Xử lý Database Deadlock khi nhiều transaction cập nhật các bảng theo thứ tự khác nhau.
- [ ] 12.19\* Thiết kế Transaction Isolation phù hợp để ngăn Dirty Read, Non-Repeatable Read và Phantom Read.
- [ ] 12.20\* Giải quyết Race Condition giữa kiểm tra điều kiện nghiệp vụ và cập nhật dữ liệu (TOCTOU).
- [ ] 12.21\* Xử lý Write Skew khi hai transaction cập nhật các bản ghi khác nhau nhưng cùng vi phạm một business invariant.
- [ ] 12.22\* Thiết kế Idempotency Key cho API khi Client retry sau Timeout nhưng Server đã xử lý thành công.
- [ ] 12.23\* Xử lý Retry Storm khi nhiều service cùng retry tới một dependency đang gặp sự cố.
- [ ] 12.24\* Thiết kế Bulkhead Isolation để một dependency chậm không làm cạn tài nguyên toàn bộ API.
- [ ] 12.25\* Xử lý Database Connection Pool Exhaustion khi traffic tăng đột biến.
- [ ] 12.26\* Thiết kế Zero-Downtime Database Migration khi version API cũ và mới cùng hoạt động.
- [ ] 12.27\* Thiết kế Read Replica và xử lý Read-After-Write Consistency khi dữ liệu replication bị trễ.
- [ ] 12.28\* Xử lý Distributed Transaction bằng Saga Pattern và Compensating Actions.
- [ ] 12.29\* Thiết kế cơ chế đảm bảo business invariant trong môi trường nhiều instance và concurrent requests.
- [ ] 12.30\* Xử lý Partial Failure khi một nghiệp vụ gọi nhiều external services và chỉ một số bước thành công.
- [ ] 12.31\* Thực hành thiết kế và kiểm chứng cơ chế xử lý theo mô hình At-Least-Once Delivery thay vì giả định Exactly-Once Delivery.

## Phase 13 — Advanced Testing & Security

- [ ] 13.18\* Xây dựng Concurrency Test để tái hiện Race Condition trong Authentication và User Management.
- [ ] 13.19\* Xây dựng Load Test với 10.000 concurrent users và xác định bottleneck thực tế.
- [ ] 13.20\* Xây dựng Stress Test để xác định giới hạn chịu tải của API.
- [ ] 13.21\* Thực hiện Chaos Testing bằng cách chủ động ngắt Redis, SQL Server hoặc RabbitMQ.
- [ ] 13.22\* Xây dựng Fault Injection Test để kiểm tra Retry, Timeout và Circuit Breaker.
- [ ] 13.23\* Kiểm thử Refresh Token Replay Attack.
- [ ] 13.24\* Kiểm thử Broken Object Level Authorization (BOLA/IDOR).
- [ ] 13.25\* Kiểm thử Mass Assignment và Privilege Escalation.
- [ ] 13.26\* Thiết kế Security Test cho Multi-Tenant Data Isolation.
- [ ] 13.27\* Xây dựng Contract Testing giữa Message Publisher và Consumer.
- [ ] 13.28\* Kiểm thử tính Idempotent của API, Consumer và Background Job.
- [ ] 13.29\* Thực hành kiểm thử dữ liệu không bị mất hoặc xử lý trùng sau khi hệ thống crash và restart.

## Phase 14 — Advanced DevOps & Production Deployment

- [ ] 14.20\* Triển khai Blue-Green Deployment với cơ chế chuyển traffic và rollback an toàn.
- [ ] 14.21\* Triển khai Canary Deployment để phát hành phiên bản mới cho một phần người dùng.
- [ ] 14.22\* Thiết kế Auto Scaling và kiểm tra khả năng hoạt động khi số lượng API instance thay đổi.
- [ ] 14.23\* Xử lý Graceful Shutdown khi container bị terminate trong lúc đang xử lý request.
- [ ] 14.24\* Thiết kế Secret Rotation mà không làm gián đoạn hệ thống.
- [ ] 14.25\* Thiết kế Backup & Restore Strategy với mục tiêu RPO và RTO cụ thể.
- [ ] 14.26\* Thực hành Disaster Recovery khi Database chính không khả dụng.
- [ ] 14.27\* Triển khai Load Balancing và kiểm tra hệ thống không phụ thuộc vào Session Affinity.
- [ ] 14.28\* Thiết kế Deployment Strategy tương thích ngược giữa API, Database Schema và Message Contract.
- [ ] 14.29\* Thực hành Rolling Deployment khi các instance đang xử lý Background Jobs và Queue Messages.
- [ ] 14.30\* Thiết lập Resource Limits cho container và xử lý CPU Throttling, Out-of-Memory Kill.
- [ ] 14.31\* Thiết kế Health Check, Readiness Check và Liveness Check để tránh chuyển traffic tới instance chưa sẵn sàng.

## Bonus — Phase 15: Senior System Design Challenges

Các bài này mang tính tổng hợp. Bạn phải tự thiết kế giải pháp, đánh giá trade-off và giải quyết vấn đề thực tế.

- [ ] 15.1\* Thiết kế INustore phục vụ 1 triệu User, 10.000 concurrent requests với latency mục tiêu P99 dưới 200ms cho các API đọc thông thường.
- [ ] 15.2\* Thiết kế Authentication System có thể tiếp tục phục vụ những request phù hợp khi Redis hoặc một dependency không thiết yếu bị lỗi, nhưng không làm giảm yêu cầu bảo mật.
- [ ] 15.3\* Thiết kế Global Rate Limiting hoạt động chính xác khi có nhiều API instance.
- [ ] 15.4\* Thiết kế hệ thống Multi-Tenant hỗ trợ Tenant Isolation, Tenant-Specific Configuration và Tenant-Level Rate Limit.
- [ ] 15.5\* Thiết kế Audit System không làm mất các sự kiện quan trọng khi hệ thống bị crash.
- [ ] 15.6\* Thiết kế quy trình chuyển từ Modular Monolith sang Microservices mà vẫn đảm bảo tương thích API và dữ liệu.
- [ ] 15.7\* Thiết kế cơ chế xử lý 100.000 Notification/phút, có Retry, Backpressure và kiểm soát chi phí.
- [ ] 15.8\* Thiết kế hệ thống đảm bảo không mất các sự kiện nghiệp vụ quan trọng khi SQL Server, RabbitMQ hoặc Worker gặp sự cố.
- [ ] 15.9\* Phân tích và tối ưu hệ thống khi traffic tăng gấp 10 lần nhưng không được tăng hạ tầng tương ứng.
- [ ] 15.10\* Thiết kế kiến trúc Multi-Region và đánh giá trade-off giữa Availability, Consistency, Latency và Cost.

Gợi ý cách thực hành: Với các requirement có dấu `*`, bạn nên chủ động tạo tình huống lỗi trước, chứng minh lỗi bằng test hoặc log, sau đó mới triển khai giải pháp và kiểm chứng lại. Đây là cách tốt để rèn luyện tư duy của Senior Engineer thay vì chỉ triển khai tính năng theo happy path.