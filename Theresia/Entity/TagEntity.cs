using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresia.Entity
{
    [Table("Tag")]
    public class TagEntity
    {
        [Key] // 主键
        public int Id { get; set; }

        [Required] // 必填
        public string Name { get; set; }


        public override bool Equals(object? obj)
        {
            TagEntity? other = obj as TagEntity;
            if (obj == null || other == null)
                return false;

            // 根据属性比较两个对象是否相等
            return this.Id == other.Id
                && this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

    }
}
