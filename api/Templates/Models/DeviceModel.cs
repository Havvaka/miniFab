using System.ComponentModel.DataAnnotations;

namespace minifab.api.Templates.Models
{
    public class DeviceModel
    {
        public int Id { get; set; }

        [Required]  
        public string DeviceId { get; set; } = null!;  // "null!" ile C# null uyarısını önleriz

        [Required]
        public string DeviceName { get; set; } = null!;


    }
}
