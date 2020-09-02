using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Configuration.DependencyInjection.Options
{
    public class ConcurrentAccessRestrictionOptions
    {
        /// <summary>
        /// Determine whether concurrent access enabled or not
        /// </summary>
        public bool ConcurrentAccessEnabled { get; set; } = true;

        /// <summary>
        /// Number of concurrent session allowed
        /// </summary>
        public int NumberOfAllowedSessions { get; set; } = 5;

        /// <summary>
        /// Session expiration 
        /// </summary>
        public TimeSpan SlideExpirationTime { get; set; } = TimeSpan.FromMinutes(15);
    }
}
