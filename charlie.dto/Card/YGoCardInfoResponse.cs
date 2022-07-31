using System.Collections.Generic;

namespace charlie.dto.Card
{
    public class YGoCardInfoResponse<T>
    {
        public List<T> data { get; set; }
    }
}
