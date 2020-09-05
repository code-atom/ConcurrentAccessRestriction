using ConcurrentAccessRestriction.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using ConcurrentAccessRestriction.Storage.Extensions;

namespace ConcurrentAccessRestriction.Storage
{
    public class UserSession : Session
    {
        public string Username { get; private set; }

        public override string SessionIdentifier
        {
            get
            {
                return Username;

            }
        }

        private UserSession(string sessionid, string username) 
        {
            Username = username;
            Id = sessionid;
        }

        //Info: Factory method helps us to validate the parameter before creating object
        public static UserSession Create(string sessionid, string username)
        {
            sessionid.ThrowArgumentException("");
            if (string.IsNullOrEmpty(sessionid) || string.IsNullOrWhiteSpace(sessionid))
            {
                throw new ArgumentNullException($"{nameof(sessionid)} is required to create a user session");
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException($"{nameof(sessionid)} is required to create a user session");
            }

            return new UserSession(sessionid, username);
        }
    }
}
