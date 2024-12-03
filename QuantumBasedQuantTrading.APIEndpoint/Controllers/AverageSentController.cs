using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuantumBasedQuantTrading.APIEndpoint.Services;
using QuantumBasedQuantTrading.Logic.Interface;
using QuantumBasedQuantTrading.Models;

namespace QuantumBasedQuantTrading.APIEndpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AverageSentController : ControllerBase
    {
        IAverageSentLogic AverageSentLogic;
        IHubContext<SignalRHub> hub;


        public AverageSentController(IAverageSentLogic avgsentlogic, IHubContext<SignalRHub> hub)
        {
            this.AverageSentLogic = avgsentlogic;
            this.hub = hub;
        }

        [HttpGet]
        public IEnumerable<AverageSent> ReadAll()
        {
            return this.AverageSentLogic.ReadAll();
        }
        [HttpGet("{id}")]
        public AverageSent Read(int id)
        {
            return this.AverageSentLogic.Read(id);
        }

        [HttpPost]
        public void Create([FromBody] AverageSent c)
        {
            this.AverageSentLogic.Create(c);
            this.hub.Clients.All.SendAsync("AvgSentCreated", c);
        }

        [HttpPut]
        public void Update([FromBody] AverageSent c)
        {
            this.AverageSentLogic.Update(c);
            this.hub.Clients.All.SendAsync("AvgSentUpdated", c);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var avgsentToDelete = (AverageSent)this.AverageSentLogic.Read(id);
            this.AverageSentLogic.Delete(id);
            this.hub.Clients.All.SendAsync("AvgSentDelete", avgsentToDelete);
        }
    }
}
