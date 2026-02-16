# PaymentSystem API Hujjatlari

Bu hujjat barcha API endpoint'lar, request/response formatlar va xato kodlarini batafsil tushuntiradi.

## 📍 Base URL

```
Development: http://localhost:5000 yoki https://localhost:5001
Production: https://api.yoursite.com
```

## 🔐 Authentication

**Hozirda:** Authentication yo'q (ochiq API)

**Kelajakda:** JWT Bearer token qo'shish mumkin:
```
Authorization: Bearer <token>
```

## 📡 Endpoints

### 1. Users - Foydalanuvchilar

#### 1.1 Ro'yxatdan o'tish

Yangi foydalanuvchi yaratish.

**Endpoint:** `POST /api/users/signup`

**Content-Type:** `application/json`

**Request Body:**
```json
{
  "fullName": "string",      // 2-200 ta belgi
  "phoneNumber": "string",   // +998 XX XXX XX XX formati
  "tariff": "string"         // max 100 ta belgi
}
```

**Example Request:**
```json
{
  "fullName": "Oybek Nuriddinov",
  "phoneNumber": "+998 90 123 45 67",
  "tariff": "Premium"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Foydalanuvchi muvaffaqiyatli ro'yxatdan o'tdi",
  "data": {
    "id": 1,
    "fullName": "Oybek Nuriddinov",
    "phoneNumber": "+998901234567",
    "tariff": "Premium",
    "createdAt": "2024-02-16T10:30:00.000Z"
  },
  "errors": []
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Validatsiya xatosi",
  "data": null,
  "errors": [
    "Ism kiritilishi shart",
    "Telefon raqam formati noto'g'ri. Format: +998 XX XXX XX XX"
  ]
}
```

**Possible Errors:**
- Ism bo'sh
- Telefon raqam formati noto'g'ri
- Tarif bo'sh
- Telefon raqam allaqachon ro'yxatdan o'tgan

---

#### 1.2 Barcha Foydalanuvchilarni Olish

Ro'yxatdan o'tgan barcha foydalanuvchilarni pagination bilan olish.

**Endpoint:** `GET /api/users`

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| pageNumber | integer | 1 | Sahifa raqami (min: 1) |
| pageSize | integer | 10 | Sahifadagi elementlar soni (max: 100) |

**Example Request:**
```
GET /api/users?pageNumber=1&pageSize=20
```

