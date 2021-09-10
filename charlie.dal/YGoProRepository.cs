using charlie.dal.interfaces;
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
        private readonly CancellationTokenSource _cancellationTokenSource;

        public YGoProRepository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
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
                return await results.Content.ReadAsStringAsync();

            return string.Empty;
        }
    }
}
