<template>
  <div class="dashboard-container">

    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <span>Veriler yükleniyor...</span>
    </div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>
    
    <div v-else>
    
      <div class="connection-bar">
        <div class="signalr-status" :class="{ connected: signalRConnected }">
          <span v-if="signalRConnected">🟢 Canlı Veri Bağlantısı Aktif - SignalR İle Canlı Veri Dinleniyor</span>
          <span v-else>🔴 Canlı Veri Bağlantısı Beklemede</span>
        </div>
        
      </div>
      
      <div v-if="latestLiveData" class="live-data-card">
        <h3>CANLI VERİ</h3>
        <div class="live-data-indicator"></div>
        <div class="last-update">Son Güncelleme: {{ formatDate(new Date()) }}</div>
        <div class="live-data-grid">
          <div class="live-data-item">
            <div class="label">Cihaz ID</div>
            <div class="value">{{ latestLiveData.deviceId }}</div>
          </div>
          <div class="live-data-item">
            <div class="label">Sıcaklık</div>
            <div class="value" :class="getTemperatureClass(latestLiveData.temperature)">
              {{ latestLiveData.temperature }}°C
            </div>
          </div>
          <div class="live-data-item">
            <div class="label">Nem</div>
            <div class="value" :class="getHumidityClass(latestLiveData.humidity)">
              {{ latestLiveData.humidity }}%
            </div>
          </div>
          <div class="live-data-item">
            <div class="label">Voltaj</div>
            <div class="value" :class="getVoltageClass(latestLiveData.voltage)">
              {{ latestLiveData.voltage }}V
            </div>
          </div>
          <div class="live-data-item">
            <div class="label">Zaman</div>
            <div class="value">{{ formatDate(latestLiveData.timestamp) }}</div>
          </div>
        </div>
      </div>
      

      <div class="sensor-data-table">
        <div class="device-info">
          <h2>{{ deviceId }} Cihazı Geçmiş Veriler</h2>
          <span class="data-count">{{ sensorData.length }} kayıt gösteriliyor</span>
        </div>

        <table>
          <thead>
            <tr>
              <th>Device ID</th>
              <th>Sıcaklık (°C)</th>
              <th>Nem (%)</th>
              <th>Voltaj (V)</th>
              <th>Zaman</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="data in sensorData" :key="data.id" :class="{ 'new-data': isNewData(data.id) }">
              <td>{{ data.deviceId }}</td>
              <td :class="getTemperatureClass(data.temperature)">{{ data.temperature }}°C</td>
              <td :class="getHumidityClass(data.humidity)">{{ data.humidity }}%</td>
              <td :class="getVoltageClass(data.voltage)">{{ data.voltage }}V</td>
              <td>{{ formatDate(data.timestamp) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import api from '@/services/api';
import signalRService from '@/services/SignalRService';

export default {
  name: 'SensorDataTable',
  
  props: {
    deviceId: {
      type: String,
      required: true
    },
    limit: {
      type: Number,
      default: 10
    }
  },
  
  data() {
    return {
      sensorData: [],
      latestLiveData: null,
      loading: false,
      error: null,
      signalRConnected: false,
      newDataIds: new Set()
    };
  },
  
  watch: {

    deviceId(newDeviceId, oldDeviceId) {
      if (oldDeviceId) {
        this.cleanupSignalR();
      }
      if (newDeviceId) {
        this.fetchInitialData();
        this.setupSignalR();
      }
    }
  },
  
  mounted() {
    this.fetchInitialData();
    this.setupSignalR();
  },
  
  beforeUnmount() {
    this.cleanupSignalR();
  },
  
  methods: {
    
    async manualRefresh() {
      await this.fetchInitialData();
    },
    
    async fetchInitialData() {
      this.loading = true;
      
      try {
        const response = await api.getLatestSensorDataByDevice(this.deviceId, this.limit);
        
        if (response && response.success) {
          this.sensorData = response.data;
          if (response.data.length > 0) {
            this.latestLiveData = { ...response.data[0] };
          }
        } else {
          this.error = 'Veri alınamadı';
        }
      } catch (err) {
        console.error('API hatası:', err);
        
      } finally {
        this.loading = false;
      }
    },
    
    async setupSignalR() {
      
      signalRService.setCallbacks({
       
        onReceiveSensorData: (data) => {
      
          if (data.deviceId === this.deviceId) {
            this.handleNewSensorData(data);
          }
        },
        
        onReceiveDeviceSensorData: (data) => {
          this.handleNewSensorData(data);
        },
        
        onConnectionChange: (isConnected) => {
          this.signalRConnected = isConnected;
        }
      });
      
      const connected = await signalRService.startConnection();
      this.signalRConnected = connected;
      
      if (connected) {
        await signalRService.joinDeviceGroup(this.deviceId);
      }
    },
  
    async cleanupSignalR() {
      if (this.deviceId) {
        await signalRService.leaveDeviceGroup(this.deviceId);
      }
    },
    
    handleNewSensorData(data) {
      console.log('Yeni sensör verisi alındı:', data);
      console.log('Mevcut sensorData uzunluğu:', this.sensorData.length);

      
      this.latestLiveData = { ...data };
      
      const existingIndex = this.sensorData.findIndex(item => item.id === data.id);
      if (existingIndex !== -1) {
        this.sensorData = [
          ...this.sensorData.slice(0, existingIndex),
          data,
          ...this.sensorData.slice(existingIndex + 1)
        ];
      } else {
        console.log('Yeni veri ekleniyor');
        this.sensorData = [data, ...this.sensorData].slice(0, this.limit);
      }
      

      this.newDataIds.add(data.id);
      
      setTimeout(() => {
        this.newDataIds.delete(data.id);
      }, 3000);
    },
    
    isNewData(id) {
      return this.newDataIds.has(id);
    },
    
    formatDate(timestamp) {
      const date = new Date(timestamp);
      return date.toLocaleString('tr-TR', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
      });
    },
    
    getTemperatureClass(temperature) {
      if (temperature > 30) return 'high-value';
      if (temperature < 15) return 'low-value';
      return 'normal-value';
    },
    
    getHumidityClass(humidity) {
      if (humidity > 70) return 'high-value';
      if (humidity < 30) return 'low-value';
      return 'normal-value';
    },
    
    getVoltageClass(voltage) {
      if (voltage > 240) return 'high-value';
      if (voltage < 210) return 'low-value';
      return 'normal-value';
    }
  }
};
</script>

<style scoped>
.dashboard-container {
  max-width: 1000px;
  margin: 20px auto;
}

.sensor-data-table {
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  padding: 20px;
  margin-top: 20px;
}

/* Canlı Veri Kartı Stillemesi */
.live-data-card {
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 15px rgba(0, 0, 0, 0.15);
  padding: 20px;
  margin-top: 20px;
  position: relative;
  overflow: hidden;
  border-left: 4px solid #dc3545;
}

.live-data-card h3 {
  color: #dc3545;
  margin-top: 0;
  display: flex;
  align-items: center;
  font-size: 1.2rem;
}

.live-data-indicator {
  width: 10px;
  height: 10px;
  background-color: #dc3545;
  border-radius: 50%;
  margin-right: 8px;
  display: inline-block;
  animation: pulse 1.5s infinite;
}

.live-data-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
  gap: 15px;
  margin-top: 15px;
}

