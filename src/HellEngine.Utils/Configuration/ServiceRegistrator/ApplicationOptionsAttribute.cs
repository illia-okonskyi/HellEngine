using System;

namespace HellEngine.Utils.Configuration.ServiceRegistrator
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    public class ApplicationOptionsAttribute : Attribute
    {
        public string Path { get; }

        public ApplicationOptionsAttribute(string path)
        {
            Path = path;
        }
    }
}
