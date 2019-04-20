using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using OBD;
using OBDPI.Server.Data.Models;

namespace OBDPI.Server.Data
{
    public class ObdPiContext : IdentityDbContext<User>
    {
        public DbSet<ObdTelemetry> ObdTelemetries { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        public ObdPiContext(DbContextOptions<ObdPiContext> options, IConfiguration configuration) : base(options)
        {
        }
    }
}
