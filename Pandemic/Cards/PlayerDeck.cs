using System;
using System.Collections.Generic;
using Pandemic.Cards.EventCards;
using Pandemic.Exceptions;

namespace Pandemic.Cards
{
    public class PlayerDeck : Deck
    {
        public PlayerDeck() { }

        public PlayerDeck(List<Card> CardsInDeck)
        {
            _cards.AddRange(CardsInDeck);
        }

        public void AddCard(Card temp)
        {
            if (temp is PlayerCard)
            {
                _cards.Add(temp);
            } else
            {
                throw new ArgumentException($"An invalid card, {temp.Name}, was attempted added to the Player Deck");
            }
        }

        public void AddCards(List<Card> NewCards)
        {
            foreach (Card CurrentCard in NewCards)
            {
                AddCard(CurrentCard);
            }
        }

        public void CombineDecks(PlayerDeck NewDeck)
        {
            _cards.AddRange(NewDeck._cards);
        }

        public void Clear()
        {
            _cards.Clear();
        }
    }
}
