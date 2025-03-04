using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Theresia.Entity
{
    [Table("CastCrew")]
    public class CastCrewEntity
    {
        [Key] // 主键
        public int Id { get; set; }
        /// <summary>
        /// 原名
        /// </summary>
        [Required] // 必填
        public string OriginalName { get; set; }
        /// <summary>
        /// 中文名
        /// </summary>
        public string? ChineseName { get; set; }
        /// <summary>
        /// 罗马音
        /// </summary>
        public string? RomanizedName { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string? Birthday {  get; set; }
        /// <summary>
        /// 性别，0女1男
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 0导演1演员
        /// </summary>
        public int Type { get; set;}
        /// <summary>
        /// 胸围
        /// </summary>
        public double Bust {  get; set; }
        /// <summary>
        /// 腰围
        /// </summary>
        public double Waist { get; set; }
        /// <summary>
        /// 臀围
        /// </summary>
        public double Hip { get; set; }
        /// <summary>
        /// 罩杯
        /// </summary>
        public string? Cup {  get; set; }
        /// <summary>
        /// 身高
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 入行日期
        /// </summary>
        public string? EntryDate { get; set; }


        public override bool Equals(object? obj)
        {
            CastCrewEntity? other = obj as CastCrewEntity;
            if (obj == null || other == null)
                return false;

            // 根据属性比较两个对象是否相等
            return this.Id == other.Id
                && this.OriginalName == other.OriginalName
                && this.RomanizedName == other.RomanizedName
                && this.ChineseName == other.ChineseName
                && this.Birthday == other.Birthday
                && this.Gender == other.Gender
                && this.Type == other.Type
                && this.Bust == other.Bust
                && this.Waist == other.Waist
                && this.Hip == other.Hip
                && this.Cup == other.Cup
                && this.Height == other.Height
                && this.EntryDate == other.EntryDate;
        }

        public override int GetHashCode()
        {
            // HashCode.Combine 是 C# 7.0 引入的一个方便方法来结合多个值生成一个哈希值
            return HashCode.Combine(Id, OriginalName, RomanizedName, Birthday, Gender, Type,Height, EntryDate);
        }
    }
}
