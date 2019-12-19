using Pandemic.Managers;
using Pandemic.Exceptions;
using Pandemic.Game;

namespace Pandemic.Cards
{
    public abstract class Card
    {
        public string Name { get; private set; }
        public Colors Color { get; private set; }

        protected internal StateManager _state;

        public Card(string name, Colors color, StateManager state)
        {
            Name = name;
            Color = color;
            _state = state;
        }

        public virtual void Play(Role playerWithCard)
        {
            throw new IllegalMoveException($"{Name} is not a playable card");
        }

        public virtual bool IsFor(City city)
        {
            return this.Name == city.Name;
        }

        public override string ToString()
        {
            return $"{Name}, {Color}";
        }

    }
}
