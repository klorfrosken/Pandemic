using System;
using System.Collections;
using System.Collections.Generic;
using Pandemic.Exceptions;

namespace Pandemic.Cards
{
    public abstract class Deck<T> : IEnumerable<T> where T : Card
    {
        private protected List<T> _cards = new List<T>();

        //Draw from the back of the list to ensure efficiency
        public T Draw()
        {
            if (_cards.Count == 0)
            {
                throw new UnexpectedBehaviourException("There are no more cards to draw in the deck. That's unfortunate... (and unexpected so the game crashes. Sorry");
            } else
            {
                int lastIndex = _cards.Count - 1;
                T DrawnCard = _cards[lastIndex];
                _cards.RemoveAt(lastIndex);
                return DrawnCard;
            }
        }

        public List<T> Draw(int numberOfCards)
        {
            if (_cards.Count == 0)
            {
                throw new UnexpectedBehaviourException("There are no more cards to draw in the deck. That's unfortunate... (and unexpected so the game crashes. Sorry");
            }
            else
            {
                List<T> drawnCards = new List<T>();
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
                T temp = _cards[randomNumber];

                _cards.RemoveAt(randomNumber);
                _cards.Add(temp);
            }
        }

        public Boolean Remove(T card)
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

        public Boolean Contains(Card card)
        {
            return _cards.Exists(Card => Card == card);
        }

        //Implementation to make Deck indexable
        public virtual T this[int index]
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

        //Implementation of IEnumerable&&IEnumerator
        public IEnumerator<T> GetEnumerator()
        {
            return new DeckEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DeckEnumerator<T>(this);
        }

        class DeckEnumerator<S> : IEnumerator<S> where S : Card
        {
            readonly Deck<S> deck;
            private int _currentIndex = -1;

            public DeckEnumerator(Deck<S> deck)
            {
                this.deck = deck;
            }

            public bool MoveNext()
            {
                _currentIndex++;

                return (_currentIndex < (deck._cards.Count));
            }

            public S Current
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
            
            object IEnumerator.Current
            {
                get 
                {
                    return Current;
                } 
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}