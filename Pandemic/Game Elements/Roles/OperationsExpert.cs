using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Cards;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Game_Elements.Roles
{
    public class OperationsExpert : Role
    {
        readonly static string Title = "Operations Expert";
        public bool UsedSpecialAbility = false;
        public string[] SpecialActionDescription = new string[]
        {

        };

        public OperationsExpert(City StartingCity, int PlayerID): base(PlayerID, Title, StartingCity)
        {
            SpecialActions = 1;
        }

        public override void PrintSpecialAbilities()
        {
            int i = TextManager.AvailableStandardActions;
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine($"{i + 1}: RESEARCH STATION MOVE - Once per turn, move from a reasearch station to any city by discarding any City card.");
            Console.WriteLine($"Build a research station in your current city, _without_ discarding a City card.");
        }

        public override void PlayFirstSpecialAbility(StateManager State)
        {
            CharterFlightFromResearchStation(State);
        }

        public override void BuildResearchStation(StateManager State)
        {
            if (!CurrentCity.ResearchStation)
            {
                CurrentCity.BuildResearchStation();
                RemainingActions--;
                State.BuildResearchStation();
            }
            else
            {
                throw new IllegalMoveException("There is already a research station in the city you're in.");
            }
        }

        void CharterFlightFromResearchStation(StateManager State)
        {
            int Choice = -1;
            if (!CurrentCity.ResearchStation)
            {
                throw new IllegalMoveException("There needs to be a research station in the city you're in, in order to perform that action.");
            }
            else if (NumberOfCityCardsInHand() == 0)
            {
                throw new IllegalMoveException("You need to be able to discard a City Card from your hand in order to perform that move.");
            } else if(NumberOfCityCardsInHand() == 1)
            {
                Choice = 0;
            }
            else
            {
                Choice = TextManager.ChooseItemFromList(State.Cities, "go to");
            }

            City NextCity = State.Cities[Choice];

            List<Card> EligibleCards = Hand.FindAll(Card => Card is CityCard);
            Choice = TextManager.ChooseItemFromList(EligibleCards, "discard");

            CurrentCity = NextCity;
            Discard(Choice);
            UsedSpecialAbility = true;
        }

        public override void Reset()
        {
            RemainingActions = MaxActions;
            UsedSpecialAbility = false;
        }
    }
}
