#!/bin/bash

# PaymentSystem Deploy Script for Contabo VPS
# Run this on your server: bash deploy.sh

set -e  # Exit on error

echo "======================================"
echo "PaymentSystem Deploy Script"
echo "Server: Contabo VPS (ibos.uz)"
echo "======================================"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
PROJECT_NAME="PaymentSystem"
DEPLOY_DIR="/var/www/payment-system"
SERVICE_NAME="payment-system"
DOMAIN="api.ibos.uz"
DB_NAME="payment_system_db"
DB_USER="postgres"
DB_PASSWORD="1234"

echo -e "${YELLOW}Step 1: Checking prerequisites...${NC}"

# Check if running as root
if [ "$EUID" -ne 0 ]; then 
    echo -e "${RED}Please run as root (use: sudo bash deploy.sh)${NC}"
    exit 1
fi

# Check .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo -e "${YELLOW}Installing .NET 8 SDK...${NC}"
    
    # Add Microsoft package repository
    wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    
    # Install .NET SDK
    apt-get update
    apt-get install -y dotnet-sdk-8.0
    
    echo -e "${GREEN}.NET 8 SDK installed successfully!${NC}"
else
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}.NET SDK already installed: $DOTNET_VERSION${NC}"
fi

# Check PostgreSQL
if ! command -v psql &> /dev/null; then
    echo -e "${RED}PostgreSQL not found! Please install PostgreSQL first.${NC}"
    exit 1
else
    echo -e "${GREEN}PostgreSQL found${NC}"
fi

# Check Nginx
if ! command -v nginx &> /dev/null; then
    echo -e "${YELLOW}Installing Nginx...${NC}"
    apt-get install -y nginx
    systemctl enable nginx
    systemctl start nginx
    echo -e "${GREEN}Nginx installed${NC}"
else
    echo -e "${GREEN}Nginx already installed${NC}"
fi

echo ""
echo -e "${YELLOW}Step 2: Creating PostgreSQL database...${NC}"

# Create database and user
sudo -u postgres psql <<EOF
-- Create user if not exists
DO \$\$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_user WHERE usename = '$DB_USER') THEN
        CREATE USER $DB_USER WITH PASSWORD '$DB_PASSWORD';
    END IF;
END
\$\$;

-- Create database if not exists
SELECT 'CREATE DATABASE $DB_NAME OWNER $DB_USER'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '$DB_NAME')\gexec

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE $DB_NAME TO $DB_USER;
EOF

echo -e "${GREEN}Database $DB_NAME created/verified${NC}"

echo ""
echo -e "${YELLOW}Step 3: Creating deployment directory...${NC}"

# Create deployment directory
mkdir -p $DEPLOY_DIR
mkdir -p $DEPLOY_DIR/wwwroot/uploads
chmod 755 $DEPLOY_DIR
chmod 777 $DEPLOY_DIR/wwwroot/uploads

echo -e "${GREEN}Directory created: $DEPLOY_DIR${NC}"

echo ""
echo -e "${YELLOW}Step 4: Downloading project from GitHub...${NC}"

# Check if git is installed
if ! command -v git &> /dev/null; then
    apt-get install -y git
fi

# Clone or pull repository
if [ -d "$DEPLOY_DIR/source" ]; then
    echo "Updating existing repository..."
    cd $DEPLOY_DIR/source
    git pull
else
    echo "Cloning repository..."
    # TODO: Replace with actual GitHub URL when created
    echo -e "${YELLOW}MANUAL STEP REQUIRED:${NC}"
    echo "Please run: git clone https://github.com/Oybek-M/PaymentSystem.git $DEPLOY_DIR/source"
    echo "Then run this script again"
    exit 0
fi

cd $DEPLOY_DIR/source

echo ""
echo -e "${YELLOW}Step 5: Building project...${NC}"

# Restore packages
dotnet restore

# Build project
dotnet build -c Release

# Publish project
dotnet publish PaymentSystem.API/PaymentSystem.API.csproj -c Release -o $DEPLOY_DIR/app

echo -e "${GREEN}Project built and published to $DEPLOY_DIR/app${NC}"

echo ""
echo -e "${YELLOW}Step 6: Configuring appsettings.Production.json...${NC}"

# Create production appsettings
cat > $DEPLOY_DIR/app/appsettings.Production.json <<EOF
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD"
  }
}
EOF

chmod 600 $DEPLOY_DIR/app/appsettings.Production.json

echo -e "${GREEN}Production settings configured${NC}"

echo ""
echo -e "${YELLOW}Step 7: Running database migrations...${NC}"

cd $DEPLOY_DIR/app

# Run migrations
export ASPNETCORE_ENVIRONMENT=Production
dotnet PaymentSystem.API.dll --migrate 2>/dev/null || {
    echo "Running EF migrations..."
    cd $DEPLOY_DIR/source
    dotnet ef database update --project PaymentSystem.DAL --startup-project PaymentSystem.API
}

