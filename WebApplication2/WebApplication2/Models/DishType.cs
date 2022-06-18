using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("dish_type")]
    public class DishType
    {
        [Key, Column(name: "dish_id")]
        public string? dish_id { get; set; }
        
        [Key, Column(name: "dish_type")]
        public string? dish_type { get; set; }
    }
}
