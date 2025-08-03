using System;
using System.ComponentModel.DataAnnotations;

namespace minifab.api.Templates.Models
{
    public class SensorDataModel
    {
        public int Id { get; set; }

        [Required]
        public string DeviceId { get; set; } = null!; 

        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public int Voltage { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
