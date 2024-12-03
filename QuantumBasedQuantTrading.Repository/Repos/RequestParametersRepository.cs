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
    public class RequestParametersRepository : Repository<RequestParameters>, IRepository<RequestParameters>
    {
        public RequestParametersRepository(QuantumBasedQuantTradingDbContext ctx):base(ctx) { }

        public override RequestParameters Read(int id)
        {
            throw new NotImplementedException();
        }

        public override void Update(RequestParameters item)
        {
            throw new NotImplementedException();
        }
    }
}
