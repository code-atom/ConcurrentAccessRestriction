using ConcurrentAccessRestriction.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentAccessRestriction.Storage.Stores
{
    /// <summary>
    /// Session store maintain session storage
    /// </summary>
    public abstract class SessionStore<T> where T : Session
    {
        /// <summary>
        /// Add session into storage
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public abstract Task CreateAsync(T session);

        /// <summary>
        /// Remove session from storage
        /// </summary>
        /// <param name="sessionIdentifier"></param>
        /// <returns></returns>
        public abstract Task RemoveAsync(T session);

        /// <summary>
        /// Retrieve list of sessions that are generated for user/device
        /// </summary>
        /// <param name="ISessionIdentifier"></param>
        /// <returns></returns>
        public abstract IEnumerable<T> GetSessions(ISessionIdentifier ISessionIdentifier);

        /// <summary>
        /// Retrieve session detail from Session Id.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public abstract T GetSession(string sessionId);
    }
}
