using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Interface
{
    public interface ISessionService
    {
        void AddSession(string sessionId, string username);
    }
}
