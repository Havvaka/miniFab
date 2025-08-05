# MiniFab IoT Sensor Data Dashboard

IoT sensörlerinden gelen verileri gerçek zamanlı olarak izlemek için geliştirilmiş bir sistemdir.

## 🎯 Proje Ne İçin?

- Gerçek zamanlı IoT sensor verilerini toplama ve görselleştirme
- SignalR ile anlık veri takibi
- Message queue ile asenkron veri işleme

## 🛠️ Kullanılan Teknolojiler

- **Backend**: ASP.NET Core 8.0, SignalR, Entity Framework
- **Frontend**: Vue.js 3
- **Database**: PostgreSQL
- **Message Broker**: RabbitMQ
- **Container**: Docker

## 🚀 Projeyi Çalıştırma

### 1. Repository'yi Klonlayın

```bash
git clone https://github.com/Havvaka/miniFab.git
cd miniFab
```

### 2. Docker ile Çalıştırma (Önerilen)

```bash
# Tüm servisleri başlatın
docker compose up -d

# Durumu kontrol edin
docker compose ps
```

### 3. Erişim Adresleri

- **Frontend Dashboard**: http://localhost:8080
- **API**: http://localhost:5000
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

### 4. Projeyi Durdurmak

```bash
docker compose down
```

## � Sistem Akışı

1. **Producer** → Rastgele sensor verisi üretir
2. **RabbitMQ** → Verileri kuyrukta tutar
3. **API** → Verileri alır ve PostgreSQL'e kaydeder
4. **SignalR** → Frontend'e gerçek zamanlı bildirim gönderir
5. **Dashboard** → Verileri görselleştirir
