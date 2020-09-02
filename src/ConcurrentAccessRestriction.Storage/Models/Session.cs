﻿using ConcurrentAccessRestriction.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Storage
{
    public abstract class Session : ISessionIdentifier
    {
        public string Id { get; set; }

        public abstract string SessionIdentifier { get; }

        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? ExpirationTime { get; set; }


        public bool IsExpired
        {
            get
            {
                return !ExpirationTime.HasValue || DateTimeOffset.UtcNow > ExpirationTime;
            }
        }
    }
}
