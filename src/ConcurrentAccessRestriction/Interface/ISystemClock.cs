using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Interface
{
    public interface ISystemClock
    {
        DateTimeOffset UtcNow { get; }
    }
}
