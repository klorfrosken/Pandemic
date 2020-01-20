using System;
using System.Collections.Generic;
using Pandemic.Exceptions;

namespace Pandemic.Cards
{
    public class PlayerDeck : Deck<PlayerCard>
    {
        public PlayerDeck() { }

        public PlayerDeck(List<PlayerCard> cardsInDeck)
        {
            cardsInDeck.Reverse();
            _cards.AddRange(cardsInDeck);
        }

        public void AddCard(PlayerCard newCard)
        {
            if (newCard == null)
            {
                throw new UnexpectedBehaviourException("For some reason a null Card was attempted added to the PlayerDeck. That's not supposed to happen!");
            } else
            {
                _cards.Add(newCard);
            }
        }

        public void AddCards(List<PlayerCard> newCards)
        {
            if(newCards == null || newCards.Count == 0)
            {
                throw new UnexpectedBehaviourException("For some reason an empty list of cards was attempted added to the PlayerDeck. That's not supposed to happen!");
            }
            else 
            {
                _cards.AddRange(newCards);
            }
        }

        public void CombineDecks(PlayerDeck newDeck)
        {
            if(newDeck == null || newDeck._cards.Count == 0)
            {
                throw new UnexpectedBehaviourException("An attempt was made to combine one or more empty Decks. That makes no sense!");
            } else
            {
                List<PlayerCard> combinedCards = new List<PlayerCard>(newDeck._cards);
                combinedCards.AddRange(_cards);
                _cards = combinedCards;
            }
        }

        public void Clear()
        {
            _cards.Clear();
        }
    }
}
