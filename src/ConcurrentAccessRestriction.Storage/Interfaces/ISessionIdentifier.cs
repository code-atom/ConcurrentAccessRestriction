using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Storage.Interfaces
{
    public interface ISessionIdentifier
    {
        string SessionIdentifier { get; }
    }
}
