using System.ComponentModel.DataAnnotations;

namespace minifab.api.Templates.Models
{
    public class DeviceModel
    {
        public int Id { get; set; }

        [Required]  
        public string DeviceId { get; set; } = null!;  

        [Required]
        public string DeviceName { get; set; } = null!;


    }
}
