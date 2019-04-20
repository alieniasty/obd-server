using System;
using System.ComponentModel.DataAnnotations;
using OBDPI.Server.Validation;

namespace OBDPI.Server.Data.Dtos
{
    public class VehicleDto
    {
        [Required]
        [GuidValidation]
        public Guid DeviceId { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(20)]
        public string Brand { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(20)]
        public string Model { get; set; }
    }
}
