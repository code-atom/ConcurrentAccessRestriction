using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentAccessRestriction.Interface
{
    public interface ISessionService
    {
        Task AddSession(string sessionId, string username);

        IEnumerable<Session> GetSessions(ISessionIdentifier sessionIdentifier);

        Session GetSession(string sessionId);

        Task RemoveSession(string sessionId);

        void SetExpiration(Session session);
    }
}
