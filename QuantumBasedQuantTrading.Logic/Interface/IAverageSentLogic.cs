using QuantumBasedQuantTrading.Models;

namespace QuantumBasedQuantTrading.Logic.Interface
{
    public interface IAverageSentLogic
    {
        void Create(AverageSent item);
        void Delete(int id);
        AverageSent Read(int id);
        IEnumerable<AverageSent> ReadAll();
        void Update(AverageSent item);
    }
}