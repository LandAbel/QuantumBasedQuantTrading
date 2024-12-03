using QuantumBasedQuantTrading.Models;
using QuantumBasedQuantTrading.Repository.Data;
using QuantumBasedQuantTrading.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumBasedQuantTrading.Repository.Repos
{
    public class MachineLearningModelDataRepository : Repository<MachineLearningModelData>, IRepository<MachineLearningModelData>
    {
        public MachineLearningModelDataRepository(QuantumBasedQuantTradingDbContext ctx):base(ctx) { }

        public override MachineLearningModelData Read(int id)
        {
            throw new NotImplementedException();
        }

        public override void Update(MachineLearningModelData item)
        {
            throw new NotImplementedException();
        }
    }
}
