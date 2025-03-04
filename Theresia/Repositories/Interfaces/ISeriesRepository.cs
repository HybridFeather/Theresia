using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Entity;
using Theresia.Enums;

namespace Theresia.Repositories.Interfaces
{
    public interface ISeriesRepository
    {

        /// <summary>
        /// 根据类型查询系列
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<SeriesEntity>> GetSeriesByType(SeriesTypeEnum type);
    }
}
