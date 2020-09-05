using ConcurrentAccessRestriction.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Storage
{
    public abstract class Session : ISessionIdentifier
    {
        private DateTimeOffset? expirationTime = null;

        public string Id { get; protected set; }

        public abstract string SessionIdentifier { get; }

        public DateTimeOffset CreatedTime { get; private set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? ExpirationTime
        {
            get
            {
                return expirationTime;
            }
            private set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("ExpirationTime not assigned to null mannaully");
                }
                expirationTime = value;
            }
        }


        public bool IsExpired
        {
            get
            {
                return !ExpirationTime.HasValue || DateTimeOffset.UtcNow > ExpirationTime;
            }
        }

        public void ExpireSession()
        {
            expirationTime = null;
        }

        public void SetExpirationTime(DateTimeOffset date)
        {
            ExpirationTime = date;
        }  

        public void ExtendSession(TimeSpan extendTimespan)
        {
            if(ExpirationTime == null)
            {
                throw new ArgumentException("Expiration time is null, Use SetExpirationTime method to set expiration time");
            }

            ExpirationTime = ExpirationTime + extendTimespan;
        }
    }
}
