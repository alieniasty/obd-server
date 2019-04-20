using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OBDPI.Server.Data.Models;

namespace OBDPI.Server.Data.Repositories
{
    public class VehicleRepository
    {
        private readonly ObdPiContext _context;

        public VehicleRepository(ObdPiContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Vehicle model, string username)
        {
            if (_context.Vehicles.Any(v => v.DeviceId == model.DeviceId)) return false;

            _context.Users
                .Include(u => u.Vehicles)
                .Single(u => u.UserName.Equals(username))
                .Vehicles
                .Add(model);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Vehicle> GetAsync(Guid id)
        {
            return await _context.Vehicles.SingleOrDefaultAsync(t => t.DeviceId == id);
        }

        public async Task<IEnumerable<Vehicle>> GetRangeAsync(int skip, int take)
        {
            return await _context.Vehicles
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public IEnumerable<Vehicle> GetAll(string username)
        {
            return _context.Users
                .Include(u => u.Vehicles)
                .Single(u => u.UserName.Equals(username))
                .Vehicles
                .ToList();
        }
    }
}
