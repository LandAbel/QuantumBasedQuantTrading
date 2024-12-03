using QuantumBasedQuantTrading.Models;

namespace QuantumBasedQuantTrading.Logic.Interface
{
    public interface IOutputSentLogic
    {
        void Create(OutputSent item);
        void Delete(int id);
        OutputSent Read(int id);
        IEnumerable<OutputSent> ReadAll();
        void Update(OutputSent item);
    }
}