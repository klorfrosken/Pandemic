using System.Collections.Generic;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class Airlift : EventCard
    {
        readonly static string _eventName = "Airlift";
        readonly static string _eventDescription = $"Move any player to any city";

        public Airlift() : base (_eventName, _eventDescription) { }

        public override void Play(Role playerWithCard, StateManager state)
        {
            if (!playerWithCard.CardInHand(_eventName)){
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {_eventName} in their hand to play.");
            }
            else
            {
                int playerChoice = TextManager.ChooseItemFromList(state.Roles, "move");
                Role playerToMove = state.Roles[playerChoice];

                List<City> EligibleCities = new List<City>(state.Cities);
                EligibleCities.Remove(playerToMove.CurrentCity);
                int cityChoice = TextManager.ChooseItemFromList(EligibleCities, $"move the {playerToMove} to");
                City nextCity = state.Cities[cityChoice];

                playerToMove.ChangeCity(nextCity, state);
            }

        }
    }
}