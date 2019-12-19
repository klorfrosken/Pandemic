using System;
using System.Collections.Generic;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Cards
{
    public class EpidemicCard : PlayerCard
    {
        readonly static string _name = "Epidemic";

        public EpidemicCard(StateManager state) : base(_name, Colors.None, state) { }

        public override void Play(Role playerWithCard)
        {
            Increase();
            Infect();
            Intensify();
        }

        void Increase()
        {
            _state.IncreaseInfectionRate();
        }

        void Infect()
        {
            InfectionCard bottomCard = _state.InfectionDeck.RemoveBottomCard();
            bottomCard.Infect(_state);
            bottomCard.Infect(_state);
            bottomCard.Infect(_state);
        }

        void Intensify()
        {
            _state.InfectionDiscard.Shuffle();
            _state.InfectionDeck.InsertOnTop(_state.InfectionDiscard);
        }
    }
}