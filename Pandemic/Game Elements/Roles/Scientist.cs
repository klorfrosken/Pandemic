using System;
using Pandemic.Game;
using Pandemic.Managers;

namespace Pandemic.Game_Elements.Roles 
{
    public class Scientist : Role
    {
        readonly static string Title = "Scientist";
        public Scientist(City StartingCity, int PlayerID, StateManager state = null, ITextManager textManager = null) : base(PlayerID, Title, StartingCity, state, textManager)
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
