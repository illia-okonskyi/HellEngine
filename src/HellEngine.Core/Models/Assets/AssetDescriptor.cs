using Newtonsoft.Json;

namespace HellEngine.Core.Models.Assets
{
    [JsonObject]
    public class AssetDescriptor
    {
        public string Key { get; set; }
        public AssetType AssetType { get; set; }
        public string AssetPath { get; set; }
    }
}
