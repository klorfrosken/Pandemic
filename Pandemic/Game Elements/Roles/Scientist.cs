using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Cards;
using Pandemic.Exceptions;

namespace Pandemic.Game_Elements.Roles 
{
    public class Scientist : Role
    {
        readonly static string Title = "Scientist";
        public Scientist(City StartingCity, int PlayerID) : base(PlayerID, Title, StartingCity)
        {
            CardsNecessaryForCure = 4;
        }

        public override void PrintSpecialAbilities()
        {
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine("You only need 4, not 5, City cards of the same disease color to Discover a Cure for that disease.");
        }
    }
}
