using ConcurrentAccessRestriction.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Storage
{
    public class UserSession : Session
    {
        public string Username { get; set; }

        public override string SessionIdentifier
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
