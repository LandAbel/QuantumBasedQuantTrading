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
    public class OutputSentRepository : Repository<OutputSent>, IRepository<OutputSent>
    {
        public OutputSentRepository(QuantumBasedQuantTradingDbContext ctx) : base(ctx)
        {

        }
        public override OutputSent Read(int id)
        {
            return ctx.OutputSentSet.FirstOrDefault(t => t.OutputSentID == id);
        }
        public override void Update(OutputSent item)
        {
            var old = Read(item.OutputSentID);
            foreach (var prop in old.GetType().GetProperties())
            {
                if (prop.GetAccessors().FirstOrDefault(t => t.IsVirtual) == null)
                {
                    prop.SetValue(old, prop.GetValue(item));
                }
            }
            ctx.SaveChanges();
        }
    }
}
