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
    public class AverageSentLogic : IAverageSentLogic
    {
        IRepository<AverageSent> repository;

        public AverageSentLogic(IRepository<AverageSent> repo)
        {
            this.repository = repo;
        }
        public void Create(AverageSent item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            this.repository.Create(item);
        }
        public void Update(AverageSent item)
        {
            this.repository.Update(item);
        }
        public void Delete(int id)
        {
            this.repository.Delete(id);
        }
        public IEnumerable<AverageSent> ReadAll()
        {
            return this.repository.ReadAll();
        }

        public AverageSent Read(int id)
        {
            var avgsent = repository.Read(id);
            if (avgsent == null)
            {
                throw new ArgumentException("There is not such entry");
            }
            return avgsent;
        }

    }
}
