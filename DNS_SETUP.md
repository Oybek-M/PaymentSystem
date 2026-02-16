# DNS Configuration - api.ibos.uz

## 🌐 Domain Sozlash

### Cloudflare (yoki boshqa DNS provider) da:

1. **DNS Records** bo'limiga o'ting
2. **Add record** bosing
3. Quyidagi ma'lumotlarni kiriting:

```
Type: A
Name: api
IPv4 address: 178.18.254.129
TTL: Auto (yoki 3600)
Proxy status: DNS only (grey cloud) - SSL uchun zarur!
```

4. **Save** bosing

### Tekshirish

DNS sozlangandan keyin (2-10 daqiqa):

```bash
# Ping test
ping api.ibos.uz

# DNS lookup
nslookup api.ibos.uz

# Expected output:
# Name:   api.ibos.uz
# Address: 178.18.254.129
```

### SSL Certificate uchun

DNS ishlashini kutib, keyin SSL o'rnating:

```bash
# Serverda
certbot --nginx -d api.ibos.uz --email admin@ibos.uz --agree-tos --redirect
```

---

## ✅ DNS Configured

Agar DNS ishlayotgan bo'lsa:

✅ `ping api.ibos.uz` - IP ni ko'rsatadi
✅ `https://api.ibos.uz` - API ochiladi  
✅ `https://api.ibos.uz/swagger` - Swagger ochiladi

---

**DNS sozlash taxminan 5-10 daqiqa vaqt oladi.**
