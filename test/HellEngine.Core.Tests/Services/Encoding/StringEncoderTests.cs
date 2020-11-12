using HellEngine.Core.Services.Encoding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace HellEngine.Core.Tests.Services.Encoding
{
    public class StringEncoderTests
    {
        #region Context
        class TestCaseContext
        {
            #region Data
            #endregion

            #region Services
            public IOptions<StringEncoderOptions> Options { get; }
            public ILogger<StringEncoder> Logger { get; }
            #endregion

            #region Utils
            #endregion

            public TestCaseContext()
            {
                Options = Mock.Of<IOptions<StringEncoderOptions>>();
                Logger = Mock.Of<ILogger<StringEncoder>>();

                Mock.Get(Options).Setup(
                    m => m.Value)
                    .Returns(new StringEncoderOptions { EncodingName = "UTF-8" });
            }
        }
        #endregion

        public static IEnumerable<object[]> EncodeFromString_Data()
        {
            yield return new object[]
            {
                "fo©of", new byte [] { 0x66, 0x6F, 0xC2, 0xA9, 0x6F, 0x66 }
            };
            yield return new object[]
            {
                "ba𝌆r", new byte [] { 0x62, 0x61, 0xF0, 0x9D, 0x8C, 0x86, 0x72 }
            };
        }

        [Theory]
        [MemberData(nameof(EncodeFromString_Data))]
        public void EncodeFromString(string str, byte[] expected)
        {
            // Arrange
            var context = new TestCaseContext();

            var sut = new StringEncoder(
                context.Options,
                context.Logger);

            // Act
            var result = sut.EncodeFromString(str);

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> EncodeToString_Data()
        {
            yield return new object[]
            {
                new byte [] { 0x66, 0x6F, 0xC2, 0xA9, 0x6F, 0x66 }, "fo©of"
            };
            yield return new object[]
            {
                new byte [] { 0x62, 0x61, 0xF0, 0x9D, 0x8C, 0x86, 0x72 }, "ba𝌆r"
            };
        }

        [Theory]
        [MemberData(nameof(EncodeToString_Data))]
        public void EncodeToString(byte[] data, string expected)
        {
            // Arrange
            var context = new TestCaseContext();

            var sut = new StringEncoder(
                context.Options,
                context.Logger);

            // Act
            var result = sut.EncodeToString(data);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
