using charlie.dal.interfaces;
using charlie.dto.Card;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.dal.json_repos
{
    public class DeckRepository : IDeckRepository
    {

        private string _path;

        public DeckRepository(IConfiguration config)
        {
            _path = config["DeckPath"];

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        public async Task<Deck> GetDeckById(Guid id)
        {
            var list = (await GetDecks()).ToList();

            var index = list.BinarySearch(new Deck() { deck_id = id }, new DeckComparer());
            if (index >= 0)
                return list.ElementAt(index);
            return null;
        }

        public async Task<Deck> SaveDeck(Deck deck)
        {
            var list = (await GetDecks()).ToList();

            deck.deck_id ??= Guid.NewGuid();

            if (list.Count == 0)
            {
                list.Add(deck);
            }
            else
            {
                int index = -1;
                index = list.FindIndex(x => x.deck_id.Value.ToString().CompareTo(deck.deck_id.Value.ToString()) > -1);
                if (index == -1)
                {
                    list.Add(deck);
                }
                else
                {
                    var replaceIndex = list.FindIndex(x => x.deck_id.Value.CompareTo(deck.deck_id.Value) == 0);
                    if (replaceIndex != -1)
                    {
                        list.RemoveAt(replaceIndex);
                        list.Insert(replaceIndex, deck);
                    }
                    else
                    {
                        list.Insert(index, deck);
                    }
                }
            }
            await File.WriteAllTextAsync(getDecksFilePath(), JsonConvert.SerializeObject(list));
            return deck;
        }

        public async Task<bool> DeleteDeck(Guid id)
        {
            var list = (await GetDecks()).ToList();

            int index = -1;
            index = list.FindIndex(x => x.deck_id.Value.ToString().CompareTo(id.ToString()) == 0);
            if (index != -1)
            {
                list.RemoveAt(index);
            }
            else
            {
                return false;
            }

            await File.WriteAllTextAsync(getDecksFilePath(), JsonConvert.SerializeObject(list));
            return true;
        }

        public async Task<IEnumerable<Deck>> GetDecks()
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            var filePath = getDecksFilePath();
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            var jsonString = await File.ReadAllTextAsync(filePath);
            var list = JsonConvert.DeserializeObject<List<Deck>>(jsonString);
            if (list == null)
                list = new List<Deck>();

            return list;
        }

        private string getDecksFilePath()
        {
            return _path + "decks.json";
        }
    }
}
