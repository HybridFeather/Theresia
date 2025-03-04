using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresia.Entity
{
    [Table("Setting")]
    public class SettingEntity
    {
        [Key] // 主键
        public int Id { get; set; }
        /// <summary>
        /// 设置项
        /// </summary>
        [Required] // 必填
        public string Key { get; set; }
        /// <summary>
        /// 设置值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string Default {  get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
    }
}
