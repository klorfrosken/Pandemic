using System;

namespace Pandemic.Exceptions
{
    public class UnexpectedBehaviourException : Exception
    {
        public UnexpectedBehaviourException() { }

        public UnexpectedBehaviourException(string message, Exception InnerException = null) : base(message, InnerException) { }
    }
}
