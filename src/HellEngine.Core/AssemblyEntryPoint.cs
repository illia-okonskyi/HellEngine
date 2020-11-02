using HellEngine.Utils.Configuration;
using System;

namespace HellEngine.Core
{
    public static class AssemblyEntryPoint
    {
        public static IServiceProvider SdkServiceProvider { get; private set; }

        public static ConfigPathBuilder MakeConfigPathBuilder()
        {
            return new ConfigPathBuilder(Constants.Config.DefaultPath);
        }

        public static Func<ConfigPathBuilder> ConfigPathBuilderFactoryMethod()
        {
            return new Func<ConfigPathBuilder>(MakeConfigPathBuilder);
        }

        public static void InitSdk(IServiceProvider sdkServiceProvider)
        {
            SdkServiceProvider = sdkServiceProvider;
        }
    }
}
