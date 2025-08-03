using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using minifab.api.Templates.Service.Device;
using minifab.api.Templates.Models;

namespace minifab.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DeviceModel>>> GetAllDevices()
        {
            var devices = await _deviceService.GetAllDevicesAsync();
            return Ok(devices);
        }

        [HttpGet("{deviceId}")]
        public async Task<ActionResult<DeviceModel>> GetDeviceById(string deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            
            if (device == null)
                return NotFound($"Cihaz bulunamadı: {deviceId}");
                
            return Ok(device);
        }

        [HttpPost]
        public async Task<ActionResult<DeviceModel>> AddDevice(DeviceModel device)
        {
            var existingDevice = await _deviceService.GetDeviceByIdAsync(device.DeviceId);
            
            if (existingDevice != null)
                return Conflict($"Bu ID ile kayıtlı cihaz zaten var: {device.DeviceId}");
                
            var newDevice = await _deviceService.AddDeviceAsync(device);
            return CreatedAtAction(nameof(GetDeviceById), new { deviceId = newDevice.DeviceId }, newDevice);
        }

        [HttpPut("{deviceId}")]
        public async Task<IActionResult> UpdateDevice(string deviceId, DeviceModel device)
        {
            if (deviceId != device.DeviceId)
                return BadRequest("Cihaz ID'leri eşleşmiyor");
                
            var success = await _deviceService.UpdateDeviceAsync(device);
            
            if (!success)
                return NotFound($"Güncellenecek cihaz bulunamadı: {deviceId}");
                
            return NoContent();
        }

        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteDevice(string deviceId)
        {
            var success = await _deviceService.DeleteDeviceAsync(deviceId);
            
            if (!success)
                return NotFound($"Silinecek cihaz bulunamadı: {deviceId}");
                
            return NoContent();
        }
    }
}
