using System;

namespace Pandemic.UnitTests.TestClasses
{
    class TestException : Exception
    { 
            public TestException() { }

            public TestException(string message, Exception InnerException = null) : base(message, InnerException) { }
    }
}