using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Theresia.Entity
{
    [Table("Series")]
    public class SeriesEntity
    {
        [Key] // 主键
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required] // 必填
        public string Name { get; set; }
        /// <summary>
        /// 1电影2视频3游戏
        /// </summary>
        public int Type { get; set; }

        public override bool Equals(object? obj)
        {
            SeriesEntity? other = obj as SeriesEntity;
            if (obj == null || other == null)
                return false;

            // 根据属性比较两个对象是否相等
            return this.Id == other.Id
                && this.Name == other.Name
                && this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id,Name, Type);
        }
    }

}
