using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using minifab.api.Templates.Service.Device;
using minifab.api.Templates.Models;
using Microsoft.AspNetCore.Http.HttpResults;

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
            return Ok(new
            {
                success = true,
                data = devices,
            });
        }

        [HttpGet("{deviceId}")]
        public async Task<ActionResult<DeviceModel>> GetDeviceById(string deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);

            if (device == null)
                return NotFound($"Device not found: {deviceId}");

            return Ok(new
            {
                success = true,
                data = device
            });
        }

        [HttpPost]
        public async Task<ActionResult<DeviceModel>> AddDevice(DeviceModel device)
        {
            var existingDevice = await _deviceService.GetDeviceByIdAsync(device.DeviceId);

            if (existingDevice != null)
                return Conflict($"This device has an id: {device.DeviceId}");

            var newDevice = await _deviceService.AddDeviceAsync(device);
            return Ok(new
            {
                success = true,
                data = newDevice,
                message = "Device added successfully"
            });
        }

        [HttpPut("{deviceId}")]
        public async Task<IActionResult> UpdateDevice(string deviceId, DeviceModel device)
        {
            if (deviceId != device.DeviceId)
                return BadRequest(new { success = false, message = "Device IDs do not match" });

            var success = await _deviceService.UpdateDeviceAsync(device);

            if (!success)
                return NotFound(new { success = false, message = $"Device not found: {deviceId}" });

            return Ok(new
            {
                success = true,
                message = "Device updated successfully:"
            });
        }

        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteDevice(string deviceId)
        {
            var success = await _deviceService.DeleteDeviceAsync(deviceId);

            if (!success)
                return NotFound(new { success = false, message = $"Device not found: {deviceId}" });

            return Ok(new
            {
                success = true,
                message = "Device deleted successfully"
            });
        }
    }
}
