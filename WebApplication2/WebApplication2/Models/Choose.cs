using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("choose")]
    public class Choose
    {
        [Key,Column(name:"order_id")]
        public string? order_id { get; set; }
        
        [Key, Column(name: "dish_id")]
        public string? dish_id { get; set; }
        public int? num { get; set; }
        
        [Key, Column(name: "order_date")]
        public DateTime? order_date { get; set; }

    }
}
