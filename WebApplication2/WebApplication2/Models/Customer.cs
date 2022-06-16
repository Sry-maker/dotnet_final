using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("customer")]
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string customer_id { get; set; }
        public string? customer_name { get; set; }
        public DateTime? birthday { get; set; }
        public string? password { get; set; }
        public string? phone { get; set; }
        public string? url { get; set; }
        public double? credit { get; set; }
    }
}
