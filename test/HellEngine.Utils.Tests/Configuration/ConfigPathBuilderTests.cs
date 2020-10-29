using HellEngine.Utils.Configuration;
using Xunit;

namespace HellEngine.Utils.Tests.Configuration
{
    public class ConfigPathBuilderTests
    {
        [Fact]
        public void Default_EmptyResult()
        {
            // Arrange
            var sut = new ConfigPathBuilder();

            // Act
            var result = sut.Build();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Default_BasePathPreserved()
        {
            // Arrange
            var basePath = "root";

            var sut = new ConfigPathBuilder(basePath);

            // Act
            var result = sut.Build();

            // Assert
            Assert.Equal(basePath, result);
        }

        [Fact]
        public void Default_EmptyBasePath_NoSeparator()
        {
            // Arrange
            var sut = new ConfigPathBuilder();

            // Act
            sut.Add("section1");
            var result = sut.Build();

            // Assert
            Assert.Equal("section1", result);
        }

        [Fact]
        public void Add_EmptyPath_NoAdd()
        {
            // Arrange
            var basePath = "root";

            var sut = new ConfigPathBuilder(basePath);

            // Act
            sut.Add(null);
            sut.Add(string.Empty);
            var result = sut.Build();

            // Assert
            Assert.Equal(basePath, result);
        }

        [Fact]
        public void Add_Concatenated()
        {
            // Arrange
            var basePath = "root";
            var path = "section";

            var sut = new ConfigPathBuilder(basePath);

            // Act
            sut.Add(path);
            var result = sut.Build();

            // Assert
            Assert.Equal($"{basePath}{ConfigPathBuilder.ConfigPathSeparator}{path}" , result);
        }
    }
}
