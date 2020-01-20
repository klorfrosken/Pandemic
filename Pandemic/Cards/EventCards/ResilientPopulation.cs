using System;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class ResilientPopulation : EventCard
    {
        readonly static string _eventName = "Resilient Population";
        readonly static string _eventDescription = $"pick a card from the discard pile for the infection deck. Remove that card form the game.";

        public ResilientPopulation(StateManager state = null, TextManager textManager = null) : base (_eventName, _eventDescription, state, textManager) { }

        public override void Play(Role playerWithCard)
        {
            if (_state.InfectionDiscard.Count() == 0)
            {
                throw new IllegalMoveException($"There are no cards in the discard pile for the infection deck for you to use {_eventName} on");
            }
            else if (!playerWithCard.CardInHand(_eventName))
            {
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {_eventName} in their hand to play.");
            }
            else
            {
                textManager.PrintEventDescription(this);
                int counter = 0;
                foreach (Card currentCard in _state.InfectionDiscard)
                {
                    counter++;
                    Console.WriteLine($"{counter}: {currentCard.ToString()}");
                }

                int Choice = textManager.GetValidInteger(1, _state.InfectionDiscard.Count());
                InfectionCard cardToRemove = _state.InfectionDiscard[Choice] as InfectionCard;   //hvorfor er ikke dette et infection-kort allerede??
                
                if (!_state.InfectionDiscard.Remove(cardToRemove))
                {
                    throw new UnexpectedBehaviourException("A card that doesn't exist was attempted removed from a Deck. That's not good...");
                }
            }
        }
    }
}
