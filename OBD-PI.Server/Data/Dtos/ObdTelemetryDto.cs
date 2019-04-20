using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDPI.Server.Data.Models;
using OBDPI.Server.Validation;

namespace OBDPI.Server.Data.Dtos
{
    public class ObdTelemetryDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [GuidValidation]
        public Guid DeviceId { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string PidType { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string PidValue { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string Unit { get; set; }

        [Required]
        public long TimeStamp { get; set; }
    }
}
