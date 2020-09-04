using ConcurrentAccessRestriction.Default;
using ConcurrentAccessRestriction.Interface;
using ConcurrentAccessRestriction.Storage;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Security.Claims;
using Xunit;

namespace ConcurrentAccessRestriction.Tests
{
    public class DefautlSessionResolverTest
    {
        private DefaultSessionResolver sessionResolver;
        private Mock<ISessionService> mockSessionService;
        private Mock<HttpContext> mockHttpContext;

        public DefautlSessionResolverTest()
        {
            mockSessionService = new Mock<ISessionService>();
            mockHttpContext = new Mock<HttpContext>();
            sessionResolver = new DefaultSessionResolver(mockSessionService.Object);
        }

        [Fact]
        public void Retrieve_UserSession_From_HttpContext()
        {
            var claimIdentity = MockData.CLaimIdentity;
            claimIdentity.AddClaim(MockData.Claims.SessionId);
            mockSessionService.Setup(x => x.GetSession(It.IsAny<string>())).Returns(MockData.UserSession);
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(claimIdentity));

            var session = sessionResolver.CurrentSession(mockHttpContext.Object);

            mockSessionService.Verify(x => x.GetSession(MockData.MockSessionId), Times.Once);
            Assert.NotNull(session);
            Assert.Equal(session.SessionIdentifier, MockData.MockUserSession);
            Assert.Equal(session.Id, MockData.MockSessionId);
            Assert.Null(session.ExpirationTime);
        }

        [Fact]
        public void Retrieve_UserSession_With_No_SessionId_Failed()
        {
            var claimIdentity = MockData.CLaimIdentity;
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(claimIdentity));

            Assert.Throws<InvalidOperationException>(() => sessionResolver.CurrentSession(mockHttpContext.Object));
        }

        [Fact]
        public void Retrieve_Null_UserSession()
        {
            var claimIdentity = MockData.CLaimIdentity;
            claimIdentity.AddClaim(MockData.Claims.SessionId);
            mockSessionService.Setup(x => x.GetSession(It.IsAny<string>())).Returns((default(UserSession)));
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(claimIdentity));

            var session = sessionResolver.CurrentSession(mockHttpContext.Object);

            mockSessionService.Verify(x => x.GetSession(MockData.MockSessionId), Times.Once);
            Assert.Null(session);
        }
    }
}
