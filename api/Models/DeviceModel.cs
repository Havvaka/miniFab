using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniFab.Api.Models
{
    [Table("Device")]
    public class DeviceModel
    {
        public int Id { get; set; }

        [Required]
        public string DeviceId { get; set; } = null!;

        [Required]
        public string DeviceName { get; set; } = null!;


    }
}
