import * as signalR from '@microsoft/signalr';

class SignalRService {
  constructor() {
    this.connection = null;
    this.callbacks = {
      onReceiveSensorData: () => {},
      onReceiveDeviceSensorData: () => {},
      onConnectionChange: () => {}
    };
    this.isConnected = false;
    this.hubUrl = 'http://localhost:5297/sensorHub'; // API ile aynı port ve host
  }

  setCallbacks(callbacks) {
    this.callbacks = { ...this.callbacks, ...callbacks };
  }

  async startConnection() {
    if (this.connection && this.isConnected) {
      console.log('SignalR zaten bağlı, tekrar bağlanmaya gerek yok');
      return true;
    }

    try {

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(this.hubUrl)
        .withAutomaticReconnect([0, 1000, 5000, 10000]) 
        .configureLogging(signalR.LogLevel.Information)
        .build();

      
      this.connection.on('ReceiveSensorData', (data) => {
        console.log('ReceiveSensorData alındı:', data);
        this.callbacks.onReceiveSensorData(data);
      });

      this.connection.on('ReceiveDeviceSensorData', (data) => {
        console.log('ReceiveDeviceSensorData alındı:', data);
        this.callbacks.onReceiveDeviceSensorData(data);
      });

     
      this.connection.onreconnecting(() => {
        console.log('SignalR yeniden bağlanıyor...');
        this.isConnected = false;
        this.callbacks.onConnectionChange(false);
      });

      this.connection.onreconnected(() => {
        console.log('SignalR yeniden bağlandı');
        this.isConnected = true;
        this.callbacks.onConnectionChange(true);
      });

      this.connection.onclose(() => {
        console.log('SignalR bağlantısı kapandı');
        this.isConnected = false;
        this.callbacks.onConnectionChange(false);
      });

      
      await this.connection.start();
      console.log('SignalR bağlantısı başarıyla kuruldu');
      this.isConnected = true;
      this.callbacks.onConnectionChange(true);
      return true;
    } catch (error) {
      console.error('SignalR bağlantısı kurulamadı:', error);
      this.isConnected = false;
      this.callbacks.onConnectionChange(false);
      return false;
    }
  }

  async stopConnection() {
    if (this.connection) {
      try {
        await this.connection.stop();
        console.log('SignalR bağlantısı durduruldu');
      } catch (error) {
        console.error('SignalR bağlantısı durdurulurken hata:', error);
      } finally {
        this.connection = null;
        this.isConnected = false;
        this.callbacks.onConnectionChange(false);
      }
    }
  }

  async joinDeviceGroup(deviceId) {
    if (!this.connection || !this.isConnected) {
      console.error('SignalR bağlantısı yok, cihaz grubuna katılınamıyor');
      return false;
    }

    try {
      await this.connection.invoke('JoinDeviceGroup', deviceId);
      console.log(`Cihaz grubuna katılındı: ${deviceId}`);
      return true;
    } catch (error) {
      console.error(`Cihaz grubuna katılırken hata: ${deviceId}`, error);
      return false;
    }
  }

  async leaveDeviceGroup(deviceId) {
    if (!this.connection || !this.isConnected) {
      return false;
    }

    try {
      await this.connection.invoke('LeaveDeviceGroup', deviceId);
      console.log(`Cihaz grubundan ayrıldı: ${deviceId}`);
      return true;
    } catch (error) {
      console.error(`Cihaz grubundan ayrılırken hata: ${deviceId}`, error);
      return false;
    }
  }
}

export default new SignalRService();