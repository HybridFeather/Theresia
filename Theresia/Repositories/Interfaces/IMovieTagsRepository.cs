using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Entity;

namespace Theresia.Repositories.Interfaces
{
    public interface IMovieTagsRepository
    {
        Task<bool> AddMovieTagsAsync(List<MovieTagsEntity> list);
        /// <summary>
        /// 添加关系映射
        /// </summary>
        /// <param name="movieCode"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        bool AddMovieTag(string movieCode, int tagId);
        /// <summary>
        /// 删除关系映射
        /// </summary>
        /// <param name="movieCode"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        bool RemoveMovieTag(string movieCode, int tagId);
    }
}
