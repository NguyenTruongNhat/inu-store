# Khung tài liệu ngắn gọn cho fresher

Đọc khi viết mới hoặc tổ chức lại một bài. Dùng bố cục linh hoạt: giữ cơ chế, ví dụ và quyết định cần hiểu; gộp/bỏ phần không liên quan. Không bắt buộc số mục hoặc số dòng cố định.

## Độ bao phủ 5W1H

| Câu hỏi | Fresher cần hiểu |
| --- | --- |
| What | Khái niệm là gì, thuật ngữ và thành phần chính có nghĩa gì? |
| Why | Vấn đề nào dẫn đến giải pháp, điều gì xảy ra nếu xử lý sai? |
| Who | Actor, lớp/layer hoặc vai trò nào sử dụng và chịu trách nhiệm? |
| When | Điều kiện kích hoạt, khi nên dùng và khi nên cân nhắc cách khác? |
| Where | Giải pháp nằm ở layer/project/file nào, dependency đi theo hướng nào? |
| How | Các bước thực thi, dữ liệu đi qua đâu, kết quả và nhánh lỗi ra sao? |

Không cần sáu heading độc lập nếu nội dung đã trả lời rõ các câu hỏi.

## Bố cục gợi ý

### Mục tiêu và tình huống

Mở bằng vấn đề cụ thể của người đọc, kết quả mong muốn và điều kiện cần biết. Chỉ thêm prerequisite thực sự cần cho chủ đề.

### Khái niệm và code hiện tại

Giải thích thuật ngữ trước. Chỉ đến file/symbol chính và phiên bản liên quan; không cần bảng liệt kê mọi file hoặc câu dẫn về repository root. Tách hành vi hiện có với đề xuất.

### Cơ chế và luồng

Giải thích thứ tự xử lý và trách nhiệm của từng thành phần. Nếu có I/O, đánh dấu thời điểm gọi database/network hoặc lưu thay đổi; không đánh đồng một lời gọi method với việc dữ liệu đã được ghi.

Nếu dùng Mermaid, chỉ giữ luồng chính và nhánh lỗi cần hiểu, khớp source. Nội dung phụ như quan hệ với transaction nên tóm bằng vài gạch đầu dòng; chỉ đi sâu khi đó là chủ đề chính.

### Ví dụ và cách áp dụng

Dùng code tối thiểu đủ minh họa, mô tả input/output dự kiến và điều kiện áp dụng. Ví dụ điều chỉnh từ Microsoft Learn cần đổi tên/type theo project, giữ đúng semantics và ghi link nguồn.

Với lệnh vận hành, nói rõ môi trường/thư mục chạy, tham số cần thay, file hoặc dữ liệu bị tác động. Phân biệt “lệnh để người đọc chạy” với “lệnh đã thực thi khi viết”.

### Best practices cho production

Nêu vài khuyến nghị trực tiếp cho chủ đề và project. Có thể dùng gạch đầu dòng hoặc bảng ngắn “Điểm hiện tại → Đề xuất”. Ưu tiên tính đúng, bảo mật, khả năng bảo trì và vận hành khi liên quan; giải thích ngắn vì sao cần làm. Ghi rõ đề xuất nào chưa áp dụng.

### So sánh khi cần ra quyết định

Chọn các phương án cùng giải quyết vấn đề đang hỏi. Bảng có thể gồm cơ chế, lợi ích, chi phí/hạn chế, điều kiện dùng và mức thay đổi cần thiết trong project.

Kết luận lựa chọn nào phù hợp bối cảnh hiện tại, lý do, và khi nào phương án khác đáng cân nhắc. Nguồn chính thức hỗ trợ khuyến nghị không tự chứng minh một phương án nhanh nhất trong hệ thống này.

### Câu hỏi phỏng vấn senior — cuối bài

Thêm 5–7 câu, mặc định 6, dưới dạng danh sách đánh số. Hỏi về thiết kế, trade-off, xử lý lỗi và tình huống production liên quan đến bài; tránh chỉ hỏi định nghĩa. Không tự viết đáp án nếu chưa được yêu cầu.

Đây là phần cuối nội dung khi tạo mới hoặc viết lại toàn bài; không ép bổ sung trong một cập nhật nhỏ ngoài phạm vi.

## Lưu ý biên tập và bàn giao

- Lồng hiểu nhầm/lỗi quan trọng vào đúng phần cơ chế hoặc khuyến nghị, không mặc định tạo mục kiểm tra riêng.
- Đặt citation gần nhận định được hỗ trợ; không cần một mục nguồn dài ở cuối.
- Kiểm tra nội dung, đường dẫn và cấu trúc như skill yêu cầu; báo ngắn kết quả và giới hạn trong lời bàn giao. Không chèn checklist test dài hoặc nhật ký xác minh vào bài nếu người dùng chưa yêu cầu.
- Rà lại: có đoạn nào lặp ý, câu dẫn dư, sơ đồ quá nhiều nhánh hoặc chi tiết phụ có thể tóm bằng gạch đầu dòng không?

## Hai ví dụ định hướng

- **EF Core loading:** xác định query hiện tại trước; giải thích I/O của các lựa chọn liên quan, giả định khi nói về N+1/số query, rồi đề xuất phương án cho use case cụ thể.
- **EF migration:** phân biệt sinh file với thay đổi database; giải thích target/startup/context/environment bằng cấu hình repository và ghi rõ chưa apply database nếu chỉ viết docs.
