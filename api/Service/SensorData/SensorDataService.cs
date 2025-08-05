using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniFab.Api.Data;
using MiniFab.Api.Models;
using MiniFab.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MiniFab.Api.Services.SensorData
{

    public interface ISensorDataService
    {
        Task<List<SensorDataModel>> GetAllSensorData();
        Task<List<SensorDataModel>> GetSensorDataByDeviceId(string deviceId, int count);
        Task<SensorDataModel> AddSensorData(SensorDataModel sensorData);
        
    }

    public class SensorDataService : ISensorDataService
    {
        private readonly ApiDbContext _dbContext;
        private readonly IHubContext<SensorHub> _hubContext;

        public SensorDataService(ApiDbContext dbContext, IHubContext<SensorHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        public async Task<List<SensorDataModel>> GetAllSensorData()
        {
            return await _dbContext.SensorDataModel.ToListAsync();
        }

        public async Task<List<SensorDataModel>> GetSensorDataByDeviceId(string deviceId, int count)
        {
            return await _dbContext.SensorDataModel
                .Where(s => s.DeviceId == deviceId)
                .OrderByDescending(s => s.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<SensorDataModel> AddSensorData(SensorDataModel sensorData)
        {

            if (sensorData.Temperature < -40 || sensorData.Temperature > 85)
            {
                throw new ArgumentException("geçersiz sensor verisi");
            }


            var device = await _dbContext.DeviceModel
                .AnyAsync(d => d.DeviceId == sensorData.DeviceId);

            if (!device)
            {
                await _dbContext.DeviceModel.AddAsync(new DeviceModel
                {
                    DeviceId = sensorData.DeviceId,
                    DeviceName = sensorData.DeviceId
                });
            }

            await _dbContext.SensorDataModel.AddAsync(sensorData);
            await _dbContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveSensorData", sensorData);
            await _hubContext.Clients.Group(sensorData.DeviceId)
                       .SendAsync("ReceiveDeviceSensorData", sensorData);

            return sensorData;
        }
    }
}
