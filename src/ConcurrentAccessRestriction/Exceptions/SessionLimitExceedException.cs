using ConcurrentAccessRestriction.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Exceptions
{
    public class SessionLimitExceedException : Exception
    {
        private readonly Session session;

        public SessionLimitExceedException(Session session)
        {
            this.session = session;
        }

        public SessionLimitExceedException(Session session, string message): base(message)
        {
            this.session = session;
        }
    }
}
