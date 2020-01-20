using System.Collections.Generic;
using Xunit;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Cards;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;

namespace Pandemic.UnitTests.Cards
{
    public class InfectionCardTests
    {
        [Fact]
        public void Infect_CityIsInfected()
        {
            Dictionary<string, City> cities = new Dictionary<string, City>();
            List<City> outbreakThisChain = new List<City>();
            StateManager state = new StateManager(
                Testing: true, 
                Cities: cities,
                OutbreakThisChain: outbreakThisChain);
            ITextManager textMgr = new TestTextManager();
            City cityToInfect = new City("testCity", Colors.Blue, state, textMgr);
            cities["testCity"] = cityToInfect;
            outbreakThisChain.Add(cityToInfect);

            InfectionCard testCard = new InfectionCard("testCity", Colors.Blue, state);

            testCard.Infect();

            Assert.Equal(1, cityToInfect.DiseaseCubes[Colors.Blue]);
            Assert.Empty(state.OutbreakThisChain);
            Assert.Contains(testCard, state.InfectionDiscard);
        }

        [Fact]
        public void Infect_NoCorrespondingCity_ThrowsException()
        {
            StateManager state = new StateManager(
                Testing: true);

            InfectionCard testCard = new InfectionCard("testCity", Colors.Blue, state);

            Assert.Throws<UnexpectedBehaviourException>(() => testCard.Infect());
        }
    }
}