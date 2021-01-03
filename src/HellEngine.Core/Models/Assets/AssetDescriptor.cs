using Newtonsoft.Json;

namespace HellEngine.Core.Models.Assets
{
    [JsonObject]
    public class AssetDescriptor
    {
        public string Key { get; set; }
        public AssetType AssetType { get; set; }
        public string AssetPath { get; set; }

        // Fields required for image assets
        public string MediaType { get; set; }
        public string SourceUrl { get; set; }
    }
}
