using System;

namespace HellEngine.Core.Exceptions
{
    public class HellScriptException : Exception
    {
        public HellScriptException(string message)
            : base(message)
        { }

        public HellScriptException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
