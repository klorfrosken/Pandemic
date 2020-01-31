using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class ResilientPopulation : EventCard
    {
        readonly static string _eventName = "Resilient Population";
        readonly static string _eventDescription = $"pick a card from the discard pile for the infection deck. Remove that card form the game.";

        public ResilientPopulation(StateManager state = null, ITextManager textManager = null) : base (_eventName, _eventDescription, state, textManager) { }

        public override void Play(Role playerWithCard)
        {
            if (!playerWithCard.CardInHand(_eventName))
            {
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {_eventName} in their hand to play.");
            }
            else if (_state.InfectionDiscard.Count() == 0)
            {
                throw new IllegalMoveException($"There are no cards in the discard pile for the infection deck for you to use {_eventName} on");
            }
            else
            {
                textManager.PrintEventDescription(this);

                int Choice = textManager.ChooseItemFromList(_state.InfectionDiscard, "remove");
                InfectionCard cardToRemove = _state.InfectionDiscard[Choice];
                _state.InfectionDiscard.Remove(cardToRemove);
                
                playerWithCard.Hand.Remove(this);
            }
        }
    }
}
