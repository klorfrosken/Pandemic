using System.Collections.Generic;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class OneQuietNight : EventCard
    {
        readonly static string _eventName = "One Quiet Night";
        readonly static string _eventDesctiption = $"Skip the infection stage of one turn.";

        public OneQuietNight() : base (_eventName, _eventDesctiption) { }

        public override void Play(Role playerWithCard, StateManager state)
        {
            if (!playerWithCard.CardInHand(_eventName))
            {
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {_eventName} in their hand to play.");
            } else
            {
                throw new OneQuietNightException($"Thanks to the {playerWithCard.RoleName} it was a calm and quiet night.");
            }
        }
    }
}
