# Server Deployment Qo'llanmasi

Bu qo'llanma PaymentSystem API ni Contabo VPS serverga deploy qilish uchun.

## 📋 Server Ma'lumotlari

- **IP:** 178.18.254.129
- **User:** root
- **Password:** ibos2025
- **Domain:** api.ibos.uz
- **OS:** Linux (Hestia Panel bilan)

## 🚀 Deploy Qilish (2 ta variant)

### Variant 1: Avtomatik Script (Tavsiya etiladi) ✅

#### 1-qadam: Serverga Kirish

```bash
ssh root@178.18.254.129
# Password: ibos2025
```

#### 2-qadam: GitHub Repository Yaratish

Bu qadamni **o'z kompyuteringizda** bajaring:

1. GitHub.com ga kiring
2. New Repository yarating:
   - Name: `PaymentSystem`
   - Description: "Payment System API for ibos.uz"
   - Private yoki Public (ixtiyoriy)
3. Repository URL ni nusxalang (masalan: `https://github.com/username/PaymentSystem.git`)

#### 3-qadam: Loyihani GitHub ga Push Qilish

**O'z kompyuteringizda** PaymentSystem papkasida:

```bash
cd /path/to/PaymentSystem

# Git init (agar qilinmagan bo'lsa)
git init
git add -A
git commit -m "Initial commit: Payment System API"

# GitHub ga push
git remote add origin https://github.com/YOUR_USERNAME/PaymentSystem.git
git branch -M main
git push -u origin main
```

#### 4-qadam: Deploy Script ni Serverga Yuklash

**Serverda** (SSH orqali):

```bash
# Deploy script ni yuklab olish
cd /root
wget https://github.com/YOUR_USERNAME/PaymentSystem/raw/main/deploy.sh

# Yoki manual yaratish:
nano deploy.sh
# (Script matnini paste qiling va Ctrl+X, Y, Enter)

# Execute ruxsatini berish
chmod +x deploy.sh
```

#### 5-qadam: Script ni O'zgartirish

Script ichidagi GitHub URL ni o'zgartiring:

```bash
nano deploy.sh
```

Quyidagi qatorni toping va o'z repository URL ingiz bilan almashtiring:

```bash
# Bu qismni toping:
echo "Cloning repository..."
# TODO: Replace with actual GitHub URL when created

# Va quyidagiga o'zgartiring:
git clone https://github.com/YOUR_USERNAME/PaymentSystem.git $DEPLOY_DIR/source
```

Save qiling (Ctrl+X, Y, Enter)

#### 6-qadam: Deploy ni Boshlash

```bash
bash deploy.sh
```

Script avtomatik quyidagilarni bajaradi:
- ✅ .NET 8 SDK o'rnatadi
- ✅ PostgreSQL database yaratadi  
- ✅ Loyihani GitHub dan clone qiladi
- ✅ Build va publish qiladi
- ✅ Systemd service yaratadi
- ✅ Nginx reverse proxy sozlaydi
- ✅ SSL certificate o'rnatadi
- ✅ API ni ishga tushiradi

---

### Variant 2: Manual Deployment (Qadam-ba-qadam)

Agar script ishlamasa, manual quyidagicha qiling:

#### 1. .NET 8 SDK O'rnatish

```bash
# SSH orqali serverga kiring
ssh root@178.18.254.129

# Microsoft repository qo'shish
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# .NET SDK o'rnatish
apt-get update
apt-get install -y dotnet-sdk-8.0

# Tekshirish
dotnet --version
# Output: 8.0.x
```

#### 2. PostgreSQL Database Yaratish

```bash
# PostgreSQL ga kirish
sudo -u postgres psql

# Database va user yaratish
CREATE USER payment_user WITH PASSWORD 'PaymentSecure2025!';
CREATE DATABASE payment_system_db OWNER payment_user;
GRANT ALL PRIVILEGES ON DATABASE payment_system_db TO payment_user;

# Chiqish
\q
```

#### 3. Loyihani Serverga Yuklash

**Variant A: GitHub orqali (tavsiya)**

```bash
# Deploy papkasini yaratish
mkdir -p /var/www/payment-system
cd /var/www/payment-system

# Git clone (o'z URL ingizni kiriting)
git clone https://github.com/YOUR_USERNAME/PaymentSystem.git source
cd source
```

