using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto.Card;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
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

        private readonly string cardSetImageUrl = "https://ygoprodeck.com/pics_sets/";

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
                _logger.ServerLogInfo("Fetching all card sets from YGoPro");
                var cardSetData = await _ygoRepo.GetAllCardSets();
                var cardSets = JsonConvert.DeserializeObject<List<CardSet>>(cardSetData);

                var path = Path.Combine(Directory.GetCurrentDirectory(), _configuration["CardImagesPath"], "pic_sets");
                Directory.CreateDirectory(path);

                await DownLoadSetImage(cardSets, path);

                await _cardSetRepo.WriteCardSetData(cardSetData);

                if (!string.IsNullOrEmpty(cardSetData))
                    results = JsonConvert.DeserializeObject<List<CardSet>>(cardSetData);
                else
                    results = new List<CardSet>();
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

        private async Task DownLoadSetImage(IEnumerable<CardSet> cardSets, string path)
        {
            var httpClient = _clientFactory.CreateClient();
            await Parallel.ForEachAsync(cardSets, async (cardSet, token) =>
            {
                var response = await httpClient.GetAsync(string.Format("{0}{1}.jpg", cardSetImageUrl, cardSet.set_code), token);
                var contentTypeHeader = response.Content.Headers.FirstOrDefault(x => x.Key.Equals("Content-Type"));
                if (response.IsSuccessStatusCode && contentTypeHeader.Value.Equals("image/jpg"))
                {
                    File.WriteAllBytes(string.Format("{0}/pics_sets/{1}.jpg", path, cardSet.set_code),
                                       await response.Content.ReadAsByteArrayAsync());
                }
            });
        }
    }
}
