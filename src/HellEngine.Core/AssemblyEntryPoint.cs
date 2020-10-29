using HellEngine.Utils.Configuration;
using System;

namespace HellEngine.Core
{
    public static class AssemblyEntryPoint
    {
        public static ConfigPathBuilder MakeConfigPathBuilder()
        {
            return new ConfigPathBuilder(Constants.Config.DefaultPath);
        }

        public static Func<ConfigPathBuilder> ConfigPathBuilderFactoryMethod()
        {
            return new Func<ConfigPathBuilder>(MakeConfigPathBuilder);
        }
    }
}
