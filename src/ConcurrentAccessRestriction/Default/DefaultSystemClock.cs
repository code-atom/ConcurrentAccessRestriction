using ConcurrentAccessRestriction.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Default
{
    internal class DefaultSystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow
        {
            get
            {
                return DateTimeOffset.UtcNow;
            }
        }
    }
}
