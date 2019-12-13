using System;
using System.Collections.Generic;
using System.Text;

namespace Pandemic.Exceptions
{
    public class IllegalMoveException : Exception
    {
        public IllegalMoveException() { }

        public IllegalMoveException(string message) : base(message) { }
    }
}
