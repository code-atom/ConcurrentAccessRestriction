using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Storage
{
    public class Session
    {
        public string Id { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
    }
}
