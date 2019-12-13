using System.Collections.Generic;
using Pandemic.Managers;

namespace Pandemic.Cards
{
    public class InfectionDeck : Deck
    {
        public InfectionDeck() { }
        public InfectionDeck(List<InfectionCard> CardsInDeck)
        {
            _cards.AddRange(CardsInDeck);
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

        public void AddCards(List<InfectionCard> NewCards)
        {
            _cards.AddRange(NewCards);
        }

        public void CombineDecks(InfectionDeck NewDeck)
        {
            _cards.AddRange(NewDeck._cards);
        }

        public void InsertOnTop(InfectionDeck discardPile)
        {
            discardPile._cards.AddRange(_cards);
            _cards = discardPile._cards;
            discardPile.Clear();
        }

        public InfectionCard RemoveBottomCard()
        {
            int lastIndex = _cards.Count-1;
            InfectionCard tempCard = _cards[lastIndex] as InfectionCard;
            _cards.RemoveAt(lastIndex);

            return tempCard;
        }

        public void Clear()
        {
            _cards.Clear();
        }
    }
}
