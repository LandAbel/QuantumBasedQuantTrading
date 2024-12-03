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
    public class AverageSentRepository : Repository<AverageSent>, IRepository<AverageSent>
    {
        public AverageSentRepository(QuantumBasedQuantTradingDbContext ctx) : base(ctx)
        {

        }
        public override AverageSent Read(int id)
        {
            return ctx.AverageSentSet.FirstOrDefault(t => t.AverageSentID == id);
        }
        public override void Update(AverageSent item)
        {
            var old = Read(item.AverageSentID);
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
