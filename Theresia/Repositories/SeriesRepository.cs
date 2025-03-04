using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Config;
using Theresia.Entity;
using Theresia.Enums;
using Theresia.Repositories.Interfaces;

namespace Theresia.Repositories
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly AppDbContext _context;

        public SeriesRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<SeriesEntity>> GetSeriesByType(SeriesTypeEnum type)
        {
            return await _context.Series.Where(s => s.Type == (int)type).ToListAsync();
        }
    }
}
