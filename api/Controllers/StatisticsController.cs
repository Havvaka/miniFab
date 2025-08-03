using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minifab.api.Templates.Data;
using minifab.api.Templates.Models;

namespace minifab.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;

        public StatisticsController(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummaryStatistics()
        {
            var deviceCount = await _dbContext.DeviceModel.CountAsync();
            var dataCount = await _dbContext.SensorDataModel.CountAsync();
            
            var lastRecord = await _dbContext.SensorDataModel
                .OrderByDescending(s => s.Timestamp)
                .FirstOrDefaultAsync();
                
            double avgTemperature = 0;
            double avgHumidity = 0;
            
            if (dataCount > 0)
            {
                avgTemperature = await _dbContext.SensorDataModel
                    .AverageAsync(s => s.Temperature);
                    
                avgHumidity = await _dbContext.SensorDataModel
                    .AverageAsync(s => s.Humidity);
            }
                
            return Ok(new
            {
                DeviceCount = deviceCount,
                DataPointCount = dataCount,
                LastRecordTime = lastRecord?.Timestamp,
                AverageTemperature = Math.Round(avgTemperature, 2),
                AverageHumidity = Math.Round(avgHumidity, 2)
            });
        }

        [HttpGet("device/{deviceId}/summary")]
        public async Task<ActionResult<object>> GetDeviceStatistics(string deviceId)
        {
            var device = await _dbContext.DeviceModel
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
                
            if (device == null)
                return NotFound($"Cihaz bulunamadı: {deviceId}");
                
            var dataCount = await _dbContext.SensorDataModel
                .Where(s => s.DeviceId == deviceId)
                .CountAsync();
                
            if (dataCount == 0)
                return Ok(new
                {
                    DeviceId = device.DeviceId,
                    DeviceName = device.DeviceName,
                    DataPointCount = 0,
                    Message = "Bu cihaza ait henüz veri bulunmamaktadır."
                });
                
            var lastRecord = await _dbContext.SensorDataModel
                .Where(s => s.DeviceId == deviceId)
                .OrderByDescending(s => s.Timestamp)
                .FirstOrDefaultAsync();
                
            var avgTemperature = await _dbContext.SensorDataModel
                .Where(s => s.DeviceId == deviceId)
                .AverageAsync(s => s.Temperature);
                
            var avgHumidity = await _dbContext.SensorDataModel
                .Where(s => s.DeviceId == deviceId)
                .AverageAsync(s => s.Humidity);
                
            var minTemp = await _dbContext.SensorDataModel
                .Where(s => s.DeviceId == deviceId)
                .MinAsync(s => s.Temperature);
                
            var maxTemp = await _dbContext.SensorDataModel
                .Where(s => s.DeviceId == deviceId)
                .MaxAsync(s => s.Temperature);
                
            return Ok(new
            {
                DeviceId = device.DeviceId,
                DeviceName = device.DeviceName,
                DataPointCount = dataCount,
                LastRecordTime = lastRecord?.Timestamp,
                AverageTemperature = Math.Round(avgTemperature, 2),
                MinTemperature = minTemp,
                MaxTemperature = maxTemp,
                AverageHumidity = Math.Round(avgHumidity, 2)
            });
        }

        [HttpGet("hourly")]
        public async Task<ActionResult<object>> GetHourlyData()
        {
            // Son 24 saate ait verileri grupla
            var startTime = DateTime.UtcNow.AddHours(-24);
            
            var hourlyData = await _dbContext.SensorDataModel
                .Where(s => s.Timestamp >= startTime)
                .GroupBy(s => new 
                { 
                    Hour = s.Timestamp.Hour,
                    Day = s.Timestamp.Day,
                    Month = s.Timestamp.Month,
                    Year = s.Timestamp.Year
                })
                .Select(g => new
                {
                    Timestamp = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, 0, 0),
                    AvgTemperature = Math.Round(g.Average(s => s.Temperature), 2),
                    AvgHumidity = Math.Round(g.Average(s => s.Humidity), 2),
                    Count = g.Count()
                })
                .OrderBy(r => r.Timestamp)
                .ToListAsync();
                
            return Ok(hourlyData);
        }
    }
}

