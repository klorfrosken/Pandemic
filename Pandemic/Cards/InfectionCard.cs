using Pandemic.Game;
using Pandemic.Managers;

namespace Pandemic.Cards
{
    public class InfectionCard : Card
    {
        public InfectionCard(string Name, Colors Color) : base(Name, Color) { }

        public void Infect(StateManager State)
        {
            City TempCity = State.Cities.Find(City => City.Name == this.Name);
            TempCity.InfectCity(Color, State);

            State.OutbreakThisChain.Clear();
            State.InfectionDiscard.AddCard(this);
        }
    }
}
