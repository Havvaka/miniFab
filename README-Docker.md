# MiniFab IoT Docker Deployment

Bu dokümantasyon, MiniFab IoT projesinin Docker Compose ile nasıl çalıştırılacağını açıklar.

## 📋 Gereksinimler

- **Docker Desktop** (Windows)
- **Docker Compose** v2.0+
- En az **4GB RAM** (tüm servisler için)

## 🚀 Hızlı Başlangıç

### 1. Projeyi İndirin
```bash
git clone <your-repo-url>
cd miniFab-project
```

### 2. Docker Compose ile Başlatın
```bash
# Tüm servisleri başlat
docker-compose up -d

# Logları takip edin
docker-compose logs -f
```

### 3. Servislere Erişim
- **Frontend Dashboard**: http://localhost:8080
- **API Backend**: http://localhost:5000
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **PostgreSQL**: localhost:5432

## 🛠️ Docker Servisleri

| Servis | Container | Port | Açıklama |
|--------|-----------|------|----------|
| postgres | minifab_postgres | 5432 | PostgreSQL Database |
| rabbitmq | minifab_rabbitmq | 5672, 15672 | RabbitMQ Message Broker |
| api | minifab_api | 5000 | ASP.NET Core API |
| producer | minifab_producer | - | Data Producer |
| frontend | minifab_frontend | 8080 | Vue.js Dashboard |

## 📊 Durum Kontrolü

### Tüm Servislerin Durumunu Kontrol Edin
```bash
docker-compose ps
```

### Servis Loglarını İnceleyin
```bash
# Tüm servisler
docker-compose logs

# Belirli bir servis
docker-compose logs api
docker-compose logs producer
docker-compose logs frontend
```

### Health Check
```bash
# PostgreSQL bağlantısı
docker-compose exec postgres pg_isready -U postgres

# RabbitMQ durumu
docker-compose exec rabbitmq rabbitmq-diagnostics ping
```

## 🔄 Yaygın Komutlar

### Servisleri Yeniden Başlatma
```bash
# Tüm servisleri durdur
docker-compose down

# Belirli bir servisi yeniden başlat
docker-compose restart api

# Değişikliklerle beraber yeniden build et
docker-compose up --build -d
```

### Veri Temizleme
```bash
# Tüm servisleri durdur ve volume'ları sil
docker-compose down -v

# Kullanılmayan Docker objelerini temizle
docker system prune -f
```

## 🐛 Sorun Giderme

### 1. Port Çakışması
Eğer portlar kullanılıyorsa, docker-compose.yml dosyasında port mappinglerini değiştirin:
```yaml
ports:
  - "8081:80"  # 8080 yerine 8081 kullan
```

### 2. Bellek Problemi
```bash
# Docker Desktop'ta memory limitini artırın
# Settings > Resources > Memory > 4GB veya daha fazla
```

### 3. Database Connection Hatası
```bash
# PostgreSQL container'ının başlatılmasını bekleyin
docker-compose logs postgres

# Connection string'i kontrol edin
docker-compose exec api env | grep CONNECTION
```

### 4. RabbitMQ Bağlantı Problemi
```bash
# RabbitMQ'nun tamamen başladığını kontrol edin
docker-compose logs rabbitmq

# Producer'ın bağlanmasını bekleyin
docker-compose logs producer
```

## 🔧 Geliştirme Modu

### Development Override
```bash
# Development konfigürasyonu ile başlat
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d
```

### Volume Mounting için docker-compose.dev.yml:
```yaml
version: '3.8'
services:
  api:
    volumes:
      - ./api:/app/src
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
```

## 📁 Klasör Yapısı
```
miniFab-project/
├── docker-compose.yml          # Ana compose dosyası
├── .env                        # Environment değişkenleri
├── .dockerignore              # Docker ignore kuralları
├── api/
│   ├── Dockerfile
│   └── appsettings.Production.json
├── Producer/
│   └── Dockerfile
└── client-dashboard/
    ├── Dockerfile
    ├── nginx.conf
    └── .dockerignore
```

## 🔄 Güncellemeler

### Kodu Güncelledikten Sonra
```bash
# Servisleri yeniden build et
docker-compose build

# Güncellenen servisleri yeniden başlat
docker-compose up -d
```

### Database Migration
```bash
# API container içinde migration çalıştır
docker-compose exec api dotnet ef database update
```

## 💾 Backup & Restore

### Database Backup
```bash
docker-compose exec postgres pg_dump -U postgres MiniFabDB > backup.sql
```

### Database Restore
```bash
docker-compose exec -T postgres psql -U postgres MiniFabDB < backup.sql
```

## 🔧 Monitoring

### Resource Usage
```bash
# Container resource kullanımı
docker stats

# Disk kullanımı
docker system df
```

### Performance Monitoring
Prometheus ve Grafana ekleyerek monitoring yapabilirsiniz:
```yaml
# docker-compose.monitoring.yml
services:
  prometheus:
    image: prom/prometheus
    ports:
      - "9090:9090"
```

## ⚡ Production Hazırlık

1. **Environment değişkenlerini güncelleyin**
2. **SSL sertifikalarını ekleyin**
3. **Resource limitlerini ayarlayın**
4. **Health check'leri yapılandırın**
5. **Log aggregation sistemi kurun**

Bu yapılandırma ile MiniFab IoT projeniz tamamen Docker ortamında çalışacaktır.
