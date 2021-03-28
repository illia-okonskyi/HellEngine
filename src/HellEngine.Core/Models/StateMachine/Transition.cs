using Newtonsoft.Json;

namespace HellEngine.Core.Models.StateMachine
{
    [JsonObject]
    public class Transition
    {
        public string Key { get; set; }
        public string TextAssetKey { get; set; }

        public string NextStateKey { get; set; }

        public bool IsEnabled { get; set; } = true;
        public bool IsVisible { get; set; } = true;
    }
}
