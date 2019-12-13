using System;
using System.Collections.Generic;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class Forecast : EventCard
    {
        readonly static string eventName = "Forecast";
        readonly static string eventDesctiption = $"Allows the player to rearrange the top 6 cards of the Infection Deck.";

        public Forecast() : base (eventName, eventDesctiption) { }

        public override void Play(Role playerWithCard, StateManager state)
        {
            if (!playerWithCard.CardInHand(eventName))
            {
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {eventName} in their hand to play.");
            } else
            {
                TextManager.PrintEventDescription(this);

                List<Card> cardsToRearrange = state.InfectionDeck.Draw(6);
                List<InfectionCard> newOrder = new List<InfectionCard>();
                int choice = -1;
                for (int i=1; i<6; i++)
                {
                    choice = TextManager.ChooseItemFromList(cardsToRearrange, $"play as card number {i}");
                    newOrder.Add(cardsToRearrange[choice] as InfectionCard);
                    cardsToRearrange.RemoveAt(choice);
                }

                newOrder.Add(cardsToRearrange[0] as InfectionCard);
                InfectionDeck tempDeck = new InfectionDeck();
                tempDeck.AddCards(newOrder);

                state.InfectionDeck.InsertOnTop(tempDeck);
            }
        }
    }
}
