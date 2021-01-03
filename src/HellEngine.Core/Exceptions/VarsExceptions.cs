using System;
using System.Collections.Generic;
using System.Text;

namespace HellEngine.Core.Exceptions
{
    public class VarsException : Exception
    {
        public VarsException()
            : base()
        { }

        public VarsException(string message)
            : base(message)
        { }

        public VarsException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public class VarNotFoundException : VarsException
    {
        public VarNotFoundException(string key)
            : base($"Var with key {key} not found")
        { }
    }

    public class VarAlreadyExistsException : VarsException
    {
        public VarAlreadyExistsException(string key)
            : base($"Var with key {key} already exists")
        { }
    }

}
