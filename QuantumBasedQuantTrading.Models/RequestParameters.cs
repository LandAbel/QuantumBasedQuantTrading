using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumBasedQuantTrading.Models
{
    [Table("fu_RequestParameters")]
    public class RequestParameters
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestParamID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string Keyword { get; set; }
    }
}
