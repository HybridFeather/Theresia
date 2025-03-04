using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresia.Entity
{
    [Table("Movie")]
    public class MovieEntity
    {
        public int Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// 番号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 所属系列
        /// </summary>
        public int Series { get; set; }
        /// <summary>
        /// 制造商
        /// </summary>
        public string? Manufacturer { get; set; }
        /// <summary>
        /// 发行商
        /// </summary>
        public string? Distributor { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        public string? ReleaseDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Rating { get; set; }
        /// <summary>
        /// 个人评价
        /// </summary>
        public string? Comment { get; set; }
        /// <summary>
        /// 喜欢
        /// </summary>
        public int Like { get; set; }
        /// <summary>
        /// 更新状态
        /// </summary>
        public int Status { get; set; }


        ////导航属性
        //public ICollection<TagEntity> Tags { get; set; }

        //public ICollection<CastCrewEntity> Cast { get; set; }
    }
}
