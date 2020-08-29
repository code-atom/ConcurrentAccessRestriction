using ConcurrentAccessRestriction.Default;
using Microsoft.AspNetCore.Http;
using System;
using Xunit;

namespace ConcurrentAccessRestriction.Tests
{
    public class DefautlSessionResolverTest
    {

        private DefaultSessionResolver sessionResolver = new DefaultSessionResolver();

        [Fact]
        public void Retrieve_UserSession_From_HttpContext()
        {
        }
    }
}
