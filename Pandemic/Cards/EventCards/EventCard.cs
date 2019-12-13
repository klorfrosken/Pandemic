using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Managers;

namespace Pandemic.Cards.EventCards
{
    public class EventCard : Card
    {
        public string description { get; private set; }
        public EventCard(string eventName, string description) : base(eventName, Colors.None) { }
    }
}
