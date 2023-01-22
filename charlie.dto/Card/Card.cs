using System.Collections.Generic;
using System.Linq;

namespace charlie.dto.Card
{
    public class Card
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string desc { get; set; }
        public int atk { get; set; }
        public int def { get; set; }
        public int level { get; set; }
        public string race { get; set; }
        public string attribute { get; set; }
        public string archetype { get; set; }
        public List<CardSetCard> card_sets { get; set; }
        public List<CardImage> card_images { get; set; }
        public List<CardPrice> card_prices { get; set; }

        public int getRarityValue(string setCode) 
        {
            var cardSet = card_sets.Where(x => x.set_code.Split('-')[0] == setCode && isKnownRarity(x.set_rarity)).FirstOrDefault();

            if (cardSet != null) {
                switch(cardSet.set_rarity)
                {
                    case "Common" : return 0;
                    case "Rare" : return 1;
                    case "Super Rare" : return 2;
                    case "Ultra Rare" : return 3;
                    case "Secret Rare": return 4;
                    default: return 0;
                }
            }
            return 0;
        }

        public bool isKnownRarity(string set_rarity)
        {
            return set_rarity == "Common" ||
                   set_rarity == "Rare" ||
                   set_rarity == "Super Rare" ||
                   set_rarity == "Ultra Rare" ||
                   set_rarity == "Secret Rare";
        }
    }

}