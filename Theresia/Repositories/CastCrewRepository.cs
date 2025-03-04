using Microsoft.EntityFrameworkCore;
using Theresia.Config;
using Theresia.Entity;
using Theresia.Enums;
using Theresia.Repositories.Interfaces;

namespace Theresia.Repositories
{
    public class CastCrewRepository : ICastCrewRepository
    {
        private readonly AppDbContext _context;
        public CastCrewRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddCast(CastCrewEntity cast)
        {
            CastCrewEntity? check = await _context.CastCrew.FirstOrDefaultAsync(c => c.OriginalName == cast.OriginalName && c.Type == cast.Type );
            if (check == null)
            {
                _context.CastCrew.Add(cast);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CastCrewEntity?> GetActorByOriginalName(string name)
        {
            CastCrewEntity? check = await _context.CastCrew.FirstOrDefaultAsync(c => c.OriginalName == name && c.Type == (int)CastCrewEnum.Actor);
            return check == null ? null : check;
        }

        public async Task<List<CastCrewEntity>> GetAllCastAsync()
        {
            return await _context.CastCrew.ToListAsync();
        }

        public async Task<CastCrewEntity?> GetCastById(int id)
        {
            CastCrewEntity? check = await _context.CastCrew.FindAsync(id);
            return check == null ? null : check;
        }

        public async Task<CastCrewEntity?> GetDirectorByOriginalName(string name)
        {
            CastCrewEntity? check = await _context.CastCrew.FirstOrDefaultAsync(c => c.OriginalName == name && c.Type == (int)CastCrewEnum.Director);
            return check == null ? null : check;
        }
    }
}
