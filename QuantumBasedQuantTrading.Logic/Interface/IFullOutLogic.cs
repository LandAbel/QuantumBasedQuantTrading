using QuantumBasedQuantTrading.Models;

namespace QuantumBasedQuantTrading.Logic.Interface
{
    public interface IFullOutLogic
    {
        void Create(FulloutAllArticles item);
        void Delete(int id);
        FulloutAllArticles Read(int id);
        IEnumerable<FulloutAllArticles> ReadAll();
        void Update(FulloutAllArticles item);
    }
}