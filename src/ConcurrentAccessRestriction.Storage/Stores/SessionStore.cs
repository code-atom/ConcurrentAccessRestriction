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
        public abstract Task RemoveAsync(ISessionIdentifier sessionIdentifier);


        public abstract IEnumerable<T> GetSessions(ISessionIdentifier ISessionIdentifier);
    }
}
