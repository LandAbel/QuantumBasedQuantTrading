using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumBasedQuantTrading.Models
{
    [Table("fu_AverageSent")]
    public class AverageSent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AverageSentID { get; set; }

        [Required]
        public DateTime Published_at { get; set; }

        [Required]
        public double AvgTitleSentiment { get; set; }

        [Required]
        public double AvgContentSentiment { get; set; }

        [Required]
        public double AvgDescriptionSentiment { get; set; }
    }
}
