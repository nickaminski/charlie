using System.Collections.Generic;

namespace charlie.dto.Card
{
    public class DraftState
    {
        public string draft_status { get; set; }
        public int current_pack { get; set; }
        public string set_name { get; set; }
        public int num_packs { get; set; }
        public IEnumerable<Card> cards { get; set; }
    }
}
