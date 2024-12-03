using QuantumBasedQuantTrading.Models;

namespace QuantumBasedQuantTrading.Logic.Interface
{
    public interface IBusinessLogic
    {
        Task AutoCollectNews(RequestParameters item);
        Task MlSubprocess(string Symbol, float titleSentiment, float contSentiment, float descSentiment, float open,
            float currentHighPrice, float currentLowPrice, float currentVolume);
    }
}