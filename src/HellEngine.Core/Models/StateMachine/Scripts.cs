namespace HellEngine.Core.Models.StateMachine
{
    public class OnStateEnterInput
    {
        public State State { get; set; }
    }

    public class OnStateLeaveInput
    {
        public State State { get; set; }
    }

    public class OnTransitionInput
    {
        public State State { get; set; }
        public Transition Transition { get; set; }
    }

    public class OnTransitionOutput
    {
        public string NextStateKeyOverride { get; set; }
    }
}
