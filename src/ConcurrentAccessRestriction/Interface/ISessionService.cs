using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Interface
{
    public interface ISessionService
    {
        void AddSession(string sessionId, string username);

        IEnumerable<Session> GetSessions(ISessionIdentifier sessionIdentifier);

        Session GetSession(string sessionId);

        void RemoveSession(string sessionId, string username);

        void SetExpiration(Session session);
    }
}
