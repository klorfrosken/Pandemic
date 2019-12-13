using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Cards;
using Pandemic.Managers;
using Pandemic.Game;

namespace Pandemic.Game_Elements.Roles
{
    public class QuarantineSpecialist : Role
    {
        readonly static string Title = "QuarantineSpecialist";
        public QuarantineSpecialist(City StartingCity, int PlayerID) : base(PlayerID, Title, StartingCity){ }

        public override void PrintSpecialAbilities()
        {
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine("You automatically prevents outbreaks and the placement of disease cubes in the city you are in and all cities connected to that city.");
        }
    }
}
