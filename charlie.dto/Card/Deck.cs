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
            if (x == null && y == null) return 0;
            if (x == null) return Int32.MinValue;
            if (y == null) return Int32.MaxValue;

            return x.deck_id.Value.ToString().CompareTo(y.deck_id.Value.ToString());
        }
    }
}
