using HellEngine.Core.Constants;
using HellEngine.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            public string HelloString { get; }
            #endregion

            #region Services
            public IOptions<HelloWorlderOptions> Options { get; }
            public ILogger<HelloWorlder> Logger { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                HelloString = "helloString";

                Options = Mock.Of<IOptions<HelloWorlderOptions>>();
                Logger = Mock.Of<ILogger<HelloWorlder>>();

                Mock.Get(Options).Setup(
                    m => m.Value)
                    .Returns(new HelloWorlderOptions { HelloString = HelloString });
            }
        }
        #endregion

        [Fact]
        public void GetHelloString_NoConfig_Default()
        {
            // Arrange
            var context = new TestCaseContext();
            Mock.Get(context.Options).Setup(
                m => m.Value)
                .Returns<HelloWorlderOptions>(null);

            var sut = new HelloWorlder(context.Options, context.Logger);

            // Act
            var result = sut.GetHelloString();

            // Assert
            Assert.Equal(HelloWorld.HelloString, result);
        }

        [Fact]
        public void GetHelloString_ReturnsConfigured()
        {
            // Arrange
            var context = new TestCaseContext();
            var sut = new HelloWorlder(context.Options, context.Logger);

            // Act
            var result = sut.GetHelloString();

            // Assert
            Assert.Equal(context.HelloString, result);
        }
    }
}
