using System;

namespace Pandemic.Exceptions
{
    public class OneQuietNightException : Exception
    {
        public OneQuietNightException() { }

        public OneQuietNightException(string message) : base(message) { }
    }
}
