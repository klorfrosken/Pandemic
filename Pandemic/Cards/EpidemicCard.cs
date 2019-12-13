using System;
using System.Collections.Generic;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Cards
{
    public class EpidemicCard : Card
    {
        readonly static string CardName = "Epidemic";

        public EpidemicCard() : base(CardName, Colors.None) { }

        public override void Play(Role playerWithCard, StateManager state)
        {
            Increase(state);
            Infect(state);
            Intensify(state);
        }

        void Increase(StateManager state)
        {
            state.IncreaseInfectionRate();
        }

        void Infect(StateManager state)
        {
            InfectionCard bottomCard = state.InfectionDeck.RemoveBottomCard();
            bottomCard.Infect(state);
            bottomCard.Infect(state);
            bottomCard.Infect(state);
        }

        void Intensify(StateManager state)
        {
            state.InfectionDiscard.Shuffle();
            state.InfectionDeck.InsertOnTop(state.InfectionDiscard);
        }
    }
}