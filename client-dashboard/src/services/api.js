import axios from 'axios';


const apiClient = axios.create({
  baseURL: 'http://localhost:5297/api',
  headers: {
    'Content-Type': 'application/json'
  },
  timeout: 10000
});

const processResponse = (response) => {
  if (response.data && response.data.success) {
    return response.data;
  } else {
    throw new Error(response.data?.message || 'API hatası ');
  }
};
export default {

  async getAllSensorData() {
    const response = await apiClient.get('/SensorData');
    return processResponse(response);
  },
  
  async getLatestSensorDataByDevice(deviceId, limit = 10) {
    const response = await apiClient.get(`/SensorData/device/${deviceId}/latest`, {
      params: { limit }
    });
    return processResponse(response);
  },
  
  async addSensorData(sensorData) {
    const response = await apiClient.post('/SensorData', sensorData);
    return processResponse(response);
  }
};