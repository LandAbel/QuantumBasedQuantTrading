using NewsAPI.Models;
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
    public class FullOutLogic : IFullOutLogic
    {
        IRepository<FulloutAllArticles> repository;

        public FullOutLogic(IRepository<FulloutAllArticles> repo)
        {
            this.repository = repo;
        }
        public void Create(FulloutAllArticles item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            this.repository.Create(item);
        }
        public void Update(FulloutAllArticles item)
        {
            this.repository.Update(item);
        }
        public void Delete(int id)
        {
            this.repository.Delete(id);
        }
        public IEnumerable<FulloutAllArticles> ReadAll()
        {
            return this.repository.ReadAll();
        }

        public FulloutAllArticles Read(int id)
        {
            var FullArticle = repository.Read(id);
            if (FullArticle == null)
            {
                throw new ArgumentException("There is not such entry");
            }
            return FullArticle;
        }

    }
}
