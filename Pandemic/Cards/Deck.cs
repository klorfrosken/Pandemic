using System;
using System.Collections;
using System.Collections.Generic;
using Pandemic.Exceptions;

namespace Pandemic.Cards
{
    public abstract class Deck : IEnumerable
    {
        private protected List<Card> _cards = new List<Card>();

        //Draw from the back of the list to ensure efficiency
        public Card Draw()
        {
            if (_cards.Count == 0)
            {
                throw new UnexpectedBehaviourException("There are no more cards to draw in the deck. That's unfortunate... (and unexpected so the game crashes. Sorry");
            } else
            {
                int lastIndex = _cards.Count - 1;
                Card DrawnCard = _cards[lastIndex];
                _cards.RemoveAt(lastIndex);
                return DrawnCard;
            }
        }

        public List<Card> Draw(int numberOfCards)
        {
            if (_cards.Count == 0)
            {
                throw new UnexpectedBehaviourException("There are no more cards to draw in the deck. That's unfortunate... (and unexpected so the game crashes. Sorry");
            }
            else
            {
                List<Card> drawnCards = new List<Card>();
                for(int i=0; i<numberOfCards; i++)
                {
                    drawnCards.Add(Draw());
                }

                return drawnCards;
            }
        }

        //Using Fisher-Yates/Knuth-shuffle to shuffle a deck of cards
        public void Shuffle()
        {
            Random rnd = new Random();

            for (int i = _cards.Count; i > 0; i--)
            {
                int randomNumber = rnd.Next(i);
                Card temp = _cards[randomNumber];

                _cards.RemoveAt(randomNumber);
                _cards.Add(temp);
            }
        }

        public Boolean Remove(Card card)
        {
            return _cards.Remove(card);
        }

        public void RemoveAt(int index)
        {
            //this should never happen, but just in case...
            if (index < 0 || index > _cards.Count)
            {
                throw new UnexpectedBehaviourException("The index was out of range for the deck. That's not good. RemoveAt() in Deck() failed"); 
            }
            else
            {
                _cards.RemoveAt(index);
            }
        }
        
        public int Count()
        {
            return _cards.Count;
        }

        //Implementation to make Deck indexable
        public virtual Card this[int index]
        {
            get
            {
                return _cards[index];
            }

            set
            {
                _cards.Insert(index, value);
            }
        }

        public Boolean Contains(Card card)
        {
            return _cards.Exists(Card => Card == card);
        }

        //Implementation of IEnumerable&&IEnumerator
        public IEnumerator GetEnumerator()
        {
            return new DeckEnumerator(this);
        }

        class DeckEnumerator : IEnumerator
        {
            Deck deck;
            private int _currentIndex = -1;

            public DeckEnumerator(Deck deck)
            {
                this.deck = deck;
            }

            public bool MoveNext()
            {
                _currentIndex++;

                return (_currentIndex < (deck._cards.Count));
            }

            public object Current
            {
                get
                {
                    try
                    {
                        return deck._cards[_currentIndex];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public void Reset()
            {
                _currentIndex = -1;
            }
        }
    }
}