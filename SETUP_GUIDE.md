# PaymentSystem - O'rnatish Qo'llanmasi

Bu qo'llanma sizga loyihani boshlang'ich holatdan to ishga tushirish uchun zarur barcha qadamlarni ko'rsatadi.

## 📋 Kerakli Dasturlar

Quyidagi dasturlar kompyuteringizda o'rnatilgan bo'lishi kerak:

1. **.NET 8.0 SDK** 
   - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Tekshirish: `dotnet --version`

2. **PostgreSQL 14+**
   - [Download](https://www.postgresql.org/download/)
   - Tekshirish: `psql --version`

3. **IDE (birini tanlang)**
   - [Visual Studio 2022](https://visualstudio.microsoft.com/) (tavsiya etiladi)
   - [JetBrains Rider](https://www.jetbrains.com/rider/)
   - [VS Code](https://code.visualstudio.com/) + C# extension

## 🚀 Qadam-baqadam O'rnatish

### Qadam 1: Repository ni Klonlash yoki Yuklab Olish

```bash
# Agar Git orqali klonlasangiz:
git clone https://github.com/username/PaymentSystem.git
cd PaymentSystem

# Yoki ZIP faylni yuklab olib extract qiling
```

### Qadam 2: PostgreSQL Database Yaratish

1. PostgreSQL ga kiring:
```bash
psql -U postgres
```

2. Yangi database yarating:
```sql
CREATE DATABASE PaymentSystemDb;
```

3. Database yaratilganligini tekshiring:
```sql
\l
```

4. Chiqish:
```sql
\q
```

### Qadam 3: Connection String Sozlash

1. `PaymentSystem.API/appsettings.json` faylini oching

2. O'z ma'lumotlaringizni kiriting:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PaymentSystemDb;Username=postgres;Password=SIZNING_PAROLINGIZ"
  }
}
```

**Diqqat:** Password ni o'z PostgreSQL parolingiz bilan almashtiring!

### Qadam 4: NuGet Package'larni Restore Qilish

```bash
# Loyiha root papkasida:
dotnet restore
```

Yoki Visual Studio da:
- Solution Explorer da solution'ga right-click
- "Restore NuGet Packages" ni tanlang

### Qadam 5: Entity Framework Migration

Terminal da quyidagi buyruqlarni ketma-ket bajaring:

```bash
# API papkasiga o'ting
cd PaymentSystem.API

# Migration yaratish
dotnet ef migrations add InitialCreate --project ../PaymentSystem.DAL

# Database'ga apply qilish
dotnet ef database update --project ../PaymentSystem.DAL
```

**Agar "dotnet ef" tool topilmasa:**
```bash
dotnet tool install --global dotnet-ef
```

### Qadam 6: Loyihani Ishga Tushirish

**A. Command Line orqali:**
```bash
cd PaymentSystem.API
dotnet run
```

**B. Visual Studio orqali:**
1. Solution'ni oching (PaymentSystem.sln)
2. F5 bosing yoki "Start Debugging" tugmasini bosing

**C. Rider orqali:**
1. Solution'ni oching
2. PaymentSystem.API ni startup project qilib tanlang
3. Run tugmasini bosing

### Qadam 7: Swagger UI ni Ochish

Loyiha ishga tushgandan keyin brauzerda quyidagi manzilni oching:
```
https://localhost:5001/swagger
```

Yoki HTTP versiyasi:
```
http://localhost:5000/swagger
```

## ✅ Test Qilish

### 1. Sign Up endpoint'ini test qilish

Swagger UI da:
1. `POST /api/users/signup` ni toping
2. "Try it out" tugmasini bosing
3. Quyidagi JSON'ni kiriting:

```json
{
  "fullName": "Test Foydalanuvchi",
  "phoneNumber": "+998 90 123 45 67",
  "tariff": "Premium"
}
```

4. "Execute" tugmasini bosing
5. Response 200 OK bo'lishi kerak

### 2. Get Users endpoint'ini test qilish

1. `GET /api/users` ni toping
2. "Try it out" tugmasini bosing
3. PageNumber: 1, PageSize: 10 kiriting
4. "Execute" tugmasini bosing
5. Siz yaratgan foydalanuvchi ko'rinishi kerak

### 3. Pay endpoint'ini test qilish

1. `POST /api/payments/pay` ni toping
2. "Try it out" tugmasini bosing
3. Form field'larni to'ldiring:
   - fullName: Test User
   - phoneNumber: +998 90 123 45 67
   - tariff: Basic
   - checkFile: PDF yoki rasm fayl tanlang (max 5MB)
4. "Execute" tugmasini bosing

## 🔧 Umumiy Muammolar va Yechimlar

### Muammo 1: "dotnet ef" topilmadi

**Yechim:**
```bash
dotnet tool install --global dotnet-ef
```

### Muammo 2: PostgreSQL ga ulanib bo'lmayapti

**Yechim:**
1. PostgreSQL service ishlab turganligini tekshiring:
   - Windows: Services.msc > PostgreSQL service
   - Linux: `sudo systemctl status postgresql`
   - Mac: `brew services list`

2. Connection string to'g'riligini tekshiring
3. Username va password to'g'riligini tekshiring

### Muammo 3: Port 5000/5001 band

**Yechim:**
`PaymentSystem.API/Properties/launchSettings.json` da portni o'zgartiring:
```json
"applicationUrl": "https://localhost:5002;http://localhost:5003"
```

### Muammo 4: HTTPS certificate muammosi

**Yechim:**
```bash
dotnet dev-certs https --trust
```

### Muammo 5: Migration xatosi

**Yechim:**
1. Avvalgi migration'larni o'chiring:
```bash
cd PaymentSystem.API
dotnet ef database drop --project ../PaymentSystem.DAL
dotnet ef migrations remove --project ../PaymentSystem.DAL
```

2. Qaytadan migration yarating:
```bash
dotnet ef migrations add InitialCreate --project ../PaymentSystem.DAL
dotnet ef database update --project ../PaymentSystem.DAL
```

## 📦 Qo'shimcha Ma'lumotlar

### Upload Papkasi

Yuklangan fayllar shu yerda saqlanadi:
```
PaymentSystem.API/wwwroot/uploads/
```

Bu papka avtomatik yaratiladi, lekin agar muammo bo'lsa qo'lda yarating.

### Database Schema

Migration'dan keyin database'da 2 ta table yaratiladi:
- `Users` - Ro'yxatdan o'tgan foydalanuvchilar
- `Payments` - To'lovlar

### Logging

Console'da loglar ko'rinadi. Agar batafsil log kerak bo'lsa, `appsettings.Development.json` da o'zgartiring:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

## 🎯 Keyingi Qadamlar

Loyiha ishga tushdi! Endi:

1. ✅ API endpoint'larini Postman/Swagger orqali test qiling
2. ✅ Frontend yarating (React, Vue, Angular)
3. ✅ Authentication qo'shing (JWT)
4. ✅ Production'ga deploy qiling

## 📞 Yordam Kerakmi?

Agar muammo yuzaga kelsa:
1. README.md faylini o'qing
2. Error message'larni diqqat bilan o'qing
3. Stack Overflow'da qidiring
4. GitHub Issues'da muammo yarating

## ✅ Checklist

Loyiha to'g'ri ishlashi uchun:

- [ ] .NET 8.0 SDK o'rnatilgan
- [ ] PostgreSQL o'rnatilgan va ishlab turibdi
- [ ] Database yaratilgan (PaymentSystemDb)
- [ ] Connection string to'g'ri sozlangan
- [ ] Migration muvaffaqiyatli bajarilgan
- [ ] Loyiha kompilyatsiya bo'ladi (dotnet build)
- [ ] Loyiha ishga tushadi (dotnet run)
- [ ] Swagger UI ochiladi
- [ ] Sign Up endpoint ishlaydi
- [ ] Pay endpoint ishlaydi (file upload)

Barcha checkbox'lar belgilangan bo'lsa - tabriklaymiz! 🎉
