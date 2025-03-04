using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresia.Entity
{
    [Table("MovieCast")]
    public class MovieCastEntity
    {
        public int Id { get; set; }
        [ForeignKey("CastCrew")]
        public int CastId { get; set; }
        [ForeignKey("Movie")]
        public string Code { get; set; }

        /////////导航属性
        //public MovieEntity Movie;
        //public CastCrewEntity Cast;
    }
}
