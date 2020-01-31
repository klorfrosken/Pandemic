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

        public Airlift(StateManager state = null, ITextManager textManager = null) : base (_eventName, _eventDescription, state, textManager) { }

        public override void Play(Role playerWithCard)
        {
            if (!playerWithCard.CardInHand(_eventName)){
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {_eventName} in their hand to play.");
            }
            else
            {
                textManager.PrintEventDescription(this);

                int playerChoice = textManager.ChooseItemFromList(_state.Roles, "move");
                Role playerToMove = _state.Roles[playerChoice];

                List<City> eligibleCities = new List<City>(_state.Cities.Values);
                eligibleCities.Remove(playerToMove.CurrentCity);
                int cityChoice = textManager.ChooseItemFromList(eligibleCities, $"move the {playerToMove} to");
                City nextCity = eligibleCities[cityChoice];

                playerToMove.ChangeCity(nextCity);
                playerWithCard.Hand.Remove(this);
            }
        }
    }
}