using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("evaluation")]
    public class Evaluation
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? evaluation_id { get; set; }
        public DateTime? date { get; set; }
        public string? customer_id { get; set; }
        public string? text { get; set; }
        public int? score { get; set; }
    }
}
