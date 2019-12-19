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

        public Airlift(StateManager state) : base (_eventName, _eventDescription, state) { }

        public override void Play(Role playerWithCard)
        {
            if (!playerWithCard.CardInHand(_eventName)){
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {_eventName} in their hand to play.");
            }
            else
            {
                int playerChoice = TextManager.ChooseItemFromList(_state.Roles, "move");
                Role playerToMove = _state.Roles[playerChoice];

                Dictionary<string, City> EligibleCities = new Dictionary<string, City>(_state.Cities);
                EligibleCities.Remove(playerToMove.CurrentCity.Name);
                int cityChoice = TextManager.ChooseItemFromList(EligibleCities.Values, $"move the {playerToMove} to");
                City nextCity = _state.GetCity(cityChoice);

                playerToMove.ChangeCity(nextCity);
            }
        }
    }
}