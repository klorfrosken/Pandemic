using System.Collections.Generic;
using Pandemic.Managers;

namespace Pandemic.Cards
{
    public class InfectionDeck : Deck<InfectionCard>
    {
        public InfectionDeck() { }

        public InfectionDeck(List<InfectionCard> cardsInDeck)
        {
            cardsInDeck.Reverse();
            _cards.AddRange(cardsInDeck);
        }

        public void AddCard(InfectionCard newCard)
        {
            _cards.Add(newCard);
        }

        public void AddCards(List<InfectionCard> newCards)
        {
            newCards.Reverse();
            _cards.AddRange(newCards);
        }

        public void InsertOnTop(InfectionDeck discardPile)
        {
            _cards.AddRange(discardPile._cards);
            discardPile.Clear();
        }

        public InfectionCard RemoveBottomCard()
        {
            InfectionCard bottomCard = _cards[0];
            _cards.RemoveAt(0);

            return bottomCard;
        }

        public void Clear()
        {
            _cards.Clear();
        }

        public void Infect()
        {
            InfectionCard topCard = this.Draw();
            topCard.Infect();
        }
    }
}
