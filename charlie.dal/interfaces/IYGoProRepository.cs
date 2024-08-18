using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface IYGoProRepository
    {
        Task<string> GetAllCardsInSetAsync(string setName, CancellationToken token);
        Task<string> GetCardByIdAsync(int id, CancellationToken token);
        Task<string> GetCardByNameAsync(string name, CancellationToken token);
        Task<string> GetAllCardSetsAsync(CancellationToken token);
        IEnumerable<Task> DownloadImagesAsync(IEnumerable<CardImage> cardImages, string basePath);
        Task DownloadImagesAsync(CardImage cardImage, string basePath);
    }
}
