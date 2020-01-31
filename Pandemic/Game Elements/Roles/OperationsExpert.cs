using System;
using System.Collections.Generic;
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

        public OperationsExpert(City StartingCity, int PlayerID, StateManager state = null, ITextManager textManager = null) : base(PlayerID, Title, StartingCity, state, textManager)
        {
            SpecialActions = 1;
        }

        public override void PrintSpecialAbilities()
        {
            int i = textManager.AvailableStandardActions;
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine($"{i + 1}: RESEARCH STATION MOVE - Once per turn, move from a reasearch station to any city by discarding any City card.");
            Console.WriteLine($"Build a research station in your current city, _without_ discarding a City card.");
        }

        public override void PlayFirstSpecialAbility()
        {
            CharterFlightFromResearchStation();
        }

        public override void BuildResearchStation()
        {
            if (!CurrentCity.HasResearchStation)
            {
                CurrentCity.BuildResearchStation();
                RemainingActions--;
            }
            else
            {
                throw new IllegalMoveException("There is already a research station in the city you're in.");
            }
        }

        void CharterFlightFromResearchStation()
        {
            if (UsedSpecialAbility)
            {
                throw new IllegalMoveException("You have already used your special ability this turn. You must wait for the next one, I'm afraid...");
            }
            if (!CurrentCity.HasResearchStation)
            {
                throw new IllegalMoveException("There needs to be a research station in the city you're in, in order to perform that action.");
            }
            else if (NumberOfCityCardsInHand() == 0)
            {
                throw new IllegalMoveException("You need to be able to discard a City Card from your hand in order to perform that move.");
            }
            else
            {
                int cardChoice = -1;
                if (NumberOfCityCardsInHand() == 1)
                {
                    cardChoice = 0;
                }
                else
                {
                    List<PlayerCard> EligibleCards = Hand.FindAll(Card => Card is CityCard);
                    cardChoice = textManager.ChooseItemFromList(EligibleCards, "discard");
                }

                List<City> availableCities = new List<City>(state.Cities.Values);
                availableCities.Remove(CurrentCity);
                int cityChoice = textManager.ChooseItemFromList(availableCities, "go to");

                City NextCity = availableCities[cityChoice];

                CurrentCity = NextCity;
                Discard(cardChoice);
                UsedSpecialAbility = true;
            }

        }

        public override void Reset()
        {
            RemainingActions = maxActions;
            UsedSpecialAbility = false;
        }
    }
}
