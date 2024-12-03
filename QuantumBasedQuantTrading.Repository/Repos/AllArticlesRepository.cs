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
    public class AllArticlesRepository : Repository<AllArticles>, IRepository<AllArticles>
    {
        public AllArticlesRepository(QuantumBasedQuantTradingDbContext ctx): base(ctx)
        {

        }
        public override AllArticles Read(int id) 
        {
            return ctx.AllArticlesSet.FirstOrDefault(t => t.ArticleID == id);
        }
        public override void Update(AllArticles item)
        {
            var old = Read(item.ArticleID);
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
