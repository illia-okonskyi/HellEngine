using HellEngine.Core.Services.Encoding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using Xunit;


namespace HellEngine.Core.Tests.Services.Encoding
{
    public class Base64EncoderTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            #endregion

            #region Services
            public IOptions<Base64EncoderOptions> Options { get; }
            public ILogger<Base64Encoder> Logger { get; }
            public IStringEncoder StringEncoder { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Options = Mock.Of<IOptions<Base64EncoderOptions>>();
                Logger = Mock.Of<ILogger<Base64Encoder>>();
                StringEncoder = Mock.Of<IStringEncoder>();

                Mock.Get(Options).Setup(
                    m => m.Value)
                    .Returns(Base64EncoderOptions.Default);
            }
        }
        #endregion

        public static IEnumerable<object[]> Encode_Data()
        {
            yield return new object[]
            {
                new byte [] { 0x66, 0x6F, 0x6F, 0x62, 0x61, 0x72, 0x31 }, "Zm9vYmFyMQ=="
            };
            yield return new object[]
            {
                new byte [] { 0x71, 0x31, 0x2F, 0x5D }, "cTEvXQ=="
            };
        }

        [Theory]
        [MemberData(nameof(Encode_Data))]
        public void Encode(byte [] data, string expected)
        {
            // Arrange
            var context = new TestCaseContext();

            var sut = new Base64Encoder(
                context.Options,
                context.Logger,
                context.StringEncoder);

            // Act
            var result = sut.Encode(data);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Encode_StringOverload_FromStringEncoded()
        {
            // Arrange
            var context = new TestCaseContext();

            var sut = new Base64Encoder(
                context.Options,
                context.Logger,
                context.StringEncoder);

            var data = "123";

            // Act
            sut.Encode(data);

            // Assert
            Mock.Get(context.StringEncoder).Verify(
                m => m.EncodeFromString(data),
                Times.Once);
        }

        public static IEnumerable<object[]> Decode_Data()
        {
            yield return new object[]
            {
                "Zm9vYmFyMQ==", new byte [] { 0x66, 0x6F, 0x6F, 0x62, 0x61, 0x72, 0x31 }
            };
            yield return new object[]
            {
                "cTEvXQ==", new byte [] { 0x71, 0x31, 0x2F, 0x5D }
            };
        }

        [Theory]
        [MemberData(nameof(Decode_Data))]
        public void Decode(string data, byte[] expected)
        {
            // Arrange
            var context = new TestCaseContext();

            var sut = new Base64Encoder(
                context.Options,
                context.Logger,
                context.StringEncoder);

            // Act
            var result = sut.Decode(data);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DecodeAsString_ToStringEncoded()
        {
            // Arrange
            var context = new TestCaseContext();

            var sut = new Base64Encoder(
                context.Options,
                context.Logger,
                context.StringEncoder);

            var data = "MTIz";

            // Act
            sut.DecodeAsString(data);

            // Assert
            Mock.Get(context.StringEncoder).Verify(
                m => m.EncodeToString(It.IsAny<byte[]>()),
                Times.Once);
        }
    }
}
