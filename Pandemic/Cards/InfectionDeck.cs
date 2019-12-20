using System.Collections.Generic;
using Pandemic.Managers;

namespace Pandemic.Cards
{
    public class InfectionDeck : Deck
    {
        public InfectionDeck() { }

        public InfectionDeck(List<InfectionCard> cardsInDeck)
        {
            _cards.AddRange(cardsInDeck);
        }

        public void Infect(StateManager state)
        {
            InfectionCard topCard = this.Draw() as InfectionCard;
            topCard.Infect(state);

        }

        public void AddCard(InfectionCard temp)
        {
            _cards.Add(temp);
        }

        public void AddCards(List<InfectionCard> newCards)
        {
            newCards.Reverse();
            _cards.AddRange(newCards);
        }

        public void CombineDecks(InfectionDeck newDeck)
        {
            _cards.AddRange(newDeck._cards);
        }

        public void InsertOnTop(InfectionDeck discardPile)
        {
            discardPile._cards.AddRange(_cards);
            _cards = discardPile._cards;
            discardPile.Clear();
        }

        public InfectionCard RemoveBottomCard()
        {
            InfectionCard bottomCard = _cards[0] as InfectionCard;
            _cards.RemoveAt(0);

            return bottomCard;
        }

        public void Clear()
        {
            _cards.Clear();
        }
    }
}
