using Pandemic.Managers;

namespace Pandemic.Cards.EventCards
{
    public abstract class EventCard : PlayerCard
    {
        public string description { get; private set; }
        protected ITextManager textManager;
        
        public EventCard(string eventName, string description, StateManager state = null, ITextManager textManager = null) : base(eventName, Colors.None, state) 
        {
            this.description = description;
            this.textManager = textManager;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
