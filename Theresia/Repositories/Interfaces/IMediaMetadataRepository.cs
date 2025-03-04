using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Entity;

namespace Theresia.Repositories.Interfaces
{
    public interface IMediaMetadataRepository
    {
        Task<MediaMetadataEntity> AddAsync(MediaMetadataEntity entity);
    }
}
