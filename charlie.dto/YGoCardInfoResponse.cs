using System.Collections.Generic;

namespace charlie.dto
{
    public class YGoCardInfoResponse<T>
    {
        public List<T> data { get; set; }
    }
}
