using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("dish")]
    public class Dish
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? dish_id { get; set; }
        public string? name { get; set; }
        public double? price { get; set; }
        public string? url { get; set; }
        public string? info { get; set; }
        public int? count { get; set; }
                
    }
}
