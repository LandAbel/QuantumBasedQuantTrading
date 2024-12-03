using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuantumBasedQuantTrading.APIEndpoint.Services;
using QuantumBasedQuantTrading.Logic.Interface;
using QuantumBasedQuantTrading.Models;

namespace QuantumBasedQuantTrading.APIEndpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AllArticlesController : ControllerBase
    {
        IAllArticlesLogic articleLogic;
        IHubContext<SignalRHub> hub;


        public AllArticlesController(IAllArticlesLogic articlelog, IHubContext<SignalRHub> hub)
        {
            this.articleLogic = articlelog;
            this.hub = hub;
        }

        [HttpGet]
        public IEnumerable<AllArticles> ReadAll()
        {
            return this.articleLogic.ReadAll();
        }
        [HttpGet("{id}")]
        public AllArticles Read(int id)
        {
            return this.articleLogic.Read(id);
        }

        [HttpPost]
        public void Create([FromBody] AllArticles c)
        {
            this.articleLogic.Create(c);
            this.hub.Clients.All.SendAsync("ArticleCreated", c);
        }

        [HttpPut]
        public void Update([FromBody] AllArticles c)
        {
            this.articleLogic.Update(c);
            this.hub.Clients.All.SendAsync("ArticleUpdated", c);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var articleToDelete = (AllArticles)this.articleLogic.Read(id);
            this.articleLogic.Delete(id);
            this.hub.Clients.All.SendAsync("ArticleDelete", articleToDelete);
        }
    }
}
