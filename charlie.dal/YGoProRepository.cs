using charlie.dal.interfaces;
using charlie.dto.Card;
using Microsoft.Extensions.Configuration;
using System;
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
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly CancellationTokenSource _cancellationTokenSource;

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
            var client = _clientFactory.CreateClient();
            var url = string.Format("{0}?set={1}", cardUrl, setName);
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

        public async Task DownloadImages(IEnumerable<CardImage> cardImages, string basePath)
        {
            var client = _clientFactory.CreateClient();
            var tasks = cardImages.SelectMany(x => new List<Task<HttpResponseMessage>>() { client.GetAsync(x.image_url), client.GetAsync(x.image_url_small) });

            var results = await Task.WhenAll(tasks);

            for(var i = 0; i < cardImages.Count(); i++)
            {
                var image = results[i * 2];
                var smallImage = results[(i * 2) + 1];

                var contentTypeHeader = image.Content.Headers.FirstOrDefault(x => x.Key.Equals("Content-Type"));
                var meta = cardImages.ElementAt(i);
                if (image.IsSuccessStatusCode && contentTypeHeader.Value.Any(x => x.Equals("image/jpeg")))
                {
                    File.WriteAllBytes(string.Format("{0}/cards/{1}.jpg", basePath, meta.id), 
                                       await image.Content.ReadAsByteArrayAsync());
                }

                contentTypeHeader = smallImage.Content.Headers.FirstOrDefault(x => x.Key.Equals("Content-Type"));
                if (smallImage.IsSuccessStatusCode && contentTypeHeader.Value.Any(x => x.Equals("image/jpeg")))
                {
                    File.WriteAllBytes(string.Format("{0}/cards_small/{1}.jpg", basePath, meta.id), 
                                       await smallImage.Content.ReadAsByteArrayAsync());
                }
            }
        }
    }
}
