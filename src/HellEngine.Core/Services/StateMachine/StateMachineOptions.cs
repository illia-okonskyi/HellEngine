using HellEngine.Core.Constants;
using HellEngine.Utils.Configuration.ServiceRegistrator;

namespace HellEngine.Core.Services.StateMachine
{
    [ApplicationOptions("StateMachine")]
    public class StateMachineOptions
    {
        public static StateMachineOptions Default => new StateMachineOptions();

        public string InitialStateKey { get; set; } = Defaults.StateMachineInititalStateKey;
        public string FinalStateKey { get; set; } = Defaults.StateMachineFinalStateKey;
    }
}
