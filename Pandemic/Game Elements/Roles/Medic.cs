using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Game_Elements.Roles
{
    public class Medic : Role
    {
        readonly static string Title = "Medic";
        public Medic(City StartingCity, int PlayerID) : base (PlayerID, Title, StartingCity) { }

        public override void PrintSpecialAbilities()
        {
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine("All cubes, not 1, of the same color are removed when Treating a disease.");
            Console.WriteLine("If a disease has been cured, all cubes of that color are automatically removed from a city when the Medic enters or is located in that city.");
            Console.WriteLine("Automatic removal of cubes can occur on other players' turns. It does not cost an action, regardless of who's turn it is.");
        }
        public override void ChangeCity(City NextCity, StateManager State)
        {
            CurrentCity = NextCity;
            AutoCure(State);
        }

        public override void TreatDisease(Colors Color, StateManager State)
        {
            Boolean NotCleared = true;
            do
            {
                try
                {
                    CurrentCity.TreatDisease(Color, State);
                }
                catch (IllegalMoveException)
                {
                    NotCleared = false;
                }
                catch (UnexpectedBehaviourException) { throw; }
            } while (NotCleared);

            if (!State.Cures[CurrentCity.Color])
            {
                RemainingActions--;
            }
        }

        void AutoCure(StateManager State)
        {
            Boolean CureDiscovered;
            Boolean CubesToCureInCity;
            for (int i = 1; i < 5; i++)
            {
                CureDiscovered = State.Cures[(Colors)i];
                CubesToCureInCity = CurrentCity.DiseaseCubes[(Colors)i] > 0;
                if (CureDiscovered && CubesToCureInCity)
                {
                    try
                    {
                        TreatDisease((Colors)i, State);
                    }
                    catch { throw; }
                }
            }
        }
    }
}
