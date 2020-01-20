using System.Collections.Generic;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class GovernmentGrant : EventCard
    {
        readonly static string _eventName = "Government Grant";
        readonly static string _eventDescription = $"Build a research station in any city";

        public GovernmentGrant(StateManager state = null, TextManager textManager = null) : base(_eventName, _eventDescription, state, textManager) { }

        public override void Play(Role playerWithCard)
        {
            if (!playerWithCard.CardInHand(_eventName))
            {
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {_eventName} in their hand to play.");
            } else if (_state.RemainingResearchStations == 0)
            {
                throw new IllegalMoveException($"There are no research stations left to build. You'll have to make do with the ones you have.");
            } else
            {
                textManager.PrintEventDescription(this);

                List<City> eligibleCities = new List<City>();
                foreach (City currentCity in _state.Cities.Values)
                {
                    if (!currentCity.HasResearchStation)
                    {
                        eligibleCities.Add(currentCity);
                    }
                }

                int choice = textManager.ChooseItemFromList(eligibleCities, "build a research station in");
                City chosenCity = eligibleCities[choice];
                chosenCity.BuildResearchStation();
            }
        }
    }
}
