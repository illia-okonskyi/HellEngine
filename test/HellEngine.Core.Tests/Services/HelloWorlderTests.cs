using HellEngine.Core.Constants;
using HellEngine.Core.Services;
using Xunit;

namespace HellEngine.Core.Tests.Services
{
    public class HelloWorlderTests
    {
        [Fact]
        public void GetHelloString()
        {
            // Arrange
            var sut = new HelloWorlder();

            // Act
            var result = sut.GetHelloString();

            // Assert
            Assert.Equal(HelloWorld.HelloString, result);
        }
    }
}
