using charlie.bll.interfaces;
using System;

namespace charlie.bll.providers
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime CurrentDateTime()
        {
            return DateTime.UtcNow;
        }

        public long CurrentTimeStamp()
        {
            return (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds + 0.5);
        }

    }
}
