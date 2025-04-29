TodoList Web App
Ứng dụng TodoList là một ứng dụng web giúp người dùng quản lý công việc hàng ngày một cách đơn giản và hiệu quả. Ứng dụng hỗ trợ các chức năng cơ bản như đăng ký, đăng nhập, tạo, sửa, xóa và đánh dấu hoàn thành các công việc.

🚀 Công nghệ sử dụng
🔧 Backend
ASP.NET Core MVC: Framework chính để xây dựng ứng dụng web theo mô hình MVC (Model-View-Controller).

C#: Ngôn ngữ lập trình chính để xử lý logic backend.

MongoDB: Cơ sở dữ liệu NoSQL dùng để lưu trữ thông tin người dùng và các công việc.

ASP.NET Core Identity: Cung cấp hệ thống xác thực và quản lý người dùng như đăng nhập, đăng ký, đăng xuất.

Dependency Injection: Sử dụng hệ thống DI tích hợp sẵn của ASP.NET Core để dễ dàng quản lý và kiểm thử các thành phần.

🎨 Frontend
Razor Views: View engine của ASP.NET Core để tạo ra giao diện HTML động.

Bulma CSS: Framework CSS hiện đại giúp xây dựng giao diện đẹp mắt và responsive.

CSS tùy chỉnh: Tùy chỉnh giao diện với theme màu nâu đất, tạo sự khác biệt và thân thiện với người dùng.

Font Awesome: Sử dụng bộ biểu tượng giúp hiển thị các icon trực quan như nút chỉnh sửa, xóa, hoàn thành,...

JavaScript: Tăng cường tương tác người dùng, ví dụ như xử lý mở rộng menu navbar, xác nhận xóa công việc,...

✅ Tính năng chính
Đăng ký và đăng nhập tài khoản người dùng.

Tạo mới công việc với tiêu đề, nội dung và trạng thái.

Chỉnh sửa và xóa công việc.

Đánh dấu công việc đã hoàn thành.

Giao diện trực quan, dễ sử dụng trên cả desktop và thiết bị di động.

📷 Giao diện mẫu
![image](https://github.com/user-attachments/assets/9dd8fa2f-ce49-4b33-a76e-783a87a5f4fc)
![image](https://github.com/user-attachments/assets/5e1c3bd9-bc00-4a64-a7f4-829ddd47309e)
![image](https://github.com/user-attachments/assets/e0f18ee1-bfa8-43b6-8744-a4cc80762dbe)
⚙️ Hướng dẫn cài đặt
Clone repository về máy:

bash
Copy
Edit
git clone https://github.com/your-username/todolist-aspnetmvc.git
Mở solution trong Visual Studio và chạy ứng dụng.

Đảm bảo đã cài đặt MongoDB và chỉnh chuỗi kết nối trong appsettings.json.

Truy cập địa chỉ https://localhost:5001 (hoặc port tương ứng) để sử dụng ứng dụng.

📁 Cấu trúc thư mục
├── Controllers/           // Các controller xử lý request
├── Models/                // Các lớp mô hình dữ liệu
├── Views/                 // Giao diện Razor View
├── wwwroot/               // Tài nguyên tĩnh (CSS, JS, hình ảnh)
├── Services/              // Các dịch vụ xử lý logic
└── appsettings.json       // Cấu hình kết nối và ứng dụng

