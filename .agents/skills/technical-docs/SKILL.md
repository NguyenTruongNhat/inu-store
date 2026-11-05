---
name: technical-docs
description: Write or update Vietnamese technical documentation for freshers in INuStore, grounded in repository code and official Microsoft Learn documentation. Use for requests to create, save, rewrite, or update technical docs with 5W1H explanations, relevant comparisons, and production recommendations. For explanation-only questions, answer in chat without creating files unless requested.
---

# Technical Docs

Viết để fresher hiểu vấn đề, đọc được code và biết cách áp dụng trong project. Mặc định lưu tài liệu trong `technical-docs/` ở repository root; ưu tiên đường dẫn/phạm vi khác nếu người dùng chỉ định.

## Phạm vi và căn cứ

- Chỉ tạo/lưu/sửa file tài liệu khi yêu cầu có ý định đó. Một câu hỏi giải thích, kể cả khi gọi skill, không tự cho phép lưu file.
- Đọc `AGENTS.md` áp dụng, tài liệu hiện có và source sở hữu hành vi trước khi viết. Kiểm tra phiên bản từ project/package/configuration thực tế; không dùng ghi chú cũ để khẳng định hiện trạng.
- Phân biệt quy ước trong AGENTS với hành vi đang có trong code. Nếu khác nhau, mô tả rõ phần chưa khớp thay vì giả định code đã thực hiện quy ước.
- Giữ đúng tên `INustore.UseCase` và các tên class/method/route đã xác minh. Không tự sửa code ứng dụng, package hoặc configuration để làm tài liệu trông đúng hơn.

## Tra cứu Microsoft Learn MCP

Với C#, .NET, ASP.NET Core và EF Core, ưu tiên server `microsoft-learn` được cấu hình ở `.codex/config.toml`:

1. Dùng `microsoft_docs_search` để tìm chủ đề và nguồn chính thức.
2. Dùng `microsoft_docs_fetch` cho trang liên quan khi viết hướng dẫn chi tiết, giải thích lỗi hoặc khi trích đoạn search chưa đủ.
3. Dùng `microsoft_code_sample_search` khi cần ví dụ code Microsoft; chọn đúng `language`, ví dụ `csharp`, `powershell` hoặc `sql`.

Đối chiếu ví dụ tìm được với phiên bản project trước khi điều chỉnh cho nghiệp vụ hiện tại. Không đưa API chỉ có ở phiên bản mới/preview vào hướng dẫn cho phiên bản đang dùng mà không nói rõ.

Dẫn URL nguồn ngay gần nhận định/ví dụ được hỗ trợ; không bịa link hoặc coi snippet search là toàn bộ ngữ cảnh. Phân biệt khuyến nghị của nguồn với suy luận của người viết. Với thư viện ngoài Microsoft, dùng nguồn chính thức của tác giả/vendor.

Nếu MCP không khả dụng, dùng tài liệu chính thức qua công cụ web có sẵn. Nếu chưa kiểm chứng được thông tin cần cập nhật, ghi rõ giới hạn và tiếp tục phần có căn cứ từ source; không khẳng định đã tra cứu thành công.

## Chọn file và tích hợp solution

- Khi tạo mới, quét các file hiện có trong thư mục tài liệu và lấy prefix số hợp lệ theo `^(\d+)\.`. Chọn số lớn nhất + 1; nếu chưa có prefix hợp lệ thì bắt đầu từ 1. Không dùng số lượng file + 1.
- Dùng `<số>.<english-kebab-case>.MD`, ví dụ `6.ef-core-loading-strategies.MD`. Tên người dùng đưa là gợi ý và được đổi cho rõ nghĩa, trừ khi họ yêu cầu giữ nguyên.
- Khi cập nhật, giữ số của tài liệu cũ nếu không được yêu cầu sắp xếp lại. Không đổi số hàng loạt, ghi đè file trùng tên hoặc tạo bản sao của chủ đề đã có mà không xem nội dung.
- Khi đổi tên, cập nhật các liên kết thực sự trỏ đến file cũ và bỏ tham chiếu solution cũ tương ứng.
- Đọc `INuStore.slnx`, tìm solution folder đang chứa các file tài liệu rồi thêm/sửa `File Path` tại đó. Giữ cách tổ chức hiện có, kể cả folder lồng như `/Solution Items/technical-docs/`; không tạo nhánh song song nếu đã có.
- Không di chuyển file vật lý chỉ để hiển thị trong Visual Studio. Kiểm tra file tham chiếu tồn tại và solution vẫn đọc được.

