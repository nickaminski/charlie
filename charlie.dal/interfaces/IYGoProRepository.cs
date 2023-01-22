using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IYGoProRepository
    {
        Task<string> GetAllCardsInSet(string setName);
        Task<string> GetCardById(int id);
        Task<string> GetCardByName(string name);
        Task<string> GetAllCardSets();
        IEnumerable<Task> DownloadImages(IEnumerable<CardImage> cardImages, string basePath);
        Task DownloadImages(CardImage cardImage, string basePath);
    }
}
