# Khung tài liệu 5W1H

Đọc khi viết mới hoặc tổ chức lại một bài. Chọn các phần giúp trả lời câu hỏi thực tế; gộp/bỏ phần không liên quan, không biến ghi chú nhỏ thành một bài tổng quan ngoài phạm vi.

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

### Khái niệm và hiện trạng project

Giải thích thuật ngữ trước. Chỉ đến source thực tế và phiên bản liên quan. Tách cấu hình có sẵn, hành vi quan sát được bằng source và các giả định còn cần xác minh.

### Cơ chế và luồng

Giải thích thứ tự xử lý và trách nhiệm của từng thành phần. Nếu có I/O, đánh dấu thời điểm gọi database/network hoặc lưu thay đổi; không đánh đồng một lời gọi method với việc dữ liệu đã được ghi.

Nếu dùng Mermaid, bắt đầu bằng entry point thực tế; thêm nhánh lỗi có ý nghĩa. Đừng vẽ middleware, transaction hay bước validation mà source không có.

### Ví dụ và cách áp dụng

Dùng code tối thiểu đủ minh họa, mô tả input/output dự kiến và điều kiện áp dụng. Ví dụ điều chỉnh từ Microsoft Learn cần đổi tên/type theo project, giữ đúng semantics và ghi link nguồn.

Với lệnh vận hành, nói rõ môi trường/thư mục chạy, tham số cần thay, file hoặc dữ liệu bị tác động. Phân biệt “lệnh để người đọc chạy” với “lệnh đã thực thi khi viết”.

### So sánh và quyết định

Chọn các phương án cùng giải quyết vấn đề đang hỏi. Bảng có thể gồm cơ chế, lợi ích, chi phí/hạn chế, điều kiện dùng và mức thay đổi cần thiết trong project.

Kết luận lựa chọn nào phù hợp bối cảnh hiện tại, lý do, và khi nào phương án khác đáng cân nhắc. Nguồn chính thức hỗ trợ khuyến nghị không tự chứng minh một phương án nhanh nhất trong hệ thống này.

### Hiểu nhầm, lỗi và cách kiểm tra

Chọn lỗi có khả năng gặp trong ngữ cảnh bài; giải thích triệu chứng → nguyên nhân → cách xử lý → cách nhận biết đã xử lý đúng.

Không dùng collection rỗng để kết luận database không có bản ghi nếu quan hệ chưa được tải; đây là một ví dụ về việc phải phân biệt trạng thái trong bộ nhớ và nguồn dữ liệu, không phải quy tắc buộc mọi bài bàn về EF Core.

### Nguồn và bằng chứng

Đặt citation gần nhận định tương ứng. Ghi rõ phạm vi kiểm tra: đọc source, đối chiếu tài liệu, parse cú pháp, chạy command hay kiểm thử hành vi. Dùng mục nguồn ngắn nếu bài dài cần tập hợp liên kết để đọc thêm; không chỉ dồn mọi citation xuống cuối bài.

## Hai ví dụ định hướng

- **EF Core loading:** xác định query hiện tại trước; giải thích I/O của các lựa chọn liên quan, giả định khi nói về N+1/số query, rồi đề xuất phương án cho use case cụ thể.
- **EF migration:** phân biệt sinh file với thay đổi database; giải thích target/startup/context/environment bằng cấu hình repository và ghi rõ chưa apply database nếu chỉ viết docs.
