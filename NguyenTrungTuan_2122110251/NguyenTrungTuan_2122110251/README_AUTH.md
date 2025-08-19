# Hệ thống Authentication - TrungTuanShop

## Tổng quan
Hệ thống authentication được xây dựng với giao diện hiện đại và responsive, hỗ trợ cả đăng nhập/đăng ký thông thường và đăng nhập bằng Google (Firebase).

## Tính năng

### 1. Đăng nhập (Login)
- **Form đăng nhập thông thường**: Email và mật khẩu
- **Đăng nhập bằng Google**: Tích hợp Firebase Authentication
- **Validation**: Kiểm tra email và mật khẩu
- **Session management**: Lưu thông tin người dùng vào session
- **Error handling**: Hiển thị thông báo lỗi rõ ràng

### 2. Đăng ký (Register)
- **Form đăng ký đầy đủ**: Họ, tên, email, mật khẩu, xác nhận mật khẩu
- **Password strength indicator**: Hiển thị độ mạnh của mật khẩu
- **Real-time validation**: Kiểm tra mật khẩu xác nhận ngay lập tức
- **Đăng ký bằng Google**: Tích hợp Firebase Authentication
- **Auto-login**: Tự động đăng nhập sau khi đăng ký thành công

### 3. Giao diện
- **Consistent design**: Sử dụng cùng style với các trang khác trong dự án
- **Hero section**: Header với background image và breadcrumb navigation
- **Bootstrap styling**: Sử dụng các class Bootstrap chuẩn
- **Responsive design**: Tương thích với mọi thiết bị
- **User-friendly**: Giao diện thân thiện, dễ sử dụng

## Cấu trúc Files

```
Views/Home/
├── Login.cshtml          # Trang đăng nhập
└── Register.cshtml       # Trang đăng ký

Controllers/
└── HomeController.cs     # Controller xử lý authentication

Models/
└── FirebaseUserModel.cs  # Model cho Firebase user
```

## Cài đặt Firebase

### 1. Tạo Firebase Project
1. Truy cập [Firebase Console](https://console.firebase.google.com/)
2. Tạo project mới
3. Bật Authentication và chọn Google provider

### 2. Cấu hình Firebase
Thay thế cấu hình Firebase trong file `Login.cshtml` và `Register.cshtml`:

```javascript
const firebaseConfig = {
    apiKey: "YOUR_API_KEY",
    authDomain: "YOUR_AUTH_DOMAIN",
    projectId: "YOUR_PROJECT_ID",
    storageBucket: "YOUR_STORAGE_BUCKET",
    messagingSenderId: "YOUR_MESSAGING_SENDER_ID",
    appId: "YOUR_APP_ID"
};
```

### 3. Cấu hình Domain
Thêm domain của website vào Firebase Console > Authentication > Settings > Authorized domains.

## Sử dụng

### 1. Đăng nhập thông thường
1. Truy cập `/Home/Login`
2. Nhập email và mật khẩu
3. Click "Đăng nhập"

### 2. Đăng nhập bằng Google
1. Truy cập `/Home/Login`
2. Click "Đăng nhập bằng Google"
3. Chọn tài khoản Google

### 3. Đăng ký
1. Truy cập `/Home/Register`
2. Điền đầy đủ thông tin
3. Đảm bảo mật khẩu đủ mạnh
4. Click "Đăng ký"

### 4. Đăng xuất
1. Click vào tên người dùng trên navigation
2. Chọn "Đăng xuất"

## Database Schema

### User Table
```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Email NVARCHAR(255) UNIQUE,
    Password NVARCHAR(255),
    IsAdmin BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    DisplayName NVARCHAR(255),
    Provider NVARCHAR(50),
    FirebaseUid NVARCHAR(255)
);
```

## Security Features

### 1. Password Hashing
- Sử dụng MD5 hash cho mật khẩu (có thể nâng cấp lên bcrypt)
- Validation độ dài mật khẩu tối thiểu

### 2. CSRF Protection
- Sử dụng `@Html.AntiForgeryToken()` để chống CSRF attacks

### 3. Input Validation
- Server-side validation
- Client-side validation với HTML5
- Real-time password strength checking

### 4. Session Management
- Lưu thông tin người dùng an toàn trong session
- Tự động clear session khi logout

## Customization

### 1. Thay đổi giao diện
Các trang authentication sử dụng style chung của dự án:
- Hero section với background image
- Bootstrap classes cho form styling
- Consistent color scheme và typography
- Responsive design với Bootstrap grid system

### 2. Thêm providers khác
Để thêm Facebook, Twitter, etc.:
1. Cấu hình provider trong Firebase
2. Thêm button trong view
3. Xử lý callback trong controller

### 3. Email verification
Để thêm email verification:
1. Tạo email service
2. Thêm field `EmailVerified` trong database
3. Gửi email verification khi đăng ký

## Troubleshooting

### 1. Firebase không hoạt động
- Kiểm tra cấu hình Firebase
- Đảm bảo domain được authorize
- Kiểm tra console browser để xem lỗi

### 2. Session không lưu
- Kiểm tra cấu hình session trong Web.config
- Đảm bảo database connection

### 3. Validation không hoạt động
- Kiểm tra JavaScript console
- Đảm bảo jQuery được load
- Kiểm tra HTML5 validation

## Performance Tips

1. **Minify CSS/JS**: Nén file CSS và JavaScript
2. **CDN**: Sử dụng CDN cho Firebase SDK
3. **Caching**: Cache static resources
4. **Database indexing**: Index các field thường query

## Future Enhancements

1. **Two-factor authentication**
2. **Password reset functionality**
3. **Social login providers** (Facebook, Twitter)
4. **Email verification**
5. **Account lockout** sau nhiều lần đăng nhập sai
6. **Remember me** functionality
7. **Profile management**

## Support

Nếu có vấn đề, vui lòng:
1. Kiểm tra console browser
2. Kiểm tra server logs
3. Đảm bảo tất cả dependencies được cài đặt
4. Kiểm tra cấu hình database
