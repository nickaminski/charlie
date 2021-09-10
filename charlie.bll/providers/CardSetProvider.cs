using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dal.json_repos;
using charlie.dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class CardSetProvider : ICardSetProvider
    {
        private ICardSetRepository _cardSetRepo;
        private IYGoProRepository _ygoRepo;
        private ILogWriter _logger;

        public CardSetProvider(ICardSetRepository cardSetRepo, IYGoProRepository ygoRepo, ILogWriter logger)
        {
            _cardSetRepo = cardSetRepo;
            _ygoRepo = ygoRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<CardSet>> GetSets(string maxYear)
        {
            var results = await _cardSetRepo.GetAll();

            if (results == null || results.Count() == 0)
            {
                _logger.ServerLogInfo(string.Format("Fetching all card sets from YGoPro"));
                var cardSetData = await _ygoRepo.GetAllCardSets();
                await _cardSetRepo.WriteCardSetData(cardSetData);

                if (!string.IsNullOrEmpty(cardSetData))
                    results = JsonConvert.DeserializeObject<List<CardSet>>(cardSetData);
                else
                    results = new List<CardSet>();
            }
            else
            {
                _logger.ServerLogInfo(string.Format("Returning cached all card sets from local"));
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
    }
}
