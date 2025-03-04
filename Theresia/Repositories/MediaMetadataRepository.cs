using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Config;
using Theresia.Entity;
using Theresia.Repositories.Interfaces;

namespace Theresia.Repositories
{
    public class MediaMetadataRepository : IMediaMetadataRepository
    {

        private readonly AppDbContext _context;
        public MediaMetadataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MediaMetadataEntity> AddAsync(MediaMetadataEntity entity)
        {
            try
            {
                await _context.MediaMetadata.AddAsync(entity);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"媒体元数据添加失败，异常信息{ex}");
            }
            return entity;
        }
    }
}
