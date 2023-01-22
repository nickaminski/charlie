using charlie.dal.interfaces;
using charlie.dto.Card;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.dal.json_repos
{
    public class CardRepository : ICardRepository
    {
        private string _path;

        public CardRepository(IConfiguration config)
        {
            _path = config["CardPath"];

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        public async Task<IEnumerable<Card>> GetAllCardsInSet(string setName)
        {
            var filePath = getFilePath(setName);
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            var jsonString = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<List<Card>>(jsonString);
        }

        public async Task WriteCardDataToSetFile(string data, string setName)
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            var filePath = getFilePath(setName);
            await File.WriteAllTextAsync(filePath, data);
        }

        public async Task<Card> GetById(int id)
        {
            var filePath = getAllCardsFilePath();
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            var jsonString = await File.ReadAllTextAsync(filePath);
            var list = JsonConvert.DeserializeObject<List<Card>>(jsonString);
            if (list == null)
                list = new List<Card>();

            return list.FirstOrDefault(x => x.id == id);
        }

        public async Task<Card> GetByName(string name)
        {
            var filePath = getAllCardsFilePath();
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            var jsonString = await File.ReadAllTextAsync(filePath);
            var list = JsonConvert.DeserializeObject<List<Card>>(jsonString);
            if (list == null)
                list = new List<Card>();

            return list.FirstOrDefault(x => x.name.Equals(name));
        }

        public async Task WriteCardDataToAllFile(string data)
        {
            var filePath = getAllCardsFilePath();
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            var jsonString = await File.ReadAllTextAsync(filePath);
            var list = JsonConvert.DeserializeObject<List<Card>>(jsonString);
            if (list == null)
                list = new List<Card>();
            var newCard = JsonConvert.DeserializeObject<Card>(data);

            if (list.Count == 0)
            {
                list.Add(newCard);
            }
            else
            {
                int index = -1;
                index = list.FindIndex(x => x.id > newCard.id);
                if (index == -1)
                {
                    list.Add(newCard);
                }
                else
                {
                    list.Insert(index, newCard);
                }
            }
            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(list));
        }


        public async Task<CardCollection> GetCollection()
        {
            var filePath = getCollectionPath();
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            var jsonString = await File.ReadAllTextAsync(filePath);

            var collection = JsonConvert.DeserializeObject<CardCollection>(jsonString);

            if (collection == null)
                collection = new CardCollection();

            return collection;
        }

        public async Task AddToCollection(IEnumerable<Card> newCards)
        {
            var filePath = getCollectionPath();
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            var jsonString = await File.ReadAllTextAsync(filePath);
            var collection = JsonConvert.DeserializeObject<CardCollection>(jsonString);
            if (collection == null)
            {
                collection = new CardCollection();
            }

            foreach (var card in newCards)
            {
                if (collection.cardCount.TryGetValue(card.id, out int count))
                {
                    if (count < 3)
                    {
                        collection.cardCount[card.id] = count + 1;
                    }
                }
                else
                {
                    collection.cardCount.Add(card.id, 1);
                }

                if (collection.cards.Count == 0)
                {
                    collection.cards.Add(card);
                }
                else
                {
                    int index = -1;

                    if (collection.cards.FindIndex(x => x.name.CompareTo(card.name) == 0) != -1)
                        continue;

                    index = collection.cards.FindIndex(x => x.name.CompareTo(card.name) > 0);
                    if (index == -1)
                    {
                        collection.cards.Add(card);
                    }
                    else
                    {
                        collection.cards.Insert(index, card);
                    }
                }
            }

            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(collection));
        }

        public void DeleteCollection()
        {
            var filePath = getCollectionPath();
            if (!File.Exists(filePath))
            {
                return;
            }

            File.Delete(filePath);
        }

        private string getCollectionPath(string user = "")
        {
            if (!string.IsNullOrEmpty(user))
                user += "_";
            return _path + user + "current_collection.json";
        }

        private string getFilePath(string setName)
        {
            return _path + setName.ToLower().Replace(" ", "_").Replace(":", "") + ".json";
        }

        private string getAllCardsFilePath()
        {
            return _path + "all_cards.json";
        }

    }
}
