using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OBDPI.Server.Data.Models;
using OBDPI.Server.Validation;

namespace OBD
{
    public class ObdTelemetry
    {
        [Key]
        public Guid Id { get; set; } 

        [Required]
        public Guid DeviceId { get; set; }

        [Required]
        public virtual Vehicle Vehicle { get; set; }

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
        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }
    }
}
