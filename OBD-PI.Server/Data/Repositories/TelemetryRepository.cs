using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OBD;

namespace OBDPI.Server.Data.Repositories
{
    public class TelemetryRepository
    {
        private readonly ObdPiContext _context;

        public TelemetryRepository(ObdPiContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(ObdTelemetry model)
        {
            var vehicle = _context.Vehicles.SingleOrDefault(v => v.DeviceId == model.DeviceId);
            if (vehicle == null) return false;

            _context.ObdTelemetries.Add(model);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ObdTelemetry> GetAsync(Guid id)
        {
            return await _context.ObdTelemetries.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<ObdTelemetry>> GetRangeAsync(int skip, int take)
        {
            return await _context.ObdTelemetries
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<ObdTelemetry>> GetAllAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.Vehicles)
                .ThenInclude(u => u.ObdTelemetries)
                .SingleAsync(u => u.UserName.Equals(username));

            return user.Vehicles.SelectMany(v => v.ObdTelemetries);
        }
    }
}
