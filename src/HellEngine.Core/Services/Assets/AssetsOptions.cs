using HellEngine.Utils.Configuration.ServiceRegistrator;

namespace HellEngine.Core.Services.Assets
{
    [ApplicationOptions("Assets")]
    public class AssetsOptions
    {
        public static AssetsOptions Default => new AssetsOptions();

        public string AssetsDir { get; set; } = Constants.Defaults.AssetsDir;
        public string VarValueSpanClass { get; set; } = Constants.Defaults.VarValueSpanClass;
    }
}
