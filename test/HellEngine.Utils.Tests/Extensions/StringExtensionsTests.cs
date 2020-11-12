using HellEngine.Utils.Extensions;
using System.IO;
using Xunit;

namespace HellEngine.Utils.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void AddPath_NoPath_NoSeparator()
        {
            var root = "";
            var value = "path";

            Assert.Equal(value, root.AddPath(value));
        }

        [Fact]
        public void AddPath()
        {
            var root = "root";
            var value = "path";
            var sep = StringExtensions.EngineDirectorySeparator;

            Assert.Equal($"{root}{sep}{value}", root.AddPath(value));
        }

        [Fact]
        public void NormalizeDirectorySeparators()
        {
            var sep = StringExtensions.EngineDirectorySeparator;
            var path = $"root{sep}dir1{sep}dir2";
            var nativeSep = Path.DirectorySeparatorChar;
            var expected = $"root{nativeSep}dir1{nativeSep}dir2";

            Assert.Equal(expected, path.NormalizeDirectorySeparators());
        }
    }
}
