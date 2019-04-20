using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBD;

namespace OBDPI.Server.Data.Models
{
    public class Vehicle
    {
        [Key]
        public Guid DeviceId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public virtual ICollection<ObdTelemetry> ObdTelemetries { get; set; } 
    }
}
