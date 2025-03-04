using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Entity;

namespace Theresia.Repositories.Interfaces
{
    public interface ICastCrewRepository
    {
        Task<bool> AddCast(CastCrewEntity cast);

        Task<CastCrewEntity?> GetActorByOriginalName(string name);

        Task<CastCrewEntity?> GetDirectorByOriginalName(string name);

        Task<CastCrewEntity?> GetCastById(int id);

        Task<List<CastCrewEntity>> GetAllCastAsync();

    }
}
