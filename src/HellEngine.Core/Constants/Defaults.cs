namespace HellEngine.Core.Constants
{
    public static class Defaults
    {
        public static string ConfigPath = "HellEngine";
        public static string Locale = "default";
        public static char DirectorySeparator = Utils.Extensions.StringExtensions.EngineDirectorySeparator;

        public static string EncodingName = EncodingNames.UTF8;

        public static string AssetsDir = "assets";
        public static string AssetsDataDir = "data";
        public static string AssetsDescriptorsDir = "descriptors";

        public static string VarValueSpanClass = "var-value";

        public static string StateMachineInititalStateKey = "common.state.initial";
        public static string StateMachineFinalStateKey = "common.state.final";
        
        public static string VarsManagerUserNameVarKey = "common.userName";
        public static string VarsManagerUserNameVarNameAssetKey = "common.vars.userName";
    }
}
