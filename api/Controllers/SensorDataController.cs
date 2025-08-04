using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using minifab.api.Templates.Service.SensorData;
using minifab.api.Templates.Models;

namespace minifab.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorDataController : ControllerBase
    {
        private readonly ISensorDataService _sensorDataService;

        public SensorDataController(ISensorDataService sensorDataService)
        {
            _sensorDataService = sensorDataService;
        }


        [HttpGet("device/{deviceId}/latest")]
        public async Task<ActionResult<object>> GetLatestSensorDataByDevice(string deviceId, int limit)
        {
            try
            {
                var sensorData = await _sensorDataService.GetSensorDataByDeviceId(deviceId, limit);

                if (sensorData.Count == 0)
                    return NotFound(new { success = false, message = $"Device not found or no data: {deviceId}" });

                return Ok(new
                {
                    success = true,
                    data = sensorData,
                    count = sensorData.Count,
                    
                    deviceId = deviceId
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Server Error" });
            }
        }
        [HttpGet]
        public async Task<ActionResult<object>> GetAllSensorData()
        {
            try
            {
                var sensorData = await _sensorDataService.GetAllSensorData();
                return Ok(new
                {
                    success = true,
                    data = sensorData,
                    count = sensorData.Count
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Server Error" });
            }
        }

    }
}