**Variant B: SCP orqali (to'g'ridan)**

**O'z kompyuteringizda:**

```bash
# ZIP ni serverga yuklash
scp PaymentSystem.zip root@178.18.254.129:/root/

# Serverda extract qilish
ssh root@178.18.254.129
cd /root
unzip PaymentSystem.zip
mv PaymentSystem /var/www/payment-system/source
```

#### 4. Build va Publish

```bash
cd /var/www/payment-system/source

# Restore packages
dotnet restore

# Publish
dotnet publish PaymentSystem.API/PaymentSystem.API.csproj \
  -c Release \
  -o /var/www/payment-system/app
```

#### 5. Production Settings

```bash
# appsettings.Production.json yaratish
cat > /var/www/payment-system/app/appsettings.Production.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=payment_system_db;Username=payment_user;Password=PaymentSecure2025!"
  }
}
EOF

chmod 600 /var/www/payment-system/app/appsettings.Production.json
```

#### 6. Database Migration

```bash
cd /var/www/payment-system/source

# Migration yaratish
dotnet ef migrations add InitialCreate --project PaymentSystem.DAL --startup-project PaymentSystem.API

# Database'ga apply qilish
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=payment_system_db;Username=payment_user;Password=PaymentSecure2025!"

dotnet ef database update --project PaymentSystem.DAL --startup-project PaymentSystem.API
```

#### 7. Systemd Service Yaratish

```bash
# Service file yaratish
cat > /etc/systemd/system/payment-system.service << 'EOF'
[Unit]
Description=Payment System API Service
After=network.target

[Service]
Type=notify
User=www-data
Group=www-data
WorkingDirectory=/var/www/payment-system/app
ExecStart=/usr/bin/dotnet /var/www/payment-system/app/PaymentSystem.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=payment-system
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF

# Permissions
chown -R www-data:www-data /var/www/payment-system
mkdir -p /var/www/payment-system/wwwroot/uploads
chmod 777 /var/www/payment-system/wwwroot/uploads

# Service ni enable va start qilish
systemctl daemon-reload
systemctl enable payment-system
systemctl start payment-system

# Status tekshirish
systemctl status payment-system
```

#### 8. Nginx Configuration

```bash
# Nginx o'rnatish (agar yo'q bo'lsa)
apt-get install -y nginx

# Config file yaratish
cat > /etc/nginx/sites-available/api.ibos.uz << 'EOF'
upstream payment_api {
    server localhost:5000;
    keepalive 32;
}

server {
    listen 80;
    server_name api.ibos.uz;

    location / {
        return 301 https://$server_name$request_uri;
    }
}

server {
    listen 443 ssl http2;
    server_name api.ibos.uz;

    # SSL (certbot tomonidan sozlanadi)
    
    client_max_body_size 5M;

    location / {
        proxy_pass http://payment_api;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }

    location /uploads/ {
        alias /var/www/payment-system/wwwroot/uploads/;
        expires 30d;
    }
}
EOF

# Enable site
ln -sf /etc/nginx/sites-available/api.ibos.uz /etc/nginx/sites-enabled/

# Test config
nginx -t

# Reload nginx
systemctl reload nginx
```

#### 9. SSL Certificate (Let's Encrypt)

```bash
# Certbot o'rnatish
apt-get install -y certbot python3-certbot-nginx

# SSL certificate olish
certbot --nginx -d api.ibos.uz --email admin@ibos.uz --agree-tos --redirect

# Auto-renewal test
certbot renew --dry-run
```

#### 10. DNS Configuration

**Domain provider (Cloudflare/другой)** da:

```
Type: A
Name: api
Value: 178.18.254.129
TTL: Auto
```

---

## ✅ Test Qilish

Deployment tugagandan keyin:

### 1. Service Status

```bash
systemctl status payment-system
```

**Expected:** `active (running)`

### 2. Logs

```bash
# Real-time logs
journalctl -u payment-system -f

# Last 50 lines
journalctl -u payment-system -n 50
```

### 3. API Test

