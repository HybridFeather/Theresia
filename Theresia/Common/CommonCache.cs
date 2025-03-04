using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Entity;
using Theresia.Enums;

namespace Theresia.Common
{
    /// <summary>
    /// 一些公用缓存
    /// </summary>
    public class CommonCache
    {
        /// <summary>
        /// 标签缓存
        /// </summary>
        public static readonly HashSet<TagEntity> TAG_CACHE = new();
        /// <summary>
        /// 卡斯缓存
        /// </summary>
        public static readonly HashSet<CastCrewEntity> CAST_CACHE = new();
        /// <summary>
        /// 系列缓存
        /// </summary>
        public static readonly HashSet<SeriesEntity> SERIES_CACHE = new();
        /// <summary>
        /// 演员缓存
        /// </summary>
        public static readonly HashSet<CastCrewEntity> ACTOR_CACHE = new();
        /// <summary>
        /// 导演缓存
        /// </summary>
        public static readonly HashSet<CastCrewEntity> DIRECTOR_CACHE = new();
        /// <summary>
        /// 媒体元数据缓存
        /// </summary>
        public static readonly Dictionary<string,MediaMetadataEntity> MEDIA_METADATA_CACHE = new(1000);

        public static void RefreshCastCache()
        {
            foreach (var item in CAST_CACHE)
            {
                if (item.Type == (int)CastCrewEnum.Actor)
                {
                    ACTOR_CACHE.Add(item);
                } 
                else if(item.Type == (int)CastCrewEnum.Director)
                {
                    DIRECTOR_CACHE.Add(item);
                }
            }
        }
    }
}
