# Payment System WebAPI

O'zbekiston uchun to'lov tizimi WebAPI - Ro'yxatdan o'tish va to'lovlarni qayd etish uchun.

## 📋 Texnologiyalar

- **.NET 8.0** - Backend framework
- **PostgreSQL** - Database
- **Entity Framework Core** - ORM
- **FluentValidation** - Validatsiya
- **Swagger/OpenAPI** - API Documentation

## 🏗️ Arxitektura

Loyiha Clean Architecture prinsiplariga asoslanib quyidagi layer'larga bo'lingan:

```
PaymentSystem/
├── PaymentSystem.Domain/       # Entities (User, Payment)
├── PaymentSystem.Common/       # DTOs, Responses, Helpers
├── PaymentSystem.DAL/          # Data Access Layer (Repositories, DbContext)
├── PaymentSystem.BLL/          # Business Logic Layer (Services, Validators)
└── PaymentSystem.API/          # Web API (Controllers, Middleware)
```

## 🚀 O'rnatish va Ishga Tushirish

### 1. PostgreSQL ni o'rnatish

```bash
# PostgreSQL o'rnatilganligini tekshiring
psql --version
```

### 2. Database yaratish

```sql
CREATE DATABASE PaymentSystemDb;
```

### 3. Connection String sozlash

`PaymentSystem.API/appsettings.json` faylida o'z ma'lumotlaringizni kiriting:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PaymentSystemDb;Username=postgres;Password=sizning_parolingiz"
  }
}
```

### 4. Dependency Install qilish

```bash
cd PaymentSystem
dotnet restore
```

### 5. Database Migration

```bash
cd PaymentSystem.API
dotnet ef migrations add InitialCreate --project ../PaymentSystem.DAL
dotnet ef database update --project ../PaymentSystem.DAL
```

### 6. Loyihani ishga tushirish

```bash
cd PaymentSystem.API
dotnet run
```

Yoki Visual Studio/Rider da F5 bosing.

API endi quyidagi manzilda ishlaydi:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger

## 📡 API Endpoints

### 1. Sign Up (Ro'yxatdan o'tish)

**POST** `/api/users/signup`

**Request Body:**
```json
{
  "fullName": "Oybek Nuriddinov",
  "phoneNumber": "+998 90 123 45 67",
  "tariff": "Premium"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Foydalanuvchi muvaffaqiyatli ro'yxatdan o'tdi",
  "data": {
    "id": 1,
    "fullName": "Oybek Nuriddinov",
    "phoneNumber": "+998901234567",
    "tariff": "Premium",
    "createdAt": "2024-02-16T10:30:00Z"
  },
  "errors": []
}
```

### 2. Get All Users (Pagination bilan)

**GET** `/api/users?pageNumber=1&pageSize=10`

**Response:**
```json
{
  "success": true,
  "message": "Foydalanuvchilar ro'yxati",
  "data": {
    "items": [
      {
        "id": 1,
        "fullName": "Oybek Nuriddinov",
        "phoneNumber": "+998901234567",
        "tariff": "Premium",
        "createdAt": "2024-02-16T10:30:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1,
    "totalPages": 1,
    "hasPrevious": false,
    "hasNext": false
  },
  "errors": []
}
```

### 3. Pay (To'lov qilish)

**POST** `/api/payments/pay`

**Content-Type:** `multipart/form-data`

**Form Fields:**
- `fullName`: string
- `phoneNumber`: string
- `tariff`: string
- `checkFile`: file (PDF/PNG/JPG, max 5MB)

**Response:**
```json
{
  "success": true,
  "message": "To'lov muvaffaqiyatli qayd etildi",
  "data": {
    "id": 1,
    "fullName": "Oybek Nuriddinov",
    "phoneNumber": "+998901234567",
    "tariff": "Premium",
    "checkFileName": "abc123-def456.pdf",
    "createdAt": "2024-02-16T10:35:00Z"
  },
  "errors": []
}
```

### 4. Get All Payments (Pagination bilan)

**GET** `/api/payments?pageNumber=1&pageSize=10`

**Response:**
```json
{
  "success": true,
  "message": "To'lovlar ro'yxati",
  "data": {
    "items": [
      {
        "id": 1,
        "fullName": "Oybek Nuriddinov",
        "phoneNumber": "+998901234567",
        "tariff": "Premium",
        "checkFileName": "abc123-def456.pdf",
        "createdAt": "2024-02-16T10:35:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1,
    "totalPages": 1,
    "hasPrevious": false,
    "hasNext": false
  },
  "errors": []
}
```

## ✅ Validatsiya Qoidalari

### SignUp uchun:
- **FullName**: 2-200 ta belgi, bo'sh bo'lmasligi kerak
- **PhoneNumber**: O'zbekiston formati (+998 XX XXX XX XX)
- **Tariff**: Bo'sh bo'lmasligi kerak, max 100 ta belgi

### Pay uchun:
- **FullName**: 2-200 ta belgi, bo'sh bo'lmasligi kerak
- **PhoneNumber**: O'zbekiston formati (+998 XX XXX XX XX)
- **Tariff**: Bo'sh bo'lmasligi kerak, max 100 ta belgi
- **CheckFile**: 
  - Faqat PDF, PNG, JPG, JPEG formatlar
  - Maksimum hajm: 5MB
  - Bo'sh bo'lmasligi kerak

## 📁 File Upload

Yuklangan fayllar quyidagi papkaga saqlanadi:
```
PaymentSystem.API/wwwroot/uploads/
```

Fayllar unique GUID nomi bilan saqlanadi, masalan:
```
abc123-def456-ghi789.pdf
```

## 🧪 Postman bilan Test qilish

### 1. Sign Up test

```bash
POST http://localhost:5000/api/users/signup
Content-Type: application/json

{
  "fullName": "Test User",
  "phoneNumber": "+998 90 123 45 67",
  "tariff": "Basic"
}
```

### 2. Pay test (Form-data)

```bash
POST http://localhost:5000/api/payments/pay
Content-Type: multipart/form-data

fullName: Test User
phoneNumber: +998 90 123 45 67
tariff: Basic
checkFile: [PDF/Image fayl tanlang]
```

## 🔧 O'zgartirishlar

### Database Connection String
`PaymentSystem.API/appsettings.json`

### Upload papkasini o'zgartirish
`PaymentSystem.API/Program.cs` da quyidagi qatorni o'zgartiring:
```csharp
var uploadPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
```

### Pagination default qiymatlarini o'zgartirish
`PaymentSystem.Common/Helpers/PaginationParams.cs`

### File size limitni o'zgartirish
`PaymentSystem.BLL/Validators/PayDtoValidator.cs`

## 🛠️ Maslahatlar

1. **Development rejimida** ishlatish uchun HTTPS sertifikatini trust qiling:
```bash
dotnet dev-certs https --trust
```

2. **Database migration** yaratish:
```bash
dotnet ef migrations add YourMigrationName --project PaymentSystem.DAL
```

3. **Loglarni** ko'rish uchun appsettings.Development.json da LogLevel ni o'zgartiring

4. **CORS** sozlamalari Program.cs da "AllowAll" policy bilan configured

## 📦 Production ga Deploy qilish

1. appsettings.Production.json yarating
2. Production database connection string ni kiriting
3. Publish qiling:
```bash
dotnet publish -c Release -o ./publish
```

## 🤝 Hissa qo'shish

Pull request yuborishdan oleh issue oching va muhokama qiling.

## 📄 License

MIT License

## 👨‍💻 Muallif

Sizning ismingiz

## 📞 Aloqa

Email: your.email@example.com
GitHub: https://github.com/yourusername
