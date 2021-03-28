using Newtonsoft.Json;
using System.Collections.Generic;

namespace HellEngine.Core.Models.StateMachine
{
    [JsonObject]
    public class State
    {
        public string Key { get; set; }
        public string TextAssetKey { get; set; }
        public string ImageAssetKey { get; set; }

        public string OnEnterScriptAssetKey { get; set; }
        public string OnLeaveScriptAssetKey { get; set; }
        public string OnTransitionScriptAssetKey { get; set; }


        public List<Transition> Transitions { get; set; }
    }
}
