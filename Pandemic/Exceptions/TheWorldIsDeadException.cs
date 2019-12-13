using System;
using System.Collections.Generic;
using System.Text;

namespace Pandemic.Exceptions
{
    public class TheWorldIsDeadException : Exception
    {
        static string[] Messages = 
            { 
                "I'm sorry, but the world just died." ,
                "Apparantly your team is incompetend and the world is now paying for it. Everybody's sick",
                "There's nothing left. Everybody just died."
            };

        static Random rnd = new Random();
        static int i = rnd.Next(Messages.Length);

        public TheWorldIsDeadException(): base(Messages[i]) { }

        public TheWorldIsDeadException(string message) : base(message) { }
    }
}
