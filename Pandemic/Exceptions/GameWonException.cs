using System;
using System.Collections.Generic;
using System.Text;

namespace Pandemic.Exceptions
{
    public class GameWonException : Exception
    {
        public GameWonException() : base ("OMG! How did you do that? Congratulations! The world lives to die slowly of global warming instead") { }

        public GameWonException(string message) : base(message) { }
    }
}
