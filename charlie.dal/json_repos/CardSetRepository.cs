using charlie.dal.interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using charlie.dto;

namespace charlie.dal.json_repos
{
    public class CardSetRepository : ICardSetRepository
    {
        private string _path;

        public CardSetRepository(IConfiguration config)
        {
            _path = config["CardSetsPath"];

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        public async Task<IEnumerable<CardSet>> GetAll()
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            var filePath = getFilePath();
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            var jsonString = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<List<CardSet>>(jsonString);
        }

        public async Task WriteCardSetData(string data)
        {
            var filePath = getFilePath();
            await File.WriteAllTextAsync(filePath, data);
        }

        private string getFilePath()
        {
            return _path + "card_sets.json";
        }
    }
}
