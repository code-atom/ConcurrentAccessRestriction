using ConcurrentAccessRestriction.Storage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentAccessRestriction.Tests
{
    public static class MockData
    {
        private static Random random = new Random();

        public const string MockSessionId = "00-000-000";
        public const string MockUserSession = "test.user@mail.com";


        public static string RandomSessionId
        {
            get
            {
                return string.Format("{0, 2}-{1, 3}-{2,3}", random.Next(), random.Next(), random.Next());
            }
        }

        public static UserSession UserSession
        {
            get
            {
                return UserSession.Create(MockSessionId, MockUserSession);
            }
        }


        public static ClaimsIdentity CLaimIdentity
        {
            get
            {
                return new ClaimsIdentity("mock");
            }
        }


        public static RequestDelegate InnerHandler = (innerHttpContext) =>
        {
            return Task.CompletedTask;
        };

        public static DateTimeOffset ExpirationTime
        {
            get
            {
                return DateTimeOffset.MinValue;
            }
        }


        public static class Claims
        {
            public static Claim Name = new Claim("Name", "test");

            public static Claim SessionId = new Claim("SessionId", MockSessionId);
        }
    }
}
