using HellEngine.Core.Services.Sessions;
using System;
using Xunit;

namespace HellEngine.Core.Tests.Services.Sessions
{
    public class SessionIdProviderTests
    {
        [Fact]
        public void SetGetSessionId()
        {
            // Arrange
            var sut = new SessionIdProvider();

            var sessionId = Guid.NewGuid();

            // Act
            sut.SetSessionId(sessionId);
            var actual = sut.GetSessionId();

            // Assert
            Assert.Equal(sessionId, actual);
        }
    }
}
