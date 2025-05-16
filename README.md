# Bounquin 

**Bounquin** là một nền tảng đọc sách trực tuyến được xây dựng bằng ASP.NET MVC, hỗ trợ người dùng trải nghiệm sách, đánh giá, bình luận và quản lý nội dung hiệu quả.

Tính năng chính

- 📖 Đọc sách chia chương
- ✍️ Đánh giá và bình luận từng cuốn sách
- 📊 Thống kê lượt đọc, lượt xem
- 🛠️ Quản trị hệ thống (sách, người dùng, thể loại, báo cáo)
- 🔍 Tìm kiếm nâng cao theo tiêu đề, thể loại

Công nghệ sử dụng
- ASP.NET MVC 5
- Entity Framework
- SQL Server
- Bootstrap 5
- jQuery, AJAX
- Toastr & SweetAlert2 cho alert đẹp

Kiến trúc dự án

- Kiến trúc **Service - Repository**
- Tách biệt rõ các lớp: Controller, Service, Repository, ViewModels
- Sử dụng PartialView và AJAX để tối ưu trải nghiệm người dùng

Cách chạy dự án

1. Clone repository:
    ```bash
    git clone https://github.com/datne77/Bounquin.git
    ```

2. Mở file `Bounquin.sln` bằng Visual Studio

3. Kiểm tra chuỗi kết nối DB (`Web.config`) và khởi tạo CSDL nếu cần

4. Chạy ứng dụng (`F5` hoặc `Ctrl + F5`)

Ghi chú

- Bạn cần SQL Server hoặc LocalDB để chạy CSDL
- Dữ liệu mẫu có thể được khởi tạo qua file `DbInitializer` hoặc Seed (nếu có)

Giấy phép

Dự án mang tính học thuật / cá nhân. Không dùng cho mục đích thương mại nếu chưa được cho phép.



