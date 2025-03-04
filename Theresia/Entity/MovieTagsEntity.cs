using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresia.Entity
{
    [Table("MovieTags")]
    public class MovieTagsEntity
    {
        public int Id { get; set; }
        [ForeignKey("Movie")]
        public string Code { get; set; }
        [ForeignKey("Tag")]
        public int TagId { get; set; }


        ////////导航属性
        //public MovieEntity Movie;
        //public TagEntity Tag;
    }
}
