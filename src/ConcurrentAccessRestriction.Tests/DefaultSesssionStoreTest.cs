using ConcurrentAccessRestriction.Default;
using ConcurrentAccessRestriction.Storage;
using ConcurrentAccessRestriction.Storage.Stores;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace ConcurrentAccessRestriction.Tests
{
    public class DefaultSesssionStoreTest
    {
      

        [Fact]
        public async Task Create_Session_Success()
        {
            var sessionStore = new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance);
            var session = UserSession.Create(MockData.MockSessionId, MockData.MockUserSession);

            await sessionStore.CreateAsync(session);

            var userSession = sessionStore.GetSession(MockData.MockSessionId);

            Assert.NotNull(userSession);

        }

        [Fact]
        public async Task Create_Duplicate_Session()
        {
            var sessionStore = new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance);
            var session = UserSession.Create(MockData.MockSessionId, MockData.MockUserSession);

            await sessionStore.CreateAsync(session);
            await sessionStore.CreateAsync(session);

            var userSessions = sessionStore.GetSessions(session);

            Assert.Single(userSessions);
        }

        [Fact]
        public async Task Create_Session_Exception_If_Session_Null()
        {
            var sessionStore = new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sessionStore.CreateAsync(null));
        }

        [Fact]
        public async Task Retrieve_List_Of_Session()
        {
            var sessionStore = new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance);
            await sessionStore.CreateAsync(UserSession.Create(MockData.RandomSessionId, MockData.MockUserSession));
            await sessionStore.CreateAsync(UserSession.Create(MockData.RandomSessionId, MockData.MockUserSession));
            await sessionStore.CreateAsync(UserSession.Create(MockData.RandomSessionId, MockData.MockUserSession));

            var sessions = sessionStore.GetSessions(MockData.UserSession);

            Assert.NotNull(sessions);
            Assert.Equal(3, sessions.Count());
            Assert.Equal(MockData.MockUserSession, sessions.First().SessionIdentifier);

        }

        [Fact]
        public async Task Retrieve_User_Session()
        {
            var sessionStore = new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance);
            await sessionStore.CreateAsync(MockData.UserSession);

            var session = sessionStore.GetSession(MockData.MockSessionId);

            Assert.NotNull(session);
            Assert.Equal(MockData.MockSessionId, session.Id);
        }

        [Fact]
        public void Retrieve_Empty_Session()
        {
            var sessionStore = new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance);
            Assert.Null(sessionStore.GetSession(MockData.RandomSessionId));
        } 

        [Fact]
        public async Task Remove_Session_Success()
        {
            var sessionStore = new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance);
            await sessionStore.CreateAsync(MockData.UserSession);

            await sessionStore.RemoveAsync(MockData.UserSession);

            var session = sessionStore.GetSession(MockData.MockSessionId);
            Assert.Null(session);
        }

        [Fact]
        public async Task Remove_Non_Exist_Session()
        {
            var sessionStore = new DefaultSessionStore(NullLogger<DefaultSessionStore>.Instance);
            await sessionStore.RemoveAsync(MockData.UserSession);

            var session = sessionStore.GetSession(MockData.MockSessionId);
            Assert.Null(session);
        }
    }
}
