using charlie.dal.interfaces;
using charlie.dto.Card;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace charlie.dal.json_repos
{
    public class YGoProRepository : IYGoProRepository
    {
        private string cardSetUrl = "https://db.ygoprodeck.com/api/v7/cardsets.php";
        private string cardUrl = "https://db.ygoprodeck.com/api/v7/cardinfo.php";
        private string ygoImageUrl = "https://images.ygoprodeck.com/images/";
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private HttpClient _httpClient;

        public YGoProRepository(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(30000);
        }

        public async Task<string> GetAllCardSets()
        {
            var client = _clientFactory.CreateClient();
            var results = await client.GetAsync(cardSetUrl, _cancellationTokenSource.Token);

            if (results.IsSuccessStatusCode)
                return await results.Content.ReadAsStringAsync();

            return string.Empty;
        }

        public async Task<string> GetAllCardsInSet(string setName)
        {
            // https://db.ygoprodeck.com/api/v7/cardinfo.php?cardset=Legendary%20Duelists%3A%20Duels%20From%20the%20Deep
            var client = _clientFactory.CreateClient();
            var url = string.Format("{0}?cardset={1}", cardUrl, setName);
            var results = await client.GetAsync(url, _cancellationTokenSource.Token);

            if (results.IsSuccessStatusCode)
                return await results.Content.ReadAsStringAsync();

            return string.Empty;
        }

        public async Task<string> GetCardById(int id)
        {
            var client = _clientFactory.CreateClient();
            var url = string.Format("{0}?id={1}", cardUrl, id);
            var results = await client.GetAsync(url, _cancellationTokenSource.Token);

            if (results.IsSuccessStatusCode)
            {
                return await results.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }

        public async Task<string> GetCardByName(string name)
        {
            var client = _clientFactory.CreateClient();
            var url = string.Format("{0}?name={1}", cardUrl, name);
            var results = await client.GetAsync(url, _cancellationTokenSource.Token);

            if (results.IsSuccessStatusCode)
            {
                return await results.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }

        public IEnumerable<Task> DownloadImages(IEnumerable<CardImage> cardImages, string basePath)
        {
            if (_httpClient == null)
                _httpClient = _clientFactory.CreateClient();

            var tasks = new List<Task>();

            foreach (var cardImage in cardImages)
            {
                tasks.Add(DownloadImages(cardImage, basePath));
            }

            return tasks;
        }

        public Task DownloadImages(CardImage cardImage, string basePath)
        {
            if (_httpClient == null)
                _httpClient = _clientFactory.CreateClient();

            var tasks = new List<Task>();

            if (!File.Exists(string.Format("{0}/{1}/{2}.jpg", basePath, "cards", cardImage.id)))
            {
                tasks.Add(
                    _httpClient.GetAsync(string.Format("{0}cards/{1}.jpg", ygoImageUrl, cardImage.id))
                      .ContinueWith(x =>
                        WriteImage(x.Result, string.Format("{0}/{1}/{2}.jpg", basePath, "cards", cardImage.id))
                      )
                );
            }

            if (!File.Exists(string.Format("{0}/{1}/{2}.jpg", basePath, "cards_small", cardImage.id)))
            {
                tasks.Add(
                    _httpClient.GetAsync(string.Format("{0}cards_small/{1}.jpg", ygoImageUrl, cardImage.id))
                      .ContinueWith(x =>
                        WriteImage(x.Result, string.Format("{0}/{1}/{2}.jpg", basePath, "cards_small", cardImage.id))
                      )
                );
            }

            if (!File.Exists(string.Format("{0}/{1}/{2}.jpg", basePath, "cards_cropped", cardImage.id)))
            {
                tasks.Add(
                    _httpClient.GetAsync(string.Format("{0}cards_cropped/{1}.jpg", ygoImageUrl, cardImage.id))
                      .ContinueWith(x =>
                        WriteImage(x.Result, string.Format("{0}/{1}/{2}.jpg", basePath, "cards_cropped", cardImage.id))
                      )
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
