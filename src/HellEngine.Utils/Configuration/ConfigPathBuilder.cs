using System.Text;

namespace HellEngine.Utils.Configuration
{
    public class ConfigPathBuilder
    {
        public static string ConfigPathSeparator = ":";

        private readonly StringBuilder sb;

        public ConfigPathBuilder(string basePath = null)
        {
            sb = new StringBuilder(basePath);
        }

        public ConfigPathBuilder Add(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return this;
            }

            if (sb.Length > 0)
            {
                sb.Append(ConfigPathSeparator);
            }
            sb.Append(path);
            return this;
        }

        public string Build()
        {
            return sb.ToString();
        }
    }
}
