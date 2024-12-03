using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuantumBasedQuantTrading.APIEndpoint.Services;
using QuantumBasedQuantTrading.Logic.Interface;
using QuantumBasedQuantTrading.Models;


namespace QuantumBasedQuantTrading.APIEndpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        IHubContext<SignalRHub> hub;

        public IBusinessLogic businessLogic;

        public BusinessController(IHubContext<SignalRHub> hub, IBusinessLogic businessLogic)
        {
            this.hub = hub;
            this.businessLogic = businessLogic;
        }

        [HttpPost("AutoCollectNews")]
        public async Task<IActionResult> AutoCollectNews([FromBody] RequestParameters parameters)
        {
            await this.businessLogic.AutoCollectNews(parameters);
            await this.hub.Clients.All.SendAsync("requestCreated", parameters);

            return Ok(new { message = "Request processed successfully", parameters});
        }
        [HttpPost("MlSubprocess")]
        public async Task<IActionResult> MlSubprocess(string Symbol, float titleSentiment, float contSentiment, float descSentiment,
            float openPrice,float currentHighPrice, float currentLowPrice, float currentVolume)
        {
            try
            {
                await this.businessLogic.MlSubprocess(Symbol, titleSentiment, contSentiment, descSentiment,
                    openPrice, currentHighPrice, currentLowPrice, currentVolume);

                await this.hub.Clients.All.SendAsync("requestCreated", new { Symbol });

                return Ok(new
                {
                    message = "Request processed successfully",
                    Symbol,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing the request.",
                    error = ex.Message
                });
            }
        }
    }
}
