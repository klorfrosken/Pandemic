using System;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Game_Elements.Roles
{
    public class Medic : Role
    {
        readonly static string Title = "Medic";
        public Medic(City StartingCity, int PlayerID, StateManager state = null, ITextManager textManager = null) : base (PlayerID, Title, StartingCity, state, textManager) { }

        public override void PrintSpecialAbilities()
        {
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine("All cubes, not 1, of the same color are removed when Treating a disease.");
            Console.WriteLine("If a disease has been cured, all cubes of that color are automatically removed from a city when the Medic enters or is located in that city.");
            Console.WriteLine("Automatic removal of cubes can occur on other players' turns. It does not cost an action, regardless of who's turn it is.");
        }
        public override void ChangeCity(City NextCity)
        {
            CurrentCity = NextCity;
            AutoCure();
        }

        public override void TreatDisease(Colors Color)
        {
            if (CurrentCity.DiseaseCubes[Color] == 0)
            {
                throw new IllegalMoveException($"There are no {Color} cubes in {CurrentCity} to remove.");
            } else
            {
                Boolean NotCleared = true;
                do
                {
                    try
                    {
                        CurrentCity.TreatDisease(Color);
                    }
                    catch (IllegalMoveException)
                    {
                        NotCleared = false;
                    }
                } while (NotCleared);

                if (!state.Cures[CurrentCity.Color])
                {
                    RemainingActions--;
                }
            }
        }

        void AutoCure()
        {
            Boolean CureDiscovered;
            Boolean CubesToCureInCity;
            for (int i = 1; i < 5; i++)
            {
                CureDiscovered = state.Cures[(Colors)i];
                CubesToCureInCity = CurrentCity.DiseaseCubes[(Colors)i] > 0;
                if (CureDiscovered && CubesToCureInCity)
                {
                    TreatDisease((Colors)i);
                }
            }
        }
    }
}
