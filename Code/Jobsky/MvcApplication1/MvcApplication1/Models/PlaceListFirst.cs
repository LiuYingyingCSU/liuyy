using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    [Table("PlaceListFirst")]
    public class PlaceListFirst
    {
        [Required(ErrorMessage = "*校区不能为空")]
        public int PlaceFirstID { get; set; }

        [Required(ErrorMessage = "*校区不能为空")]
        [StringLength(20, ErrorMessage = "长度必须少于20个字符")]
        public string PlaceName { get; set; }
    }
}