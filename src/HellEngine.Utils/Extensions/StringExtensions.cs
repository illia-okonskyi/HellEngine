using System.IO;

namespace HellEngine.Utils.Extensions
{
    public static class StringExtensions
    {
        public static char EngineDirectorySeparator = '/';

        public static string AddPath(this string path, string value)
        {
            var result = path?? string.Empty;
            if (!string.IsNullOrEmpty(result))
            {
                result += EngineDirectorySeparator;
            }

            return result + value;
        }

        public static string NormalizeDirectorySeparators(this string path)
        {
            return path.Replace(EngineDirectorySeparator, Path.DirectorySeparatorChar);
        }
    }
}
