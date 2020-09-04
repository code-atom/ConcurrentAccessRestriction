using ConcurrentAccessRestriction.Configuration.DependencyInjection.Options;
using ConcurrentAccessRestriction.Default;
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
        private readonly DefaultSessionStore sessionStore;
        private SessionService sessionService;

        public SessionServiceTest()
        {
            option = new Mock<IOptions<ConcurrentAccessRestrictionOptions>>();
            sessionStore = new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance);
            sessionService = new SessionService(NullLogger<SessionService>.Instance, sessionStore, option.Object);
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

    }
}
