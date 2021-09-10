using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IYGoProRepository
    {
        Task<string> GetAllCardsInSet(string setName);
        Task<string> GetCardById(int id);
        Task<string> GetAllCardSets();
    }
}
