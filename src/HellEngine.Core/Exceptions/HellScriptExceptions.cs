using System;

namespace HellEngine.Core.Exceptions
{
    public class HellScriptException : Exception
    {
        public HellScriptException()
            : base()
        { }

        public HellScriptException(string message)
            : base(message)
        { }

        public HellScriptException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public class UnexpectedScriptTypeException : HellScriptException
    {
        public UnexpectedScriptTypeException()
            : base("Unexpected script context type")
        { }
    }

    public class RuntimeScriptException : HellScriptException
    {
        public RuntimeScriptException(Exception innerException)
            : base("Exception executing HellScript", innerException)
        { }
    }

    public class ServiceAccessDeniedException : HellScriptException
    {
        public ServiceAccessDeniedException(Type service)
            : base($"Access to service {service} denied")
        { }
    }
}
