using System;
using System.Collections.Generic;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class ResilientPopulation : EventCard
    {
        readonly static string eventName = "Resilient Population";
        readonly static string eventDescription = $"pick a card from the discard pile for the infection deck. Remove that card form the game.";

        public ResilientPopulation() : base (eventName, eventDescription) { }

        public override void Play(Role playerWithCard, StateManager state)
        {
            if (state.InfectionDiscard.Count() == 0)
            {
                throw new IllegalMoveException($"There are no cards in the discard pile for the infection deck for you to use {eventName} on");
            }
            else if (!playerWithCard.CardInHand(eventName))
            {
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {eventName} in their hand to play.");
            }
            else
            {
                TextManager.PrintEventDescription(this);
                int counter = 0;
                foreach (Card currentCard in state.InfectionDiscard)
                {
                    counter++;
                    Console.WriteLine($"{counter}: {currentCard.ToString()}");
                }

                int Choice = TextManager.GetValidInteger(1, state.InfectionDiscard.Count());
                InfectionCard cardToRemove = state.InfectionDiscard[Choice] as InfectionCard;   //hvorfor er ikke dette et infection-kort allerede??
                state.InfectionDiscard.Remove(cardToRemove);
            }
        }
    }
}
