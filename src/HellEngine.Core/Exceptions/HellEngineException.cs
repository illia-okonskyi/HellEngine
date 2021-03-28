using System;

namespace HellEngine.Core.Exceptions
{
    public class HellEngineException : Exception
    {
        public HellEngineException()
            : base()
        { }

        public HellEngineException(string message)
            : base(message)
        { }

        public HellEngineException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
