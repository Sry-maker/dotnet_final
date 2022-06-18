using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("reservation")]
    public class Reservation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? reservation_id { get; set; }
        public DateTime? reservation_date { get; set; }
        public string? start_time { get; set; }
        public string? end_time { get; set; }
        public string? table_id { get; set; }
        public string? customer_id { get; set; }
        public int? state { get; set; }
    }
}
