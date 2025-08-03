# MiniFab IoT Sensor Data Dashboard

**Real-time IoT sensor data monitoring system with SignalR integration**

Bu proje, IoT sensörlerinden gelen verileri gerçek zamanlı olarak toplamak, işlemek ve görselleştirmek için geliştirilmiş tam kapsamlı bir sistemdir. Proje; veri üretici (Producer), API backend, ve Vue.js frontend dashboard olmak üzere üç ana bileşenden oluşmaktadır.

## 🎯 Proje Özellikleri

- **Gerçek Zamanlı Veri İzleme**: SignalR ile anlık sensor verisi takibi
- **Message Queue Entegrasyonu**: RabbitMQ ile asenkron veri işleme
- **Modern Web Dashboard**: Vue.js 3 ile geliştirilmiş responsive arayüz
- **Database Entegrasyonu**: PostgreSQL ile veri kalıcılığı
- **RESTful API**: ASP.NET Core 8.0 ile modern API mimarisi
- **IoT Data Simulation**: Gerçekçi sensor data üretici

## 🏗️ Sistem Mimarisi

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Producer      │───▶│   RabbitMQ      │───▶│   API Backend   │
│   (C# Console)  │    │   Message       │    │   (ASP.NET)     │
│                 │    │   Broker        │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                                        │
                                                        ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Vue.js        │◀───│   SignalR       │◀───│   PostgreSQL    │
│   Dashboard     │    │   Hub           │    │   Database      │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 📋 Sistem Gereksinimleri

### Yazılım Gereksinimleri
- **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** - Backend API ve Producer için
- **[Node.js](https://nodejs.org/)** (v16 veya üzeri) - Frontend dashboard için
- **[PostgreSQL](https://www.postgresql.org/download/)** (v12 veya üzeri) - Veritabanı
- **[RabbitMQ](https://www.rabbitmq.com/download.html)** - Message broker
- **Git** - Kaynak kod yönetimi

### İsteğe Bağlı
- **[Docker](https://www.docker.com/get-started)** - RabbitMQ ve PostgreSQL için konteyner çözümü
- **[Visual Studio](https://visualstudio.microsoft.com/)** veya **[VS Code](https://code.visualstudio.com/)** - Geliştirme ortamı

## 🚀 Kurulum Adımları

### 1. Repository'yi Klonlayın

```bash
git clone https://github.com/yourusername/minifab-signalr.git
cd minifab-signalr
```

### 2. PostgreSQL Kurulumu ve Yapılandırması

#### Windows (Installer ile):
1. [PostgreSQL'i indirin ve kurun](https://www.postgresql.org/download/windows/)
2. Kurulum sırasında şifre belirleyin (örn: `Buzdandus9.`)
3. pgAdmin ile veritabanı oluşturun:

```sql
-- pgAdmin'de SQL sorgusu çalıştırın
CREATE DATABASE "MiniFabDB";
```

#### Docker ile (Alternatif):
```bash
docker run --name postgres-minifab -e POSTGRES_DB=MiniFabDB -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=Buzdandus9. -p 5432:5432 -d postgres:15
```

### 3. RabbitMQ Kurulumu

#### Windows (Installer ile):
1. [Erlang'ı indirin ve kurun](https://www.erlang.org/downloads)
2. [RabbitMQ'yu indirin ve kurun](https://www.rabbitmq.com/download.html)
3. RabbitMQ Management Plugin'i etkinleştirin:

```bash
# PowerShell'i Admin olarak açın
rabbitmq-plugins enable rabbitmq_management
```

4. RabbitMQ servisini başlatın:
```bash
net start RabbitMQ
```

#### Docker ile (Alternatif):
```bash
docker run --name rabbitmq-minifab -p 5672:5672 -p 15672:15672 -d rabbitmq:3-management
```

### 4. API Backend Kurulumu

```bash
# API dizinine gidin
cd api

# NuGet paketlerini geri yükleyin
dotnet restore

# Veritabanı yapılandırması için appsettings.json'u düzenleyin
# PostgreSQL bağlantı bilgilerinizi güncelleyin
```

**appsettings.json** dosyasını düzenleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=MiniFabDB;Username=postgres;Password=YOUR_PASSWORD_HERE"
  }
}
```

```bash
# Veritabanı migration'larını uygulayın
dotnet ef database update

# API'yi çalıştırın
dotnet run
```

API şu adreste çalışacak: `https://localhost:7018` veya `http://localhost:5018`

### 5. Frontend Dashboard Kurulumu

```bash
# Frontend dizinine gidin
cd ../client-dashboard

# npm dependencies'leri yükleyin
npm install

# Geliştirme sunucusunu başlatın
npm run serve
```

Dashboard şu adreste çalışacak: `http://localhost:8080`

### 6. Producer (Veri Üretici) Kurulumu

```bash
# Producer dizinine gidin
cd ../Producer

# NuGet paketlerini geri yükleyin
dotnet restore

# Producer'ı çalıştırın
dotnet run
```

## 🔧 Servis Yapılandırması

### API Servisi Bağlantı Noktaları
- **HTTPS**: `https://localhost:7018`
- **HTTP**: `http://localhost:5018`
- **SignalR Hub**: `/sensorHub`
- **Swagger UI**: `/swagger`

### CORS Yapılandırması
API, aşağıdaki origin'lerden gelen isteklere izin verir:
- `http://localhost:8080` (Vue.js dev server)
- `http://localhost:3000` (Alternatif frontend port)

### Veritabanı Bağlantısı
PostgreSQL bağlantı stringi `appsettings.json` dosyasında tanımlanmıştır:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=MiniFabDB;Username=postgres;Password=YOUR_PASSWORD"
}
```

## 📊 Veri Modeli

### SensorDataModel
```csharp
public class SensorDataModel
{
    public int Id { get; set; }
    public string DeviceId { get; set; }
    public double Temperature { get; set; }    // °C
    public int Humidity { get; set; }          // %
    public int Voltage { get; set; }           // mV
    public DateTime Timestamp { get; set; }
}
```

### DeviceModel
```csharp
public class DeviceModel
{
    public string DeviceId { get; set; }
    public string DeviceName { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsActive { get; set; }
}
```

## 🔄 Sistem Akışı

1. **Producer** → Rastgele sensor verisi üretir (5 saniyede bir)
2. **RabbitMQ** → Producer'dan gelen veriyi `sensor_data_queue` kuyruğuna alır
3. **API Consumer** → RabbitMQ'dan veriyi consume eder ve PostgreSQL'e kaydeder
4. **SignalR Hub** → Yeni veri geldiğinde tüm bağlı client'lara gerçek zamanlı bildirim gönderir
5. **Vue.js Dashboard** → SignalR ile gerçek zamanlı veri alır ve görselleştirir

## 🛠️ API Endpoints

### Sensor Data
- `GET /api/sensordata` - Tüm sensor verilerini listele
- `GET /api/sensordata/latest` - Son sensor verilerini getir
- `GET /api/sensordata/device/{deviceId}` - Belirli cihaza ait verileri getir

### Device Management
- `GET /api/device` - Tüm cihazları listele
- `GET /api/device/{deviceId}` - Belirli cihaz bilgisini getir

### Statistics
- `GET /api/statistics/summary` - Genel istatistikler
- `GET /api/statistics/device/{deviceId}` - Cihaz bazlı istatistikler

## 🎨 Frontend Özellikleri

- **Gerçek Zamanlı Dashboard**: SignalR ile anlık veri güncellemeleri
- **Cihaz Listesi**: Aktif/pasif cihaz durumları
- **Veri Görselleştirme**: Tablolar ve kartlarla veri sunumu
- **Responsive Design**: Mobil uyumlu arayüz
- **Connection Status**: RabbitMQ ve SignalR bağlantı durumu gösterimi

## 🐛 Hata Giderme

### Yaygın Problemler

#### 1. PostgreSQL Bağlantı Hatası
```bash
# PostgreSQL servisinin çalıştığını kontrol edin
# Windows:
net start postgresql-x64-15

# Bağlantı string'ini kontrol edin
# Kullanıcı adı, şifre ve veritabanı adının doğru olduğundan emin olun
```

#### 2. RabbitMQ Bağlantı Hatası
```bash
# RabbitMQ servisinin çalıştığını kontrol edin
# Windows:
net start RabbitMQ

# Management UI'ye erişim: http://localhost:15672
# Kullanıcı: guest, Şifre: guest
```

#### 3. CORS Hatası
```javascript
// API'nin CORS yapılandırmasını kontrol edin
// Program.cs dosyasında AllowOrigins listesine frontend URL'inizi ekleyin
```

#### 4. SignalR Bağlantı Problemi
```javascript
// Frontend'de SignalR bağlantı URL'ini kontrol edin
// src/services/SignalRService.js dosyasında:
const connection = new HubConnectionBuilder()
    .withUrl("https://localhost:7018/sensorHub")
    .build();
```

### Log Takibi

#### API Logları
```bash
# Console'da API loglarını takip edin
dotnet run --verbosity normal
```

#### Producer Logları
```bash
# Producer console çıktısını takip edin
# RabbitMQ bağlantı durumu ve message gönderimi loglanır
```

## 🧪 Test Etme

### 1. Sistem Bileşenlerini Test Edin

```bash
# PostgreSQL bağlantısını test edin
psql -h localhost -U postgres -d MiniFabDB

# RabbitMQ management'a erişin
# http://localhost:15672 (guest/guest)

# API'nin çalıştığını kontrol edin
curl https://localhost:7018/api/sensordata

# Frontend'in çalıştığını kontrol edin
# http://localhost:8080
```

### 2. Veri Akışını Test Edin

1. Producer'ı çalıştırın ve console'da mesaj gönderimini kontrol edin
2. API console'unda RabbitMQ'dan mesaj alımını kontrol edin
3. Dashboard'da gerçek zamanlı veri güncellemelerini gözlemleyin

## 📦 Production Deployment

### Environment Variables

Production ortamı için aşağıdaki environment variable'ları ayarlayın:

```bash
# API için
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="Your_Production_DB_Connection"

# RabbitMQ için
RABBITMQ_HOST=your-rabbitmq-host
RABBITMQ_PORT=5672
RABBITMQ_USER=your-username
RABBITMQ_PASS=your-password
```

### Build Commands

```bash
# API build
cd api
dotnet build --configuration Release
dotnet publish --configuration Release

# Frontend build
cd client-dashboard
npm run build
```

## 🤝 Katkıda Bulunma

1. Repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için `LICENSE` dosyasına bakın.

## 📞 İletişim

- **Proje Sahibi**: [Your Name]
- **Email**: [your-email@example.com]
- **GitHub**: [https://github.com/yourusername]

## 🙏 Teşekkürler

- Microsoft SignalR Team
- RabbitMQ Community
- Vue.js Community
- PostgreSQL Community

---

**⚡ Başarılı bir kurulum sonrasında sistem tamamen çalışır durumda olacak ve gerçek zamanlı IoT sensor verilerini izleyebileceksiniz!**
