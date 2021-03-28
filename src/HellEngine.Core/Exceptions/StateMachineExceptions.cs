using System;

namespace HellEngine.Core.Exceptions
{
    public class StateMachineException : HellEngineException
    {
        public StateMachineException()
            : base()
        { }

        public StateMachineException(string message)
            : base(message)
        { }

        public StateMachineException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public class StateNotFoundException : StateMachineException
    {
        public StateNotFoundException(string key, Exception innerException)
            : base($"State {key} not found", innerException)
        { }
    }

    public class BadStateException : StateMachineException
    {
        public BadStateException(string key, Exception innerException)
            : base($"State {key} has bad syntax", innerException)
        { }
    }

    public class TransitionNotFoundException : StateMachineException
    {
        public TransitionNotFoundException(string key)
            : base($"Transition {key} not found or one more transition with same key is present")
        { }
    }
}
