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
    public class FullOutRepository : Repository<FulloutAllArticles>, IRepository<FulloutAllArticles>
    {
        public FullOutRepository(QuantumBasedQuantTradingDbContext ctx) : base(ctx)
        {

        }
        public override FulloutAllArticles Read(int id)
        {
            return ctx.FulloutAllArticlesSet.FirstOrDefault(t => t.ArticleFullID == id);
        }
        public override void Update(FulloutAllArticles item)
        {
            var old = Read(item.ArticleFullID);
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
