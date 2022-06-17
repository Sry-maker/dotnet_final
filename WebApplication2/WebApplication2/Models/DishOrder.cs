using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("dish_order")]
    public class DishOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string order_id { get; set; }
        public DateTime? order_date { get; set; }
        public string? table_id { get; set; }
        public string? customer_id { get; set; }
        public double? price { get; set; }
        public int? state { get; set; }
    }
}
