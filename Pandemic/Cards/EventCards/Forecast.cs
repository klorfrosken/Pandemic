using System.Collections.Generic;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class Forecast : EventCard
    {
        readonly static string _eventName = "Forecast";
        readonly static string _eventDesctiption = $"Allows the player to rearrange the top 6 cards of the Infection Deck.";

        public Forecast(StateManager state = null, ITextManager textManager = null) : base (_eventName, _eventDesctiption, state, textManager) { }

        public override void Play(Role playerWithCard)
        {
            if (!playerWithCard.CardInHand(_eventName))
            {
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {_eventName} in their hand to play.");
            } else
            {
                textManager.PrintEventDescription(this);

                int numberOfCards = 6;

                if(_state.InfectionDeck.Count() < numberOfCards)
                {
                    numberOfCards = _state.InfectionDeck.Count();
                }
                List<InfectionCard> cardsToRearrange = _state.InfectionDeck.Draw(numberOfCards);
                List<InfectionCard> newOrder = new List<InfectionCard>();
                int choice;
                for (int i=1; i<numberOfCards; i++)
                {
                    choice = textManager.ChooseItemFromList(cardsToRearrange, $"play as card number {i}");
                    newOrder.Add(cardsToRearrange[choice]);
                    cardsToRearrange.RemoveAt(choice);
                }
                newOrder.Add(cardsToRearrange[0]);

                _state.InfectionDeck.AddCards(newOrder);
                playerWithCard.Hand.Remove(this);
            }
        }
    }
}
