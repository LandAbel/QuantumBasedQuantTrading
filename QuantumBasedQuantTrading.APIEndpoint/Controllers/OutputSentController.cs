using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuantumBasedQuantTrading.APIEndpoint.Services;
using QuantumBasedQuantTrading.Logic.Interface;
using QuantumBasedQuantTrading.Models;

namespace QuantumBasedQuantTrading.APIEndpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OutputSentController : ControllerBase
    {
        IOutputSentLogic OutputSentLogic;
        IHubContext<SignalRHub> hub;


        public OutputSentController(IOutputSentLogic outsentlogic, IHubContext<SignalRHub> hub)
        {
            this.OutputSentLogic = outsentlogic;
            this.hub = hub;
        }

        [HttpGet]
        public IEnumerable<OutputSent> ReadAll()
        {
            return this.OutputSentLogic.ReadAll();
        }
        [HttpGet("{id}")]
        public OutputSent Read(int id)
        {
            return this.OutputSentLogic.Read(id);
        }

        [HttpPost]
        public void Create([FromBody] OutputSent c)
        {
            this.OutputSentLogic.Create(c);
            this.hub.Clients.All.SendAsync("OutSentCreated", c);
        }

        [HttpPut]
        public void Update([FromBody] OutputSent c)
        {
            this.OutputSentLogic.Update(c);
            this.hub.Clients.All.SendAsync("OutSentUpdated", c);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var outsentToDelete = (OutputSent)this.OutputSentLogic.Read(id);
            this.OutputSentLogic.Delete(id);
            this.hub.Clients.All.SendAsync("OutSentDelete", outsentToDelete);
        }
    }
}
