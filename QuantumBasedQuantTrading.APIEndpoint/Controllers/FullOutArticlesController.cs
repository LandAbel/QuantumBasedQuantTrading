using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuantumBasedQuantTrading.APIEndpoint.Services;
using QuantumBasedQuantTrading.Logic.Interface;
using QuantumBasedQuantTrading.Models;

namespace QuantumBasedQuantTrading.APIEndpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FullOutArticlesController : ControllerBase
    {
        IFullOutLogic fullArticleLogic;
        IHubContext<SignalRHub> hub;


        public FullOutArticlesController(IFullOutLogic fullArticlelogic, IHubContext<SignalRHub> hub)
        {
            this.fullArticleLogic = fullArticlelogic;
            this.hub = hub;
        }

        [HttpGet]
        public IEnumerable<FulloutAllArticles> ReadAll()
        {
            return this.fullArticleLogic.ReadAll();
        }
        [HttpGet("{id}")]
        public FulloutAllArticles Read(int id)
        {
            return this.fullArticleLogic.Read(id);
        }

        [HttpPost]
        public void Create([FromBody] FulloutAllArticles c)
        {
            this.fullArticleLogic.Create(c);
            this.hub.Clients.All.SendAsync("FullArticleCreated", c);
        }

        [HttpPut]
        public void Update([FromBody] FulloutAllArticles c)
        {
            this.fullArticleLogic.Update(c);
            this.hub.Clients.All.SendAsync("FullArticleUpdated", c);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var fullarticleToDelete = (FulloutAllArticles)this.fullArticleLogic.Read(id);
            this.fullArticleLogic.Delete(id);
            this.hub.Clients.All.SendAsync("FullArticleDelete", fullarticleToDelete);
        }
    }
}
