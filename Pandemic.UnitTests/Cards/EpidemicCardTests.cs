using System.Collections.Generic;
using Xunit;
using Pandemic.Managers;
using Pandemic.Cards;
using Pandemic.Game;
using Pandemic.UnitTests.TestClasses;

namespace Pandemic.UnitTests.Cards
{
    public class EpidemicCardTests
    {
        [Fact]
        public void Increase_InfectionRateIncreased()
        {
            StateManager state = new StateManager(
                Testing: true,
                InfectionIndex: 2);

            EpidemicCard testCard = new EpidemicCard(state);

            testCard.TestIncrease();

            Assert.Equal(3, state.InfectionIndex);
        }

        [Fact]
        public void Infect_NewCityInfected()
        {
            StateManager state = new StateManager(
                Testing: true);
            ITextManager textMgr = new TestTextManager();

            InfectionCard infectionCard = new InfectionCard("Atlanta", Colors.Blue, state);
            state.InfectionDeck.AddCard(infectionCard);

            City infectionCity = new City("Atlanta", Colors.Blue, state, textMgr);
            state.Cities["Atlanta"] = infectionCity;

            EpidemicCard testCard = new EpidemicCard(state);

            testCard.TestInfect();

            Assert.Equal(3, infectionCity.DiseaseCubes[Colors.Blue]);
            Assert.Empty(state.InfectionDeck);
            Assert.Contains(infectionCard, state.InfectionDiscard);
        }

        [Fact]
        public void Intensify_Succeeds()
        {
            StateManager state = new StateManager(Testing: true);

            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard,
            };

            state.InfectionDiscard = new InfectionDeck(infectionDeckCards);
            InfectionDeck unshuffledDeck = new InfectionDeck(infectionDeckCards);

            EpidemicCard testCard = new EpidemicCard(state);

            testCard.TestIntensify();

            Assert.Empty(state.InfectionDiscard);
            Assert.NotEmpty(state.InfectionDeck);
            Assert.NotEqual(unshuffledDeck, state.InfectionDeck);
        }
    }
}