.live-data-item {
  padding: 10px;
  background-color: #f8f9fa;
  border-radius: 4px;
}

.live-data-item .label {
  font-size: 0.8rem;
  color: #6c757d;
  margin-bottom: 5px;
}

.live-data-item .value {
  font-size: 1.5rem;
  font-weight: 600;
}

.connection-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: #f8f9fa;
  border-radius: 8px;
  padding: 10px 15px;
  margin-top: 10px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.actions {
  display: flex;
  align-items: center;
  gap: 15px;
}

.refresh-btn {
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  padding: 8px 12px;
  cursor: pointer;
  display: flex;
  align-items: center;
  transition: background-color 0.3s;
}

.refresh-btn:hover {
  background-color: #0069d9;
}

.refresh-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.refresh-icon {
  margin-right: 5px;
  font-size: 1.1rem;
}

.polling-toggle {
  display: flex;
  align-items: center;
  gap: 5px;
}

.interval-select {
  padding: 5px 8px;
  border-radius: 4px;
  border: 1px solid #ced4da;
}

.last-update {
  font-size: 0.8rem;
  color: #6c757d;
  margin-bottom: 10px;
  text-align: right;
}

/* Mevcut CSS */
.device-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.data-count {
  color: #6c757d;
  font-size: 0.9rem;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 10px;
}

th, td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #e9ecef;
}

th {
  background-color: #f8f9fa;
  font-weight: 600;
}

tr:hover {
  background-color: #f6f8fa;
}

.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40px;
}

.spinner {
  border: 4px solid rgba(0, 0, 0, 0.1);
  border-radius: 50%;
  border-top: 4px solid #3498db;
  width: 30px;
  height: 30px;
  animation: spin 1s linear infinite;
  margin-bottom: 10px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes pulse {
  0% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.5; transform: scale(1.2); }
  100% { opacity: 1; transform: scale(1); }
}

.error-message {
  color: #dc3545;
  padding: 20px;
  text-align: center;
  border: 1px solid #f5c6cb;
  background-color: #f8d7da;
  border-radius: 4px;
}

.high-value {
  color: #dc3545;
}

.low-value {
  color: #007bff;
}

.normal-value {
  color: #28a745;
}

.signalr-status {
  margin: 0;
  padding: 8px;
  border-radius: 4px;
  text-align: center;
  background-color: #f8f9fa;
}

.signalr-status.connected {
  background-color: #e8f5e9;
}

.new-data {
  animation: highlight 3s ease-out;
}

@keyframes highlight {
  0% { background-color: #fffde7; }
  100% { background-color: transparent; }
}
</style>
