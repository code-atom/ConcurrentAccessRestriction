using ConcurrentAccessRestriction.Configuration.DependencyInjection.Options;
using ConcurrentAccessRestriction.Default;
using ConcurrentAccessRestriction.Interface;
using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Stores;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConcurrentAccessRestriction.Tests
{
    public class SessionServiceTest
    {
        private Mock<IOptions<ConcurrentAccessRestrictionOptions>> option;
        private Mock<ISystemClock> mockSystemClock;
        private readonly DefaultSessionStore sessionStore;
        private SessionService sessionService;

        public SessionServiceTest()
        {
            option = new Mock<IOptions<ConcurrentAccessRestrictionOptions>>();
            option.Setup(x => x.Value).Returns(new ConcurrentAccessRestrictionOptions());
            mockSystemClock = new Mock<ISystemClock>();
            mockSystemClock.SetupGet(x => x.UtcNow).Returns(DateTimeOffset.UtcNow);
            sessionStore = new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance);
            sessionService = new SessionService(NullLogger<SessionService>.Instance, sessionStore, option.Object, mockSystemClock.Object);
        }
        

        [Fact]
        public async Task Create_Session()
        {
            var sessionId = MockData.RandomSessionId;
            await sessionService.AddSession(sessionId, MockData.MockUserSession);

            var session = sessionService.GetSession(sessionId);

            Assert.NotNull(session);
            Assert.Equal(sessionId, session.Id);
            Assert.Equal(MockData.MockUserSession, session.SessionIdentifier);
            Assert.Null(session.ExpirationTime);
        }

        [Fact]
        public async Task Retrieve_Sessions()
        {
            await sessionService.AddSession(MockData.RandomSessionId, MockData.MockUserSession);
            await sessionService.AddSession(MockData.RandomSessionId, MockData.MockUserSession);
            await sessionService.AddSession(MockData.RandomSessionId, MockData.MockUserSession);

            var sessions = sessionService.GetSessions(MockData.UserSession);

            Assert.NotNull(sessions);
            Assert.Equal(3, sessions.Count());
        }

        [Fact]
        public async Task Retrieve_Session()
        {
            var sessionId = MockData.RandomSessionId;
            await sessionService.AddSession(sessionId, MockData.MockUserSession);

            var session = sessionService.GetSession(sessionId);

            Assert.NotNull(session);
            Assert.Equal(sessionId, session.Id);
        }


        [Fact]
        public async Task Delete_Session()
        {
            var sessionId = MockData.RandomSessionId;
            await sessionService.AddSession(sessionId, MockData.MockUserSession);

            var session = sessionService.GetSession(sessionId);

            Assert.NotNull(session);

            await sessionService.RemoveSession(sessionId);

            session = sessionService.GetSession(sessionId);
            Assert.Null(session);
        }


        [Fact]
        public async Task Delete_Non_Exist_Session()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => sessionService.RemoveSession(MockData.MockSessionId));
        }


        [Fact]
        public async Task Set_New_Session_Expiration()
        {
            var dateTime = DateTimeOffset.UtcNow;
            mockSystemClock.SetupGet(x => x.UtcNow).Returns(dateTime);

            var sessionId = MockData.RandomSessionId;
            await sessionService.AddSession(sessionId, MockData.MockUserSession);

            await sessionService.ExtendSessionExpiration(sessionId);

            var session = sessionService.GetSession(sessionId);
            var expectedExpirationTime = dateTime + option.Object.Value.SlideExpirationTime;
            Assert.NotNull(session);
            Assert.True(session.ExpirationTime.HasValue);
            Assert.False(session.IsExpired);
            Assert.Equal(expectedExpirationTime, session.ExpirationTime);
        }

        [Fact]
        public async Task Throw_Exception_On_Extend_Non_Exist_Session()
        {
           await Assert.ThrowsAsync<InvalidOperationException>(() => sessionService.ExtendSessionExpiration(MockData.RandomSessionId));
        }

        [Fact]
        public async Task Extend_Expiration_DateTime_In_Existing_Session_With_1_Minute_Logic()
        {
            var dateTime = DateTimeOffset.UtcNow;
            mockSystemClock.SetupGet(x => x.UtcNow).Returns(dateTime);
            option.Setup(x => x.Value).Returns(new ConcurrentAccessRestrictionOptions { SlideExpirationTime = TimeSpan.FromSeconds(30) });

            var sessionId = MockData.RandomSessionId;
            await sessionService.AddSession(sessionId, MockData.MockUserSession);

            await sessionService.ExtendSessionExpiration(sessionId);

            await sessionService.ExtendSessionExpiration(sessionId);

            var session = sessionService.GetSession(sessionId);
            var expectedExpirationTime = dateTime + TimeSpan.FromMinutes(1);
            Assert.NotNull(session);
            Assert.True(session.ExpirationTime.HasValue);
            Assert.False(session.IsExpired);
            Assert.Equal(expectedExpirationTime, session.ExpirationTime);
        }

        [Fact]

        public async Task Check_Session_Expiration_Non_Modified_If_Session_Expiration_Not_Less_Than_1_Minutes()
        {
            var dateTime = DateTimeOffset.UtcNow;
            mockSystemClock.SetupGet(x => x.UtcNow).Returns(dateTime);
            option.Setup(x => x.Value).Returns(new ConcurrentAccessRestrictionOptions { SlideExpirationTime = TimeSpan.FromMinutes(2) });

            var sessionId = MockData.RandomSessionId;
            await sessionService.AddSession(sessionId, MockData.MockUserSession);

            await sessionService.ExtendSessionExpiration(sessionId);

            var session = sessionService.GetSession(sessionId);
            var expectedExpirationTime = dateTime + TimeSpan.FromMinutes(2);
            Assert.NotNull(session);
            Assert.True(session.ExpirationTime.HasValue);
            Assert.False(session.IsExpired);
            Assert.Equal(expectedExpirationTime, session.ExpirationTime);
        }
    }
}
