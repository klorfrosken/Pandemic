using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Managers;

namespace Pandemic.Cards
{
    public abstract class PlayerCard : Card
    {
        public PlayerCard(string name, Colors color, StateManager state = null) : base(name, color, state) { }
    }
}