**Success Response (200 OK):**
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
        "createdAt": "2024-02-16T10:30:00.000Z"
      },
      {
        "id": 2,
        "fullName": "Test User",
        "phoneNumber": "+998912345678",
        "tariff": "Basic",
        "createdAt": "2024-02-16T11:00:00.000Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 2,
    "totalPages": 1,
    "hasPrevious": false,
    "hasNext": false
  },
  "errors": []
}
```

**Pagination Ma'lumotlari:**
- `items` - Joriy sahifadagi foydalanuvchilar
- `pageNumber` - Joriy sahifa raqami
- `pageSize` - Sahifadagi elementlar soni
- `totalCount` - Umumiy foydalanuvchilar soni
- `totalPages` - Umumiy sahifalar soni
- `hasPrevious` - Oldingi sahifa bormi?
- `hasNext` - Keyingi sahifa bormi?

---

### 2. Payments - To'lovlar

#### 2.1 To'lov Qilish

Yangi to'lov yaratish (check fayli bilan).

**Endpoint:** `POST /api/payments/pay`

**Content-Type:** `multipart/form-data`

**Form Fields:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| fullName | string | ✅ | 2-200 ta belgi |
| phoneNumber | string | ✅ | +998 XX XXX XX XX |
| tariff | string | ✅ | max 100 ta belgi |
| checkFile | file | ✅ | PDF/PNG/JPG/JPEG, max 5MB |

**Example Request (cURL):**
```bash
curl -X POST "http://localhost:5000/api/payments/pay" \
  -H "Content-Type: multipart/form-data" \
  -F "fullName=Oybek Nuriddinov" \
  -F "phoneNumber=+998 90 123 45 67" \
  -F "tariff=Premium" \
  -F "checkFile=@/path/to/receipt.pdf"
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "To'lov muvaffaqiyatli qayd etildi",
  "data": {
    "id": 1,
    "fullName": "Oybek Nuriddinov",
    "phoneNumber": "+998901234567",
    "tariff": "Premium",
    "checkFileName": "abc123-def456-ghi789.pdf",
    "createdAt": "2024-02-16T10:35:00.000Z"
  },
  "errors": []
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Validatsiya xatosi",
  "data": null,
  "errors": [
    "Check fayli kiritilishi shart",
    "Check fayli hajmi 5MB dan oshmasligi kerak",
    "Faqat PDF, PNG, JPG, JPEG formatdagi fayllar qabul qilinadi"
  ]
}
```

**File Restrictions:**
- ✅ Allowed formats: PDF, PNG, JPG, JPEG
- ✅ Max size: 5MB
- ❌ Other formats: rejected

**File Storage:**
- Files saved to: `wwwroot/uploads/`
- Naming pattern: `{GUID}.{extension}`
- Example: `abc123-def456-ghi789.pdf`

---

#### 2.2 Barcha To'lovlarni Olish

Barcha to'lovlarni pagination bilan olish.

**Endpoint:** `GET /api/payments`

**Query Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| pageNumber | integer | 1 | Sahifa raqami (min: 1) |
| pageSize | integer | 10 | Sahifadagi elementlar soni (max: 100) |

**Example Request:**
```
GET /api/payments?pageNumber=1&pageSize=10
```

**Success Response (200 OK):**
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
        "createdAt": "2024-02-16T10:35:00.000Z"
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

---

## 🔒 Validatsiya Qoidalari

### FullName (Ism)
- ✅ Minimum: 2 ta belgi
- ✅ Maximum: 200 ta belgi
- ❌ Bo'sh bo'lmasligi kerak
- ✅ Har qanday belgilar (lotin, kirill)

### PhoneNumber (Telefon)
- ✅ Format: `+998 XX XXX XX XX`
- ✅ Alternativ: `+998XXXXXXXXX` (bo'sh joysiz)
- ❌ Boshqa formatlar qabul qilinmaydi
- 📝 Saqlashda: bo'sh joylar olib tashlanadi

**To'g'ri formatlar:**
```
✅ +998 90 123 45 67
✅ +998 91 234 56 78
✅ +998901234567
```

**Noto'g'ri formatlar:**
```
❌ 998901234567      (+ belgisi yo'q)
❌ +998 9012345678   (format xato)
❌ 901234567         (kod yo'q)
❌ +7 901 234 5678   (boshqa mamlakat)
```

### Tariff (Tarif)
- ✅ Maximum: 100 ta belgi
- ❌ Bo'sh bo'lmasligi kerak
- ✅ Har qanday string (Premium, Basic, Enterprise, ...)

### CheckFile (Check Fayli)
- ✅ Formats: PDF, PNG, JPG, JPEG
- ✅ Max size: 5MB (5,242,880 bytes)
- ❌ Bo'sh fayl qabul qilinmaydi
- ❌ Boshqa formatlar qabul qilinmaydi

---

## 📊 Response Formatlari

### Generic Response Wrapper

Barcha response'lar quyidagi formatda qaytadi:

```typescript
interface ApiResponse<T> {
  success: boolean;      // Muvaffaqiyatmi?
  message: string;       // Xabar
  data: T | null;        // Ma'lumot (success: true da)
  errors: string[];      // Xatolar ro'yxati (success: false da)
}
```

### Pagination Response

```typescript
interface PagedList<T> {
  items: T[];           // Joriy sahifadagi elementlar
  pageNumber: number;   // Joriy sahifa
  pageSize: number;     // Sahifa hajmi
  totalCount: number;   // Umumiy elementlar
  totalPages: number;   // Umumiy sahifalar
  hasPrevious: boolean; // Oldingi sahifa bormi?
  hasNext: boolean;     // Keyingi sahifa bormi?
}
```

---

## ⚠️ Xato Kodlari va Xabarlar

### HTTP Status Codes

| Code | Meaning | When |
|------|---------|------|
| 200 | OK | Muvaffaqiyatli so'rov |
| 400 | Bad Request | Validatsiya xatosi |
| 500 | Internal Server Error | Server xatosi |

### Xato Xabarlari (Uzbek)

**User Errors:**
- "Ism kiritilishi shart"
- "Ism kamida 2 ta belgidan iborat bo'lishi kerak"
- "Ism 200 ta belgidan oshmasligi kerak"
- "Telefon raqam kiritilishi shart"
- "Telefon raqam formati noto'g'ri. Format: +998 XX XXX XX XX"
- "Tarif kiritilishi shart"
- "Tarif nomi 100 ta belgidan oshmasligi kerak"
- "Bu telefon raqam bilan foydalanuvchi allaqachon ro'yxatdan o'tgan"

**Payment Errors:**
- "Check fayli kiritilishi shart"
- "Check fayli bo'sh bo'lmasligi kerak"
- "Check fayli hajmi 5MB dan oshmasligi kerak"
- "Faqat PDF, PNG, JPG, JPEG formatdagi fayllar qabul qilinadi"
- "Check fayli yuklanmagan"

---

## 🧪 Postman Collection

### Sign Up Request

```json
POST {{base_url}}/api/users/signup
Content-Type: application/json

{
  "fullName": "{{$randomFullName}}",
  "phoneNumber": "+998 90 {{$randomInt}}",
  "tariff": "Premium"
}
```

### Get Users Request

```
GET {{base_url}}/api/users?pageNumber=1&pageSize=10
```

### Pay Request

```
POST {{base_url}}/api/payments/pay
Content-Type: multipart/form-data

fullName: Test User
phoneNumber: +998 90 123 45 67
tariff: Basic
checkFile: [file]
```

### Environment Variables

```json
{
  "base_url": "http://localhost:5000"
}
```

---

## 📝 Misollar

### JavaScript (Fetch API)

```javascript
// Sign Up
async function signUp() {
  const response = await fetch('http://localhost:5000/api/users/signup', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      fullName: 'Oybek Nuriddinov',
      phoneNumber: '+998 90 123 45 67',
      tariff: 'Premium'
    })
  });
  
  const data = await response.json();
  console.log(data);
}

