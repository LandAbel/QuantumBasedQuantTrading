using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumBasedQuantTrading.Models
{
    [Table("fu_MLParameters")]
    public  class MachineLearningModelData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MLResultID { get; set; }
        public double trainMAE { get; set; }
        public double valMAE { get; set; }
        public double testMAE { get; set; }
        public double predictedValue { get; set; }
        public string Symbol { get; set; }
    }
}
