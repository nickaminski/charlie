using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto.Card;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class CardSetProvider : ICardSetProvider
    {
        private ICardSetRepository _cardSetRepo;
        private IYGoProRepository _ygoRepo;
        private ILogWriter _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        private readonly string cardSetImageUrl = "https://images.ygoprodeck.com/images/sets/";

        public CardSetProvider(IHttpClientFactory clientFactory, IConfiguration configuration, ICardSetRepository cardSetRepo, IYGoProRepository ygoRepo, ILogWriter logger)
        {
            _cardSetRepo = cardSetRepo;
            _ygoRepo = ygoRepo;
            _logger = logger;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<IEnumerable<CardSet>> GetSets(string maxYear)
        {
            var results = await _cardSetRepo.GetAll();

            if (results == null || results.Count() == 0)
            {
                results = await FetchSetsFromYGOPro();
            }
            else
            {
                _logger.ServerLogInfo("Returning cached all card sets from local");
            }

            if (Int32.TryParse(maxYear, out int year))
            {
                results = results.Where(x => x.tcg_date != null && 
                                             Int32.Parse(x.tcg_date.Split("-")[0]) <= year)
                                 .OrderBy(x => x.tcg_date)
                                 .ToList();
            }

            return results;
        }

        public async Task<CardSet> GetSetByName(string name)
        {
            var results = await _cardSetRepo.GetAll();

            if (results == null || results.Count() == 0)
            {
                results = await FetchSetsFromYGOPro();
            }
            else
            {
                _logger.ServerLogInfo("Returning cached all card sets from local");
            }

            return results.FirstOrDefault(x => x.set_name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<IEnumerable<CardSet>> FetchSetsFromYGOPro()
        {
            _logger.ServerLogInfo("Fetching all card sets from YGoPro");
            var cardSetData = await _ygoRepo.GetAllCardSets();
            var cardSets = JsonConvert.DeserializeObject<List<CardSet>>(cardSetData);

            var path = Path.Combine(Directory.GetCurrentDirectory(), _configuration["CardImagesPath"], "sets");
            Directory.CreateDirectory(path);

            try
            {
                // await DownLoadSetImages(cardSets, path);
            }
            catch (Exception e)
            { }

            await _cardSetRepo.WriteCardSetData(cardSetData);

            if (!string.IsNullOrEmpty(cardSetData))
                return JsonConvert.DeserializeObject<List<CardSet>>(cardSetData);
            else
                return new List<CardSet>();
        }

        private Task DownLoadSetImages(IEnumerable<CardSet> cardSets, string path)
        {
            var fetchSets = cardSets.Where(x => !File.Exists(string.Format("{0}/{1}.jpg", path, x.set_code)));
            var httpClient = _clientFactory.CreateClient();
            return Parallel.ForEachAsync(fetchSets, async (cardSet, token) =>
            {
                var response = await httpClient.GetAsync(string.Format("{0}{1}.jpg", cardSetImageUrl, cardSet.set_code), token);
                var contentTypeHeader = response.Content.Headers.FirstOrDefault(x => x.Key.Equals("Content-Type"));
                if (response.IsSuccessStatusCode && (contentTypeHeader.Value.Any(x => x.Equals("image/jpeg")) || contentTypeHeader.Value.Any(x => x.Equals("image/jpg"))))
                {
                    File.WriteAllBytes(string.Format("{0}/{1}.jpg", path, cardSet.set_code),
                                       await response.Content.ReadAsByteArrayAsync());
                }
            });
        }
    }
}
