using QuantumBasedQuantTrading.Logic.Interface;
using QuantumBasedQuantTrading.Models;
using QuantumBasedQuantTrading.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumBasedQuantTrading.Logic.Logics
{
    public class AllArticlesLogic : IAllArticlesLogic
    {
        IRepository<AllArticles> repository;

        public AllArticlesLogic(IRepository<AllArticles> repo)
        {
            this.repository = repo;
        }
        public void Create(AllArticles item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            this.repository.Create(item);
        }
        public void Update(AllArticles item)
        {
            this.repository.Update(item);
        }
        public void Delete(int id)
        {
            this.repository.Delete(id);
        }
        public IEnumerable<AllArticles> ReadAll()
        {
            return this.repository.ReadAll();
        }

        public AllArticles Read(int id)
        {
            var article = repository.Read(id);
            if (article == null)
            {
                throw new ArgumentException("There is not such entry");
            }
            return article;
        }
    }
}
