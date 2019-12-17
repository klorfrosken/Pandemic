﻿using System.Collections.Generic;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class GovernmentGrant : EventCard
    {
        readonly static string _eventName = "Government Grant";
        readonly static string _eventDescription = $"Build a research station in any city";

        public GovernmentGrant() : base(_eventName, _eventDescription) { }

        public override void Play(Role playerWithCard, StateManager state)
        {
            if (!playerWithCard.CardInHand(_eventName))
            {
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {_eventName} in their hand to play.");
            } else if (state.RemainingResearchStations == 0)
            {
                throw new IllegalMoveException($"There are no research stations left to build. You'll have to make do with the ones you have.");
            } else
            {
                TextManager.PrintEventDescription(this);

                List<City> EligibleCities = new List<City>();
                foreach (City currentCity in state.Cities)
                {
                    if (!currentCity.ResearchStation)
                    {
                        EligibleCities.Add(currentCity);
                    }
                }

                int Choice = TextManager.ChooseItemFromList(EligibleCities, "build a research station in");
                City ChosenCity = EligibleCities[Choice];
                ChosenCity.BuildResearchStation();
            }
        }
    }
}
