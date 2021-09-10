using System.Collections.Generic;

namespace charlie.dto
{
    public class CardCollection
    {
        public List<Card> cards { get; set; }
        public Dictionary<int, int> cardCount { get; set; }

        public CardCollection()
        {
            cardCount = new Dictionary<int, int>();
            cards = new List<Card>();
        }
    }
}
