using System;
using Pandemic.Managers;
using Pandemic.Game;

namespace Pandemic.Game_Elements.Roles
{
    public class QuarantineSpecialist : Role
    {
        readonly static string Title = "QuarantineSpecialist";
        public QuarantineSpecialist(City StartingCity, int PlayerID, StateManager state = null, ITextManager textManager = null) : base(PlayerID, Title, StartingCity, state, textManager){ }

        public override void PrintSpecialAbilities()
        {
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine("You automatically prevents outbreaks and the placement of disease cubes in the city you are in and all cities connected to that city.");
        }
    }
}
