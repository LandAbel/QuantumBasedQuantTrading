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
    public class OutputSentLogic : IOutputSentLogic
    {
        IRepository<OutputSent> repository;

        public OutputSentLogic(IRepository<OutputSent> repo)
        {
            this.repository = repo;
        }
        public void Create(OutputSent item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            this.repository.Create(item);
        }
        public void Update(OutputSent item)
        {
            this.repository.Update(item);
        }
        public void Delete(int id)
        {
            this.repository.Delete(id);
        }
        public IEnumerable<OutputSent> ReadAll()
        {
            return this.repository.ReadAll();
        }

        public OutputSent Read(int id)
        {
            var sentiment = repository.Read(id);
            if (sentiment == null)
            {
                throw new ArgumentException("There is not such entry");
            }
            return sentiment;
        }

    }
}
