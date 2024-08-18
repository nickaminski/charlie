using charlie.dal.interfaces;
using charlie.dto.Card;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace charlie.dal
{
    public class YGoProRepository : IYGoProRepository
    {
        private string cardSetUrl = "https://db.ygoprodeck.com/api/v7/cardsets.php";
        private string cardUrl = "https://db.ygoprodeck.com/api/v7/cardinfo.php";
        private string ygoImageUrl = "https://images.ygoprodeck.com/images/";
        private readonly IHttpClientFactory _clientFactory;

        public YGoProRepository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetAllCardSetsAsync(CancellationToken token)
        {
            var client = _clientFactory.CreateClient();
            var results = await client.GetAsync(cardSetUrl, token);

            if (results.IsSuccessStatusCode)
                return await results.Content.ReadAsStringAsync();

            return string.Empty;
        }

        public async Task<string> GetAllCardsInSetAsync(string setName, CancellationToken token)
        {
            var client = _clientFactory.CreateClient();
            var url = string.Format("{0}?cardset={1}", cardUrl, setName);
            var results = await client.GetAsync(url, token);

            if (results.IsSuccessStatusCode)
                return await results.Content.ReadAsStringAsync();

            return string.Empty;
        }

        public async Task<string> GetCardByIdAsync(int id, CancellationToken token)
        {
            var client = _clientFactory.CreateClient();
            var url = string.Format("{0}?id={1}", cardUrl, id);
            var results = await client.GetAsync(url, token);

            if (results.IsSuccessStatusCode)
            {
                return await results.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }

        public async Task<string> GetCardByNameAsync(string name, CancellationToken token)
        {
            var client = _clientFactory.CreateClient();
            var url = string.Format("{0}?name={1}", cardUrl, name);
            var results = await client.GetAsync(url, token);

            if (results.IsSuccessStatusCode)
            {
                return await results.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }

        public IEnumerable<Task> DownloadImagesAsync(IEnumerable<CardImage> cardImages, string basePath)
        {
            var tasks = new List<Task>();

            foreach (var cardImage in cardImages)
            {
                tasks.Add(DownloadImagesAsync(cardImage, basePath));
            }

            return tasks;
        }

        public Task DownloadImagesAsync(CardImage cardImage, string basePath)
        {
            var tasks = new List<Task>();

            var client = _clientFactory.CreateClient();
            if (!File.Exists(string.Format("{0}/{1}/{2}.jpg", basePath, "cards", cardImage.id)))
            {
                    tasks.Add(
                        client.GetAsync(string.Format("{0}cards/{1}.jpg", ygoImageUrl, cardImage.id))
                            .ContinueWith(x =>
                                WriteImage(x.Result, string.Format("{0}/{1}/{2}.jpg", basePath, "cards", cardImage.id)))
                    );
            }

            if (!File.Exists(string.Format("{0}/{1}/{2}.jpg", basePath, "cards_small", cardImage.id)))
            {
                tasks.Add(
                    client.GetAsync(string.Format("{0}cards_small/{1}.jpg", ygoImageUrl, cardImage.id))
                        .ContinueWith(x =>
                            WriteImage(x.Result, string.Format("{0}/{1}/{2}.jpg", basePath, "cards_small", cardImage.id)))
                );
            }

            if (!File.Exists(string.Format("{0}/{1}/{2}.jpg", basePath, "cards_cropped", cardImage.id)))
            {
                tasks.Add(
                    client.GetAsync(string.Format("{0}cards_cropped/{1}.jpg", ygoImageUrl, cardImage.id))
                        .ContinueWith(x =>
                            WriteImage(x.Result, string.Format("{0}/{1}/{2}.jpg", basePath, "cards_cropped", cardImage.id)))
                );
            }

            return Task.WhenAll(tasks);
        }

        private async Task WriteImage(HttpResponseMessage x, string writePath)
        {
            var contentTypeHeader = x.Content.Headers.FirstOrDefault(x => x.Key.Equals("Content-Type"));
            if (x.IsSuccessStatusCode && (contentTypeHeader.Value.Any(x => x.Equals("image/jpeg")) || contentTypeHeader.Value.Any(x => x.Equals("image/jpg"))))
            {
                File.WriteAllBytes(writePath,
                                   await x.Content.ReadAsByteArrayAsync());
            }
        }
    }
}
