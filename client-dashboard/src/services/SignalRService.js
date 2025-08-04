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
    this.hubUrl = 'http://localhost:5297/sensorHub';
  }

  setCallbacks(callbacks) {
    this.callbacks = { ...this.callbacks, ...callbacks };
  }

  async startConnection() {
    if (this.connection && this.isConnected) {  
      return true;
    }

    try {

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(this.hubUrl)
        .withAutomaticReconnect([0, 1000, 5000, 10000]) 
        .configureLogging(signalR.LogLevel.Information)
        .build();

      
      this.connection.on('ReceiveSensorData', (data) => {

        this.callbacks.onReceiveSensorData(data);
      });

      this.connection.on('ReceiveDeviceSensorData', (data) => {
        this.callbacks.onReceiveDeviceSensorData(data);
      });

      this.connection.onreconnecting(() => {
        this.isConnected = false;
        this.callbacks.onConnectionChange(false);
      });

      this.connection.onreconnected(() => {
       
        this.isConnected = true;
        this.callbacks.onConnectionChange(true);
      });

      this.connection.onclose(() => {
      
        this.isConnected = false;
        this.callbacks.onConnectionChange(false);
      });

      
      await this.connection.start();
     
      this.isConnected = true;
      this.callbacks.onConnectionChange(true);
      return true;
    } catch (error) {
     
      this.isConnected = false;
      this.callbacks.onConnectionChange(false);
      return false;
    }
  }

  async stopConnection() {
    if (this.connection) {
      try {
        await this.connection.stop();
 
      } catch (error) {
        // Bağlantı kapatma işlemi sırasında oluşabilecek hatalar görmezden gelinir
        console.warn('Connection stop error:', error);
      } finally {
        this.connection = null;
        this.isConnected = false;
        this.callbacks.onConnectionChange(false);
      }
    }
  }

  async joinDeviceGroup(deviceId) {
    if (!this.connection || !this.isConnected) {
     
      return false;
    }

    try {
      await this.connection.invoke('JoinDeviceGroup', deviceId);
      console.log(`Successfully joined device group: ${deviceId}`);
      return true;
    } catch (error) {
      console.error(`Failed to join device group: ${deviceId}`, error);
      return false;
    }
  }

  async leaveDeviceGroup(deviceId) {
    if (!this.connection || !this.isConnected) {
      return false;
    }

    try {
      await this.connection.invoke('LeaveDeviceGroup', deviceId);
      console.log(`Successfully left device group: ${deviceId}`);
      return true;
    } catch (error) {
      console.error(`Failed to leave device group: ${deviceId}`, error);
      return false;
    }
  }
}

export default new SignalRService();