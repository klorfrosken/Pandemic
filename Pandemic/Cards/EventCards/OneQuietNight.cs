using System.Collections.Generic;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class OneQuietNight : EventCard
    {
        readonly static string eventName = "One Quiet Night";
        readonly static string eventDesctiption = $"Skip the infection stage of one turn.";

        public OneQuietNight() : base (eventName, eventDesctiption) { }

        public override void Play(Role playerWithCard, StateManager state)
        {
            if (!playerWithCard.CardInHand(eventName))
            {
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {eventName} in their hand to play.");
            } else
            {
                throw new OneQuietNightException($"Thanks to the {playerWithCard.RoleName} it was a calm and quiet night.");
            }
        }
    }
}
