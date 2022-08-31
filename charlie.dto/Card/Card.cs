using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
    }

    public class CardNameComparer : IComparer<Card>
    {
        public int Compare([AllowNull] Card x, [AllowNull] Card y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return Int32.MinValue;
            if (y == null) return Int32.MaxValue;

            return x.name.CompareTo(y.name);
        }
    }

    public class CardIdComparer : IComparer<Card>
    {
        public int Compare([AllowNull] Card x, [AllowNull] Card y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return Int32.MinValue;
            if (y == null) return Int32.MaxValue;

            return x.id - y.id;
        }
    }
}