```bash
# Health check
curl http://localhost:5000/health

# Swagger
curl http://localhost:5000/swagger

# Via domain (SSL bilan)
curl https://api.ibos.uz/swagger
```

### 4. Browser Test

Brauzerda oching:
```
https://api.ibos.uz/swagger
```

### 5. Endpoint Test

```bash
# Sign Up test
curl -X POST https://api.ibos.uz/api/users/signup \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Test User",
    "phoneNumber": "+998 90 123 45 67",
    "tariff": "Premium"
  }'

# Get Users
curl https://api.ibos.uz/api/users?pageNumber=1&pageSize=10
```

---

## 🔧 Useful Commands

### Service Management

```bash
# Start
systemctl start payment-system

# Stop
systemctl stop payment-system

# Restart
systemctl restart payment-system

# Status
systemctl status payment-system

# Enable auto-start
systemctl enable payment-system

# Disable auto-start
systemctl disable payment-system
```

### Logs

```bash
# Real-time logs
journalctl -u payment-system -f

# Last 100 lines
journalctl -u payment-system -n 100

# Logs from today
journalctl -u payment-system --since today

# Logs from specific time
journalctl -u payment-system --since "2024-02-16 10:00:00"
```

### Update Deployment

```bash
# Pull latest code
cd /var/www/payment-system/source
git pull

# Rebuild and publish
dotnet publish PaymentSystem.API/PaymentSystem.API.csproj \
  -c Release \
  -o /var/www/payment-system/app

# Run migrations (if needed)
dotnet ef database update --project PaymentSystem.DAL --startup-project PaymentSystem.API

# Restart service
systemctl restart payment-system
```

### Database Backup

```bash
# Create backup
pg_dump -U payment_user payment_system_db > /root/backup_$(date +%Y%m%d_%H%M%S).sql

# Restore backup
psql -U payment_user payment_system_db < /root/backup_20240216_120000.sql
```

---

## ⚠️ Troubleshooting

### Problem 1: Service won't start

```bash
# Check logs
journalctl -u payment-system -n 50

# Check if port 5000 is used
netstat -tulpn | grep 5000

# Check permissions
ls -la /var/www/payment-system/app
```

### Problem 2: Database connection error

```bash
# Test database connection
psql -U payment_user -d payment_system_db -h localhost

# Check connection string in appsettings
cat /var/www/payment-system/app/appsettings.Production.json
```

### Problem 3: Nginx 502 Bad Gateway

```bash
# Check if API is running
curl http://localhost:5000/health

# Check Nginx logs
tail -f /var/log/nginx/error.log

# Restart service
systemctl restart payment-system
systemctl reload nginx
```

### Problem 4: SSL Certificate error

```bash
# Renew certificate
certbot renew

# Force renew
certbot renew --force-renewal

# Check certificate
certbot certificates
```

---

## 📁 Directory Structure

```
/var/www/payment-system/
├── source/                 # Git repository
│   ├── PaymentSystem.API/
│   ├── PaymentSystem.BLL/
│   ├── PaymentSystem.DAL/
│   └── ...
├── app/                    # Published application
│   ├── PaymentSystem.API.dll
│   ├── appsettings.json
│   ├── appsettings.Production.json
│   └── wwwroot/
│       └── uploads/        # Check files (777)
└── logs/                   # Application logs (optional)
```

---

## 🎯 Final Checklist

Deployment tugagandan keyin:

- [ ] .NET SDK o'rnatilgan
- [ ] PostgreSQL database yaratilgan
- [ ] Loyiha GitHub'dan clone qilingan
- [ ] Project build va publish qilingan
- [ ] Production settings sozlangan
- [ ] Database migration bajarilgan
- [ ] Systemd service yaratilgan va ishlamoqda
- [ ] Nginx reverse proxy configured
- [ ] SSL certificate o'rnatilgan
- [ ] DNS A record qo'shilgan
- [ ] Swagger UI ochiladi: https://api.ibos.uz/swagger
- [ ] API ishlayapti: https://api.ibos.uz/api/users
- [ ] File upload ishlayapti

---

**Deployment muvaffaqiyatli! 🎉**

Savollar bo'lsa, scriptdagi YELLOW xabarlarni tekshiring yoki loglarni ko'ring.
