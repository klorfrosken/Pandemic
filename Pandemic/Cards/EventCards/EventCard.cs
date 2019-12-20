using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Managers;

namespace Pandemic.Cards.EventCards
{
    public class EventCard : PlayerCard
    {
        public string description { get; private set; }
        protected TextManager textManager;
        
        public EventCard(string eventName, string description, StateManager state, TextManager textManager) : base(eventName, Colors.None, state) 
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
