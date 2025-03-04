using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresia.Entity
{
    [Table("CastCrewPhoto")]
    public class CastCrewPhotoEntity
    {
        public int Id { get; set; }
        [ForeignKey("CastCrew")]
        public int CastId { get; set; }
        /// <summary>
        /// 厂牌
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        ///////导航属性
        //public CastCrewEntity Cast;
    }
}
