using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Config;
using Theresia.Entity;
using Theresia.Repositories.Interfaces;

namespace Theresia.Repositories
{
    public class CastCrewPhotoRepository : ICastCrewPhotoRepository
    {
        private readonly AppDbContext _context;
        public CastCrewPhotoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddPhoto(CastCrewPhotoEntity entity)
        {
            CastCrewPhotoEntity? check = await _context.CastCrewPhoto.FirstOrDefaultAsync(c => c.CastId == entity.CastId && c.Label == entity.Label);
            if (check == null)
            {
                _context.CastCrewPhoto.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CastCrewPhotoEntity?> GetPhotoByCastIdAndLabel(int castId,string labelName)
        {

            List<CastCrewPhotoEntity> check = await _context.CastCrewPhoto.Where(c => c.CastId == castId).ToListAsync();

            if(check.Count == 0)
            {
                return null;
            }

            if (string.IsNullOrEmpty(labelName))
            {
                return check.Where(c => c.Label == labelName).First() ?? check.First();
            }
            else
            {
                return check.First();
            }
        }
    }
}
