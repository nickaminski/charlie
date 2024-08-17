using charlie.bll.interfaces;
using charlie.common.exceptions;
using charlie.dal.interfaces;
using charlie.dto.Card;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class CardProvider : ICardProvider
    {
        private ICardRepository _cardRepo;
        private IYGoProRepository _ygoRepo;
        private ICardSetProvider _setProv;
        private ILogWriter _logger;
        private IConfiguration _configuration;

        private const double secretRatio = 1.0 / 31.0;
        private const double ultraRatio = 1.0 / 12.0;
        private const double superRatio = 1.0 / 5.0;

        public CardProvider(ICardRepository cardSetRepo, IYGoProRepository ygoRepo, ILogWriter logger, IConfiguration configuration, ICardSetProvider setProv)
        {
            _cardRepo = cardSetRepo;
            _ygoRepo = ygoRepo;
            _logger = logger;
            _configuration = configuration;
            _setProv = setProv;
        }

        public async Task AddToCollection(IEnumerable<Card> newCards)
        {
            _logger.ServerLogInfo("Adding {0} cards to collection", newCards.Count());
            await _cardRepo.AddToCollectionAsync(newCards);
        }

        public void DeleteCollection()
        {
            _logger.ServerLogInfo("Deleting collection");
            _cardRepo.DeleteCollection();
        }

        public async Task<IEnumerable<Card>> GetAllCardsInSet(string setName)
        {
            if (string.IsNullOrEmpty(setName))
                throw new HttpResponseException(400, "setName cannot be empty.");

            var results = await _cardRepo.GetAllCardsInSetAsync(setName);

            if (results == null || results.Count() == 0)
            {
                _logger.ServerLogInfo("Fetching set: {0} from YGoPro", setName);
                var path = _configuration["CardImagesPath"];
                var cardSetData = await _ygoRepo.GetAllCardsInSetAsync(setName);

                if (!string.IsNullOrEmpty(cardSetData))
                {
                    List<Task> tasks = new List<Task>();
                    var obj = JsonConvert.DeserializeObject<YGoCardInfoResponse<Card>>(cardSetData);
                    Directory.CreateDirectory(Path.Combine(path, "cards"));
                    Directory.CreateDirectory(Path.Combine(path, "cards_small"));
                    Directory.CreateDirectory(Path.Combine(path, "cards_cropped"));
                    foreach (var card in obj.data)
                    {
                        tasks.AddRange(_ygoRepo.DownloadImagesAsync(card.card_images, path));

                        foreach (var item in card.card_images)
                        {
                            item.image_url = string.Format("/cards/{0}.jpg", item.id);
                            item.image_url_small = string.Format("/cards_small/{0}.jpg", item.id);
                            item.image_url_cropped = string.Format("/cards_cropped/{0}.jpg", item.id);
                        }
                    }
                    var unwrappedData = JsonConvert.SerializeObject(obj.data);
                    tasks.Add(_cardRepo.WriteCardDataToSetFileAsync(unwrappedData, setName));
                    await Task.WhenAll(tasks);
                    return obj.data;
                }

                return new List<Card>();
            }
            else
            {
                _logger.ServerLogInfo("Returning cached set: {0} from local", setName);
                return results;
            }
        }

        public async Task<Card> GetCardById(int id)
        {
            var results = await _cardRepo.GetByIdAsync(id);

            if (results == null)
            {
                _logger.ServerLogInfo("Fetching card id: {0} from YGoPro", id);
                var cardData = await _ygoRepo.GetCardByIdAsync(id);

                if (!string.IsNullOrEmpty(cardData))
                {
                    return await WriteCardData(cardData);
                }

                return new Card();
            }
            else
            {
                _logger.ServerLogInfo("Returning cached card id: {0} from local", id);
                return results;
            }
        }

        public async Task<Card> GetCardByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new HttpResponseException(400, "name cannot be empty.");

            var results = await _cardRepo.GetByNameAsync(name);

            if (results == null)
            {
                _logger.ServerLogInfo("Fetching card name: {0} from YGoPro", name);
                var cardData = await _ygoRepo.GetCardByNameAsync(name);

                if (!string.IsNullOrEmpty(cardData))
                {
                    return await WriteCardData(cardData);
                }

                return new Card();
            }
            else
            {
                _logger.ServerLogInfo("Returning cached card name: {0} from local", name);
                return results;
            }
        }

        public async Task<CardCollection> GetCollection()
        {
            _logger.ServerLogInfo("Retrieving collection");
            return await _cardRepo.GetCollectionAsync();
        }

        public async Task<IEnumerable<Card>> OpenPack(string setName)
        {
            var random = new Random();
            var setCards = await _cardRepo.GetAllCardsInSetAsync(setName);
            var set = await _setProv.GetSetByName(setName);

            var rarityMap = new Dictionary<int, List<Card>>();
            rarityMap[0] = new List<Card>();
            rarityMap[1] = new List<Card>();
            rarityMap[2] = new List<Card>();
            rarityMap[3] = new List<Card>();
            rarityMap[4] = new List<Card>();

            foreach (var card in setCards)
            {
                rarityMap[card.getRarityValue(set.set_code)].Add(card);
            }

            List<Card> flipCards = new List<Card>();

            // fill common slots
            // not all sets have commons, outer loop checks for lowest rarity available
            for (var r = 0; r < 4; r++)
            {
                if (rarityMap[r].Count == 0) continue;

                for (var x = 0; x < 7; x++)
                {
                    var roll = (int)Math.Floor(random.NextDouble() * rarityMap[r].Count);
                    flipCards.Add(rarityMap[r].ElementAt(roll));
                }

                if (flipCards[0].id != -1) break;
            }

            // roll for rares
            var rarityRoll = random.NextDouble();
            if (rarityRoll <= secretRatio && rarityMap[4].Count > 0)
            {
                var roll2 = (int)Math.Floor(random.NextDouble() * rarityMap[4].Count);
                flipCards.Add(rarityMap[4][roll2]);
            }
            else if (rarityRoll <= ultraRatio && rarityMap[3].Count > 0)
            {
                var roll2 = (int)Math.Floor(random.NextDouble() * rarityMap[3].Count);
                flipCards.Add(rarityMap[3][roll2]);
            }
            else if (rarityRoll <= superRatio && rarityMap[2].Count > 0)
            {
                var roll2 = (int)Math.Floor(random.NextDouble() * rarityMap[2].Count);
                flipCards.Add(rarityMap[2][roll2]);
            }
            else
            {
                for (var r = 0; r < 4; r++)
                {
                    if (rarityMap[r].Count == 0) continue;

                    var roll2 = (int)Math.Floor(random.NextDouble() * rarityMap[r].Count);
                    flipCards.Add(rarityMap[r][roll2]);

                    if (flipCards[7].id != -1) break;
                }
            }

            if (rarityMap[1].Count > 0)
            {
                var roll3 = (int)Math.Floor(random.NextDouble() * rarityMap[1].Count);
                flipCards.Add(rarityMap[1][roll3]);
            }
            else if (rarityMap[2].Count > 0)
            {
                var roll3 = (int)Math.Floor(random.NextDouble() * rarityMap[2].Count);
                flipCards.Add(rarityMap[2][roll3]);
            }
            else if (rarityMap[3].Count > 0)
            {
                var roll3 = (int)Math.Floor(random.NextDouble() * rarityMap[3].Count);
                flipCards.Add(rarityMap[3][roll3]);
            }
            else if (rarityMap[4].Count > 0)
            {
                var roll3 = (int)Math.Floor(random.NextDouble() * rarityMap[4].Count);
                flipCards.Add(rarityMap[4][roll3]);
            }
            else if (rarityMap[0].Count > 0)
            {
                var roll3 = (int)Math.Floor(random.NextDouble() * rarityMap[0].Count);
                flipCards.Add(rarityMap[0][roll3]);
            }

            return flipCards;
        }

        private async Task<Card> WriteCardData(string cardData)
        {
            var obj = JsonConvert.DeserializeObject<YGoCardInfoResponse<Card>>(cardData);
            var card = obj.data.FirstOrDefault();
            var path = _configuration["CardImagesPath"];
            Directory.CreateDirectory(Path.Combine(path, "cards"));
            Directory.CreateDirectory(Path.Combine(path, "cards_small"));
            Directory.CreateDirectory(Path.Combine(path, "cards_cropped"));

            var taskList = _ygoRepo.DownloadImagesAsync(card.card_images, path).ToList();

            foreach (var item in card.card_images)
            {
                item.image_url = string.Format("/cards/{0}.jpg", item.id);
                item.image_url_small = string.Format("/cards_small/{0}.jpg", item.id);
                item.image_url_cropped = string.Format("/cards_cropped/{0}.jpg", item.id);
            }

            taskList.Add(_cardRepo.WriteCardDataToAllFileAsync(JsonConvert.SerializeObject(card)));

            await Task.WhenAll(taskList);

            return card;
        }
    }
}
