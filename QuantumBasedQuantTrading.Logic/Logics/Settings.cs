using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantumBasedQuantTrading.Logic.Interface;

namespace QuantumBasedQuantTrading.Logic.Logics
{
    public class Settings : ISettings
    {
        public string QvantApi { get; set; }

        public string NewsApi { get; set; }
    }
}
