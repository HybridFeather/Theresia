using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.DO;
using Theresia.Entity;

namespace Theresia.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<MovieEntity?> GetMovieByCodeAsync(string code);

        Task<bool> AddMovieAsync(MovieEntity entity);

        Task<bool> UpdateAsync(MovieEntity entity);

        Task<List<MovieEntity>> GetAllMovieAsync();

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="queryDTO"></param>
        /// <returns></returns>
        Task<List<MovieEntity>> QueryByConditions(MovieQueryDTO queryDTO);

    }
}
