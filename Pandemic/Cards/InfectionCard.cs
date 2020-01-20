using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Cards
{
    public class InfectionCard : Card
    {
        public InfectionCard(string Name, Colors Color, StateManager state = null) : base(Name, Color, state) { }

        public void Infect()
        {
            City cityToInfect;
            try
            {
                cityToInfect = _state.Cities[Name];
            }
            catch 
            {
                throw new UnexpectedBehaviourException($"There was no city corresponding to {Name} in the cities list. What could have happened to it?");
            }

            cityToInfect.InfectCity(Color);

            _state.OutbreakThisChain.Clear();
            _state.InfectionDiscard.AddCard(this);
        }
    }
}
