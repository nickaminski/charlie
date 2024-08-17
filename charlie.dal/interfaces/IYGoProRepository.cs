using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IYGoProRepository
    {
        Task<string> GetAllCardsInSetAsync(string setName);
        Task<string> GetCardByIdAsync(int id);
        Task<string> GetCardByNameAsync(string name);
        Task<string> GetAllCardSetsAsync();
        IEnumerable<Task> DownloadImagesAsync(IEnumerable<CardImage> cardImages, string basePath);
        Task DownloadImagesAsync(CardImage cardImage, string basePath);
    }
}
