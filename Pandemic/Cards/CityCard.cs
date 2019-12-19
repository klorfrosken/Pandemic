using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Managers;

namespace Pandemic.Cards
{
    public class CityCard : PlayerCard
    {
        public CityCard(string Name, Colors Color, StateManager state) : base(Name, Color, state) { }
    }
}
