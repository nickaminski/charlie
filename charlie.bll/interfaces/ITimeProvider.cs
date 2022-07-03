using System;

namespace charlie.bll.interfaces
{
    public interface ITimeProvider
    {
        long CurrentTimeStamp();
        DateTime CurrentDateTime();
    }
}