echo -e "${GREEN}Database migrations completed${NC}"

echo ""
echo -e "${YELLOW}Step 8: Creating systemd service...${NC}"

# Create systemd service
cat > /etc/systemd/system/$SERVICE_NAME.service <<EOF
[Unit]
Description=Payment System API Service
After=network.target

[Service]
Type=notify
User=www-data
Group=www-data
WorkingDirectory=$DEPLOY_DIR/app
ExecStart=/usr/bin/dotnet $DEPLOY_DIR/app/PaymentSystem.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=payment-system
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF

# Set permissions
chown -R www-data:www-data $DEPLOY_DIR
chmod +x $DEPLOY_DIR/app/PaymentSystem.API.dll

# Reload systemd
systemctl daemon-reload
systemctl enable $SERVICE_NAME
systemctl restart $SERVICE_NAME

echo -e "${GREEN}Service created and started${NC}"

# Check service status
sleep 3
if systemctl is-active --quiet $SERVICE_NAME; then
    echo -e "${GREEN}✓ Service is running${NC}"
else
    echo -e "${RED}✗ Service failed to start. Check logs: journalctl -u $SERVICE_NAME -n 50${NC}"
fi

echo ""
echo -e "${YELLOW}Step 9: Configuring Nginx reverse proxy...${NC}"

# Create Nginx config
cat > /etc/nginx/sites-available/$DOMAIN <<EOF
upstream payment_api {
    server localhost:5000;
    keepalive 32;
}

server {
    listen 80;
    server_name $DOMAIN;

    # Redirect HTTP to HTTPS (will be configured after SSL)
    return 301 https://\$server_name\$request_uri;
}

server {
    listen 443 ssl http2;
    server_name $DOMAIN;

    # SSL configuration (will be set up by Certbot)
    # ssl_certificate /etc/letsencrypt/live/$DOMAIN/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/$DOMAIN/privkey.pem;

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # File upload limit (5MB for check files)
    client_max_body_size 5M;

    # API proxy
    location / {
        proxy_pass http://payment_api;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
        proxy_buffering off;
    }

    # Uploaded files
    location /uploads/ {
        alias $DEPLOY_DIR/wwwroot/uploads/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # Swagger UI
    location /swagger {
        proxy_pass http://payment_api;
        proxy_http_version 1.1;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }

    # Health check
    location /health {
        access_log off;
        return 200 "OK";
        add_header Content-Type text/plain;
    }
}
EOF

# Enable site
ln -sf /etc/nginx/sites-available/$DOMAIN /etc/nginx/sites-enabled/

# Test nginx config
nginx -t

if [ $? -eq 0 ]; then
    systemctl reload nginx
    echo -e "${GREEN}Nginx configured successfully${NC}"
else
    echo -e "${RED}Nginx configuration error${NC}"
    exit 1
fi

echo ""
echo -e "${YELLOW}Step 10: Setting up SSL with Let's Encrypt...${NC}"

# Install certbot
if ! command -v certbot &> /dev/null; then
    apt-get install -y certbot python3-certbot-nginx
fi

# Request SSL certificate
echo "Requesting SSL certificate for $DOMAIN..."
certbot --nginx -d $DOMAIN --non-interactive --agree-tos --email admin@ibos.uz --redirect

if [ $? -eq 0 ]; then
    echo -e "${GREEN}SSL certificate installed successfully!${NC}"
else
    echo -e "${YELLOW}SSL setup skipped or failed. Run manually: certbot --nginx -d $DOMAIN${NC}"
fi

echo ""
echo "======================================"
echo -e "${GREEN}Deployment Complete!${NC}"
echo "======================================"
echo ""
echo "🎉 Your API is now running!"
echo ""
echo "📍 API URL: https://$DOMAIN"
echo "📖 Swagger UI: https://$DOMAIN/swagger"
echo "💾 Database: $DB_NAME"
echo "📁 App Directory: $DEPLOY_DIR/app"
echo "📁 Uploads: $DEPLOY_DIR/wwwroot/uploads"
echo ""
echo "🔧 Useful Commands:"
echo "   Status: systemctl status $SERVICE_NAME"
echo "   Logs:   journalctl -u $SERVICE_NAME -f"
echo "   Restart: systemctl restart $SERVICE_NAME"
echo "   Stop:    systemctl stop $SERVICE_NAME"
echo ""
echo "🧪 Test API:"
echo "   curl https://$DOMAIN/swagger"
echo "   curl https://$DOMAIN/api/users"
echo ""

# Show service status
echo -e "${YELLOW}Current Service Status:${NC}"
systemctl status $SERVICE_NAME --no-pager -l
