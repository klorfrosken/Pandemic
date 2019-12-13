using Pandemic.Managers;
using Pandemic.Exceptions;
using Pandemic.Game;

namespace Pandemic.Cards
{
    public abstract class Card
    {
        public string Name { get; private set; }
        public Colors Color { get; private set; }

        public Card(string name, Colors color)
        {
            Name = name;
            Color = color;
        }

        public virtual void Play(Role playerWithCard, StateManager state)
        {
            throw new IllegalMoveException($"{Name} is not a playable card");
        }

        public override string ToString()
        {
            return $"{Name}, {Color}";
        }
    }
}
