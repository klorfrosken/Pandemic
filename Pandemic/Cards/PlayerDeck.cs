using System.Collections.Generic;
using Pandemic.Exceptions;

namespace Pandemic.Cards
{
    public class PlayerDeck : Deck
    {
        public PlayerDeck() { }

        public PlayerDeck(List<Card> cardsInDeck)
        {
            cardsInDeck.Reverse();
            _cards.AddRange(cardsInDeck);
        }

        public void AddCard(Card newCard)
        {
            if (newCard is PlayerCard)
            {
                _cards.Add(newCard);
            } else
            {
                throw new UnexpectedBehaviourException($"An invalid card, {newCard.Name}, was attempted added to the Player Deck");
            }
        }

        public void AddCards(List<Card> newCards)
        {
            if(newCards == null)
            {
                throw new UnexpectedBehaviourException("For some reason an empty list of cards was attempted added to the PlayerDeck. That's not supposed to happen!");
            } else
            {
                _cards.AddRange(newCards);
            }
        }

        public void CombineDecks(PlayerDeck newDeck)
        {
            _cards.AddRange(newDeck._cards);
        }

        public void Clear()
        {
            _cards.Clear();
        }
    }
}
