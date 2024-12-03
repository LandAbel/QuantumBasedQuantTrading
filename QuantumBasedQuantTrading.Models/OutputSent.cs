using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumBasedQuantTrading.Models
{
    [Table("fu_OutputSent")]
    public class OutputSent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OutputSentID { get; set; }

        [Required]
        public DateTime Published_at { get; set; }

        [Required]
        public double TitleSentiment { get; set; }

        [Required]
        public double ContentSentiment { get; set; }

        [Required]
        public double DescriptionSentiment { get; set; }
    }
}
