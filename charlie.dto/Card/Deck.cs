using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace charlie.dto.Card
{
    public class Deck
    {
        public Guid? deck_id { get; set; }
        public string name { get; set; }
        public List<Card> cards { get; set; }

        public Dictionary<int, int> cardCount { get; set; }
    }

    public class DeckComparer : IComparer<Deck>
    {
        public int Compare([AllowNull] Deck x, [AllowNull] Deck y)
        {
            return x.deck_id.Value.ToString().CompareTo(y.deck_id.Value.ToString());
        }
    }
}
