﻿using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class OneQuietNight : EventCard
    {
        readonly static string _eventName = "One Quiet Night";
        readonly static string _eventDesctiption = $"Skip the infection stage of one turn.";

        public OneQuietNight(StateManager state = null , ITextManager textManager = null) : base (_eventName, _eventDesctiption, state, textManager) { }

        public override void Play(Role playerWithCard)
        {
            if (!playerWithCard.CardInHand(_eventName))
            {
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {_eventName} in their hand to play.");
            } else
            {
                textManager.PrintEventDescription(this);
                throw new OneQuietNightException($"Thanks to the {playerWithCard.RoleName} it was a calm and quiet night.");
            }
        }
    }
}
