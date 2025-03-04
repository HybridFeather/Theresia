using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Entity;

namespace Theresia.Repositories.Interfaces
{
    public interface ICastCrewPhotoRepository
    {
        Task<bool> AddPhoto(CastCrewPhotoEntity photo);

        Task<CastCrewPhotoEntity?> GetPhotoByCastIdAndLabel(int castId,string labelName);
    }
}
