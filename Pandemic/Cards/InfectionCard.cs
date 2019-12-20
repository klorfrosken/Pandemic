using Pandemic.Game;
using Pandemic.Managers;

namespace Pandemic.Cards
{
    public class InfectionCard : Card
    {
        public InfectionCard(string Name, Colors Color, StateManager state = null) : base(Name, Color, state) { }

        public void Infect(StateManager State)
        {
            City TempCity = State.Cities[this.Name];
            TempCity.InfectCity(Color);

            State.OutbreakThisChain.Clear();
            State.InfectionDiscard.AddCard(this);
        }
    }
}