// Pay with File
async function pay() {
  const formData = new FormData();
  formData.append('fullName', 'Oybek Nuriddinov');
  formData.append('phoneNumber', '+998 90 123 45 67');
  formData.append('tariff', 'Premium');
  formData.append('checkFile', fileInput.files[0]);
  
  const response = await fetch('http://localhost:5000/api/payments/pay', {
    method: 'POST',
    body: formData
  });
  
  const data = await response.json();
  console.log(data);
}
```

### C# (HttpClient)

```csharp
// Sign Up
var client = new HttpClient();
var content = new StringContent(
    JsonSerializer.Serialize(new {
        fullName = "Oybek Nuriddinov",
        phoneNumber = "+998 90 123 45 67",
        tariff = "Premium"
    }),
    Encoding.UTF8,
    "application/json"
);

var response = await client.PostAsync(
    "http://localhost:5000/api/users/signup",
    content
);

var result = await response.Content.ReadAsStringAsync();
```

### Python (Requests)

```python
import requests

# Sign Up
response = requests.post(
    'http://localhost:5000/api/users/signup',
    json={
        'fullName': 'Oybek Nuriddinov',
        'phoneNumber': '+998 90 123 45 67',
        'tariff': 'Premium'
    }
)

print(response.json())

# Pay with file
files = {'checkFile': open('receipt.pdf', 'rb')}
data = {
    'fullName': 'Oybek Nuriddinov',
    'phoneNumber': '+998 90 123 45 67',
    'tariff': 'Premium'
}

response = requests.post(
    'http://localhost:5000/api/payments/pay',
    files=files,
    data=data
)

print(response.json())
```

---

## 🔄 Versiyalash

**Joriy versiya:** v1.0.0

Kelajakda API versiyalari qo'shilishi mumkin:
```
/api/v1/users/signup
/api/v2/users/signup
```

---

## 📞 Qo'llab-quvvatlash

Savollar yoki muammolar bo'lsa:
- GitHub Issues: [link]
- Email: support@example.com
- Telegram: @username
