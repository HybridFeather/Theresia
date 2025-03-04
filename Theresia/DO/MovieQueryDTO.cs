using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Enums;

namespace Theresia.DO
{
    /// <summary>
    /// 电影查询DTO
    /// </summary>
    public class MovieQueryDTO
    {
        /// <summary>
        /// 番号
        /// </summary>
        public string Code { get; set; } = "";

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = "";
        /// <summary>
        /// 系列id
        /// </summary>
        public int SeriesId { get; set; }
        /// <summary>
        /// 演员id
        /// </summary>
        public int ActorId { get; set; } 
        /// <summary>
        /// 导演id
        /// </summary>
        public int DirectId { get; set; }
        /// <summary>
        /// 喜欢，为2则不根据这个字段查询
        /// </summary>
        public int Like { get; set; } = 2;
        /// <summary>
        /// 最小评分
        /// </summary>
        public double MinRating { get; set; } = 0;
        /// <summary>
        /// 最大评分
        /// </summary>
        public double MaxRating { get; set; } = 5;
        /// <summary>
        /// 最小时间
        /// </summary>
        public string MinReleaseDate { get; set; } = DateTime.MinValue.ToString("yyyy-MM-dd");
        /// <summary>
        /// 最大时间
        /// </summary>
        public string MaxReleaseDate { get; set; } = DateTime.MaxValue.ToString("yyyy-MM-dd");
        /// <summary>
        /// 标签
        /// </summary>
        public List<int>? Tags { get; set; } = new List<int>();
        /// <summary>
        /// 页索引
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 每页大小 MaxValue则把全部数据查出来
        /// </summary>
        public int PageSize { get; set; } = Int32.MaxValue;
        /// <summary>
        /// 排序字段
        /// </summary>
        public SortByEnum SortBy { get; set; } = SortByEnum.ReleaseDate;
        /// <summary>
        /// 排序方式
        /// </summary>
        public SortOrderEnum SortOrder { get; set; } = SortOrderEnum.DESC;


    }
}
