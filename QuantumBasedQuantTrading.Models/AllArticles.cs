using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace QuantumBasedQuantTrading.Models
{
    [Table("fu_AllArticles")]
    public class AllArticles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleID { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public string URL { get; set; }

        [Required]
        public DateTime Published_at { get; set; }
    }
}
