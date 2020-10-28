using HellEngine.Core.Constants;
using HellEngine.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HellEngine.Core.Tests.Services
{
    public class HelloWorlderTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            #endregion

            #region Services
            public ILogger<HelloWorlder> Logger { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Logger = Mock.Of<ILogger<HelloWorlder>>();
            }
        }
        #endregion

        [Fact]
        public void GetHelloString()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new HelloWorlder(context.Logger);

            // Act
            var result = sut.GetHelloString();

            // Assert
            Assert.Equal(HelloWorld.HelloString, result);
        }
    }
}
