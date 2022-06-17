using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("dining_table")]
    public class DiningTable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? table_id { get; set; }
        public int? capacity { get; set; }
        public string? state { get; set; }
    }
}
