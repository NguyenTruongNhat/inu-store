Bạn hiểu đúng: **`SaveChanges()` thực hiện SQL và chờ database xử lý xong.** Nhưng code của bạn gọi **`SaveChangesAsync()`**, nên cách chờ kết quả khác nhau.

Luồng thêm user gồm hai bước:

```csharp
_unitOfWork.UserRepository.Add(user);
```

Bước này chỉ đưa user vào `ChangeTracker` của DbContext với trạng thái `Added`, chưa gửi `INSERT`.

```csharp
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

Bước này thực hiện lưu xuống database. `await` giúp code phía sau chờ thao tác lưu hoàn tất.

Trong code cũ:

```csharp
_unitOfWork.SaveChangesAsync(cancellationToken);
return Ok();
```

**`SaveChangesAsync()` vẫn bắt đầu chạy dù thiếu `await`.** Nhưng khi thao tác database đang chờ I/O, nó trả về một `Task`. Bạn bỏ qua `Task` đó, nên luồng xử lý có thể diễn ra như sau:

1. Bắt đầu lưu user.
2. Database chưa xử lý xong, code tiếp tục chạy.
3. Controller trả `200 OK`.
4. Request kết thúc, DI giải phóng DbContext.
5. Thao tác lưu đang chạy có thể thất bại vì DbContext hoặc connection đã bị đóng. Exception nằm trong `Task` bị bỏ qua nên controller không nhận được.

Nếu database xử lý đủ nhanh, code thiếu `await` **vẫn có thể thêm được user**. Vì thế lỗi này có thể xuất hiện không ổn định; chỉ từ việc thiếu `await` cũng chưa xác định được exception cụ thể của lần lưu thất bại.

Sau khi sửa, thứ tự được bảo đảm:

```text
Controller await CreateUser()
    → Service Add(user)
    → Service await SaveChangesAsync()
    → Lưu thành công: controller trả 200 OK
    → Lưu thất bại: exception truyền về luồng xử lý request
```

**`await` không kích hoạt SQL; nó chờ SQL hoàn tất và giúp nhận lỗi khi lưu.** Đó là lý do cần `Task` và `await` xuyên suốt từ controller xuống service.