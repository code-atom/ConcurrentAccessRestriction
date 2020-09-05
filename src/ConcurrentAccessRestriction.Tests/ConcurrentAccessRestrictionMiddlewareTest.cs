using Castle.Core.Logging;
using ConcurrentAccessRestriction.Configuration.DependencyInjection.Options;
using ConcurrentAccessRestriction.Default;
using ConcurrentAccessRestriction.Exceptions;
using ConcurrentAccessRestriction.Hosting;
using ConcurrentAccessRestriction.Interface;
using ConcurrentAccessRestriction.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConcurrentAccessRestriction.Tests
{
    public class ConcurrentAccessRestrictionMiddlewareTest
    {
        private ConcurrentAccessRestrictionMiddleware middleware;
        private DefaultSessionResolver inMemorySessionResolver;
        private SessionService inMemorySessionService;
        private Mock<HttpContext> mockHttpContext;
        private Mock<ISystemClock> mockSystemClock;
        private Mock<IOptions<ConcurrentAccessRestrictionOptions>> option;

        public ConcurrentAccessRestrictionMiddlewareTest()
        {
            mockHttpContext = new Mock<HttpContext>();
            mockSystemClock = new Mock<ISystemClock>();
            option = new Mock<IOptions<ConcurrentAccessRestrictionOptions>>();
            option.SetupGet(x => x.Value).Returns(new ConcurrentAccessRestrictionOptions());
            mockSystemClock.SetupGet(x => x.UtcNow).Returns(DateTimeOffset.UtcNow);
            inMemorySessionService = new SessionService(NullLogger<SessionService>.Instance, new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance), option.Object, mockSystemClock.Object);

            inMemorySessionResolver = new DefaultSessionResolver(inMemorySessionService);

            middleware = new ConcurrentAccessRestrictionMiddleware(MockData.InnerHandler, NullLogger<ConcurrentAccessRestrictionMiddleware>.Instance, option.Object);
        }

        [Fact]
        public async Task Error_If_Session_Not_Found()
        {
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(MockData.CLaimIdentity));

            await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.Invoke(mockHttpContext.Object, inMemorySessionService, inMemorySessionResolver));
        }

        [Fact]
        public async Task Check_Session_Limit_Reached()
        {
            var claimIdentity = MockData.CLaimIdentity;
            claimIdentity.AddClaim(MockData.Claims.SessionId);
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(claimIdentity));
            option.SetupGet(x => x.Value).Returns(new ConcurrentAccessRestrictionOptions { NumberOfAllowedSessions = 1 });

            await inMemorySessionService.AddSession(MockData.RandomSessionId, MockData.MockUserSession);
            await inMemorySessionService.AddSession(MockData.MockSessionId, MockData.MockUserSession);

            await Assert.ThrowsAsync<SessionLimitExceedException>(() => middleware.Invoke(mockHttpContext.Object, inMemorySessionService, inMemorySessionResolver));
        }

        [Fact]
        public async Task Disabled_Session_Check()
        {
            var claimIdentity = MockData.CLaimIdentity;
            claimIdentity.AddClaim(MockData.Claims.SessionId);
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(claimIdentity));
            option.SetupGet(x => x.Value).Returns(new ConcurrentAccessRestrictionOptions { ConcurrentAccessEnabled = false });

            await inMemorySessionService.AddSession(MockData.MockSessionId, MockData.MockUserSession);

            await middleware.Invoke(mockHttpContext.Object, inMemorySessionService, inMemorySessionResolver);

            var session = inMemorySessionService.GetSession(MockData.MockSessionId);
            Assert.NotNull(session);
            Assert.Null(session.ExpirationTime);
        }

        [Fact]
        public async Task Disabled_Session_Check_If_User_Not_Authenticated()
        {
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity()));
            await inMemorySessionService.AddSession(MockData.MockSessionId, MockData.MockUserSession);

            await middleware.Invoke(mockHttpContext.Object, inMemorySessionService, inMemorySessionResolver);

            var session = inMemorySessionService.GetSession(MockData.MockSessionId);
            Assert.NotNull(session);
            Assert.Null(session.ExpirationTime);
        }

        [Fact]
        public async Task Check_Session_Expired()
        {
            var claimIdentity = MockData.CLaimIdentity;
            claimIdentity.AddClaim(MockData.Claims.SessionId);
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(claimIdentity));

            option.SetupGet(x => x.Value).Returns(new ConcurrentAccessRestrictionOptions { SlideExpirationTime = TimeSpan.Zero });

            await inMemorySessionService.AddSession(MockData.MockSessionId, MockData.MockUserSession);

            await inMemorySessionService.ExtendSessionExpiration(MockData.MockSessionId);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => middleware.Invoke(mockHttpContext.Object, inMemorySessionService, inMemorySessionResolver));

        }

        [Fact]
        public async Task Set_New_Session_Expiration()
        {
            var claimIdentity = MockData.CLaimIdentity;
            claimIdentity.AddClaim(MockData.Claims.SessionId);
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(claimIdentity));

            option.SetupGet(x => x.Value).Returns(new ConcurrentAccessRestrictionOptions());

            await inMemorySessionService.AddSession(MockData.MockSessionId, MockData.MockUserSession);

            await middleware.Invoke(mockHttpContext.Object, inMemorySessionService, inMemorySessionResolver);

            var session = inMemorySessionService.GetSession(MockData.MockSessionId);
            Assert.NotNull(session);
            Assert.NotNull(session.ExpirationTime);
            Assert.False(session.IsExpired);
        }

        [Fact]
        public async Task Check_Session_Extened_If_Session_Near_To_Expired()
        {
            var claimIdentity = MockData.CLaimIdentity;
            claimIdentity.AddClaim(MockData.Claims.SessionId);
            mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(claimIdentity));

            option.SetupGet(x => x.Value).Returns(new ConcurrentAccessRestrictionOptions { SlideExpirationTime = TimeSpan.FromSeconds(30) });

            await inMemorySessionService.AddSession(MockData.MockSessionId, MockData.MockUserSession);

            await inMemorySessionService.ExtendSessionExpiration(MockData.MockSessionId);

            await middleware.Invoke(mockHttpContext.Object, inMemorySessionService, inMemorySessionResolver);



        }

        [Fact]
        public async Task Check_If_Two_Concurrent_Session_Check_And_Expired_Latest()
        {
        }

    }
}
