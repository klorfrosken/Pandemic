using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Managers;

namespace Pandemic.Cards.EventCards
{
    public class EventCard : PlayerCard
    {
        public string description { get; private set; }
        
        public EventCard(string eventName, string description, StateManager state) : base(eventName, Colors.None, state) 
        {
            this.description = description;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
