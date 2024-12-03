using QuantumBasedQuantTrading.Models;

namespace QuantumBasedQuantTrading.Logic.Interface
{
    public interface IAllArticlesLogic
    {
        void Create(AllArticles item);
        void Delete(int id);
        AllArticles Read(int id);
        IEnumerable<AllArticles> ReadAll();
        void Update(AllArticles item);
    }
}