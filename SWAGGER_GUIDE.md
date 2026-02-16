# Swagger UI Foydalanish Qo'llanmasi

Bu qo'llanma Swagger UI orqali API'ni qanday test qilishni ko'rsatadi.

## 🌐 Swagger UI ga Kirish

Loyihani ishga tushirgandan keyin brauzerda oching:

**Development:**
```
https://localhost:5001/swagger
```

Yoki HTTP versiya:
```
http://localhost:5000/swagger
```

## 📋 Swagger UI da Ko'rinadigan Ma'lumotlar

### Har bir endpoint uchun:

✅ **Summary** - Qisqacha tavsif
✅ **Batafsil Description** - To'liq ma'lumot
✅ **Parameters** - Kerakli parametrlar
✅ **Request Body Schema** - Yuborish kerak bo'lgan ma'lumotlar
✅ **Example Values** - Misol qiymatlar
✅ **Response Examples** - Javob misollari
✅ **Validatsiya qoidalari** - Tekshiruvlar
✅ **Limitlar** - Maksimum/minimum qiymatlar

## 🎯 Qanday Test Qilish?

### POST /api/users/signup

1. Endpoint'ni oching
2. "Try it out" bosing
3. Example data'ni o'zgartiring yoki shundayligicha qoldiring
4. "Execute" bosing
5. Response'ni ko'ring

### GET /api/users

1. Endpoint'ni oching
2. "Try it out" bosing  
3. pageNumber va pageSize kiriting
4. "Execute" bosing

### POST /api/payments/pay

1. Endpoint'ni oching
2. "Try it out" bosing
3. Field'larni to'ldiring
4. **"Choose File"** dan PDF/rasm tanlang
5. "Execute" bosing

## 📖 Swagger'da Ko'rinadigan Ma'lumotlar

Har bir endpoint uchun quyidagilar batafsil yozilgan:

### 1. Telefon Format
- ✅ +998 90 123 45 67
- ✅ +998901234567
- ❌ 998901234567

### 2. File Limitlar
- Formatlar: PDF, PNG, JPG, JPEG
- Maksimum: 5MB

### 3. Pagination
- Default: 10
- Maksimum: 100

### 4. Validatsiya Xatolari
Har bir xato uchun o'zbekcha xabar

Batafsil ma'lumot uchun Swagger UI'ni oching!
