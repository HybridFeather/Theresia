using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Entity;

namespace Theresia.Repositories.Interfaces
{
    public interface IMovieCastRepository
    {
        Task<bool> AddMovieCast(List<MovieCastEntity> list);
    }
}