## Viết để hiểu cơ chế

Khi tạo mới hoặc tổ chức lại một bài, đọc [khung tài liệu](references/document-template.md). Đây là khung linh hoạt, không phải yêu cầu mọi bài có cùng số mục. Cập nhật nhỏ nên giữ cấu trúc bài và phạm vi yêu cầu.

Bao quát 5W1H: What, Why, Who, When, Where, How. Có thể lồng các câu trả lời vào nội dung hoặc bảng, không lặp lại cùng một ý dưới sáu tiêu đề.

- Đi từ vấn đề thực tế → thuật ngữ/khái niệm → cơ chế → ví dụ → áp dụng. Giải thích thuật ngữ trước khi dùng và giữ tên tiếng Anh cần thiết.
- Dùng ví dụ nhỏ gần nghiệp vụ User/Role/Auth hoặc phần code đang được hỏi; không áp đặt các entity này lên chủ đề không liên quan.
- Với code/lệnh, nêu đầu vào, kết quả dự kiến và điểm quan trọng. Giải thích vì sao hoạt động; chỉ rõ khi nào I/O thực sự xảy ra, dữ liệu đã có trong bộ nhớ hay được tải thêm, và thay đổi nào mới được lưu.
- Tách rõ **code hiện tại**, **ví dụ minh họa** và **phương án đề xuất**. Không trình bày một endpoint, dependency hoặc kết quả chạy chưa tồn tại như đã có.
- Nêu hiểu nhầm/lỗi thường gặp có liên quan, nguyên nhân và cách khắc phục. Dẫn đến đúng file và symbol đã đọc; kiểm tra lại line number nếu sử dụng.
- Dùng Mermaid khi giúp hiểu luồng: điểm bắt đầu, bước chính, nhánh lỗi liên quan và kết quả. Sơ đồ phải khớp source và nội dung; bài đơn giản không bắt buộc có sơ đồ.

## So sánh và khuyến nghị production

- Chỉ thêm so sánh khi giúp ra quyết định. Dùng bảng cơ chế, ưu điểm, hạn chế, khi dùng và ảnh hưởng tới project; chọn tiêu chí phù hợp như I/O, hiệu năng, bảo trì, kiểm thử hoặc vận hành.
- Ví dụ EF Core: so sánh eager/lazy/explicit loading khi chủ đề là tải quan hệ; thêm projection khi liên quan đến dữ liệu trả về. Đọc query thực tế, không suy ra cách tải chỉ từ tên package hoặc cấu hình proxy.
- Không mặc định công nghệ mới hơn luôn tốt hơn. Đưa ra khuyến nghị cho bối cảnh project và điều kiện nên chọn phương án khác.
- Khi nói về phiên bản, bảo mật hoặc khuyến nghị có thể thay đổi, tra cứu nguồn hiện tại. Không khẳng định thị phần, “production luôn dùng/phổ biến nhất”, số query cố định hoặc số liệu hiệu năng nếu chưa có nguồn/phép đo và giả định phù hợp.
- Nêu trade-off và căn cứ; nếu là đề xuất chưa được benchmark trong project thì nói rõ.

## Kiểm tra và bàn giao

- Đối chiếu code, route, tên file, package, configuration và command với nguồn đã đọc. Không đưa credential/token thật vào ví dụ.
- Kiểm tra link, code fence, Mermaid và cú pháp lệnh khi có công cụ phù hợp. Không nói “đã test/chạy thành công” chỉ vì đã đọc source hoặc parse cú pháp.
- Không tự chạy các lệnh trong bài để minh họa, nhất là migration, SQL thay đổi dữ liệu, cài package hoặc đổi configuration ứng dụng. Chỉ thực thi tác vụ đó nếu người dùng yêu cầu riêng và trong phạm vi đã cho phép.
- Với thay đổi thuần docs, kiểm tra nội dung/đường dẫn/solution; không tự build hoặc chạy toàn bộ test ứng dụng.
- Kết thúc bằng đường dẫn tài liệu, tên file đổi nếu có, kiểm tra đã thực hiện và phần chưa xác minh. Giữ báo cáo ngắn, không chép lại toàn bộ bài.
