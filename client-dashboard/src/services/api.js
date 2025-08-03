import axios from 'axios';

// API temel URL'si
const apiClient = axios.create({
  baseURL: 'http://localhost:5297/api',
  headers: {
    'Content-Type': 'application/json'
  },
  timeout: 10000
});

// Yanıt işleme için yardımcı fonksiyonlar
const processResponse = (response) => {
  if (response.data && response.data.success) {
    return response.data;
  } else {
    throw new Error(response.data?.message || 'API hatası oluştu');
  }
};

export default {
  // Tüm sensör verilerini getir
  async getAllSensorData() {
    const response = await apiClient.get('/SensorData');
    return processResponse(response);
  },
  
  // Belirli bir cihazın son sensör verilerini getir
  async getLatestSensorDataByDevice(deviceId, limit = 10) {
    const response = await apiClient.get(`/SensorData/device/${deviceId}/latest`, {
      params: { limit }
    });
    return processResponse(response);
  },
  
  // Yeni sensör verisi ekle
  async addSensorData(sensorData) {
    const response = await apiClient.post('/SensorData', sensorData);
    return processResponse(response);
  }
};