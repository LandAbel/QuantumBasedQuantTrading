using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumBasedQuantTrading.Models
{

    [Table("fu_FulloutAllArticles")]
    public class FulloutAllArticles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleFullID { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime Published_at { get; set; }

        public string? Content { get; set; }
    }
}
