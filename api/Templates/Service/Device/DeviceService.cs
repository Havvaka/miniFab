using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minifab.api.Templates.Data;
using minifab.api.Templates.Models;

namespace minifab.api.Templates.Service.Device
{
    public interface IDeviceService
    {
        Task<List<DeviceModel>> GetAllDevicesAsync();
        Task<DeviceModel> GetDeviceByIdAsync(string deviceId);
        Task<DeviceModel> AddDeviceAsync(DeviceModel device);
        Task<bool> UpdateDeviceAsync(DeviceModel device);
        Task<bool> DeleteDeviceAsync(string deviceId);
    }

    public class DeviceService : IDeviceService
    {
        private readonly ApiDbContext _dbContext;

        public DeviceService(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DeviceModel>> GetAllDevicesAsync()
        {
            return await _dbContext.DeviceModel.ToListAsync();
        }

        public async Task<DeviceModel> GetDeviceByIdAsync(string deviceId)
        {
            return await _dbContext.DeviceModel.FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        }

        public async Task<DeviceModel> AddDeviceAsync(DeviceModel device)
        {
            await _dbContext.DeviceModel.AddAsync(device);
            await _dbContext.SaveChangesAsync();
            return device;
        }

        public async Task<bool> UpdateDeviceAsync(DeviceModel device)
        {
            var existingDevice = await _dbContext.DeviceModel
                .FirstOrDefaultAsync(d => d.DeviceId == device.DeviceId);
                
            if (existingDevice == null)
                return false;
                
            existingDevice.DeviceName = device.DeviceName;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDeviceAsync(string deviceId)
        {
            var device = await _dbContext.DeviceModel
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
                
            if (device == null)
                return false;
                
            _dbContext.DeviceModel.Remove(device);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
