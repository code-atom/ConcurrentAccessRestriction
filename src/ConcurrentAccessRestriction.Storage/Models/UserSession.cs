using ConcurrentAccessRestriction.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Storage
{
    public class UserSession : Session, ISessionIdentifier
    {
        public string Username { get; set; }

        public string SessionIdentifier
        {
            get
            {
                return Username;

            }
        }

        public UserSession(string sessionid, string username) 
        {
            Username = username;
            Id = sessionid;
        }
    }
}
