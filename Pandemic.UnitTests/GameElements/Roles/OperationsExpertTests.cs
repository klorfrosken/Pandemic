using Xunit;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;
using Pandemic.Cards;
using System.Collections.Generic;

namespace Pandemic.UnitTests.GameElements.Roles
{
    public class OperationsExpertTests
    {
        [Fact]
        public void BuildResearchStation_Succeeds()
        {
            StateManager state = new StateManager(Testing: true);
            City currentCity = new City("Atlanta", Colors.Blue, state);
            OperationsExpert player = new OperationsExpert(currentCity, 0, state);

            player.BuildResearchStation();

            Assert.True(currentCity.HasResearchStation);
            Assert.Equal(3, player.RemainingActions);
        }

        [Fact]
        public void BuildResearchStation_researchStationAlready_ThrowsException()
        {
            StateManager state = new StateManager(Testing: true);
            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.BuildResearchStation();
            OperationsExpert player = new OperationsExpert(currentCity, 0, state);

            Assert.Throws<IllegalMoveException>(() => player.BuildResearchStation());
        }

        [Fact]
        public void CharterFlightFromResearchStation_AlreadyUsedAbility_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            OperationsExpert player = new OperationsExpert(currentCity, 0);
            player.UsedSpecialAbility = true;
            
            Assert.Throws<IllegalMoveException>(() => player.PlayFirstSpecialAbility());
        }

        [Fact]
        public void CharterFlightFromResearchStation_NoResearchStationInCity_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            OperationsExpert player = new OperationsExpert(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => player.PlayFirstSpecialAbility());
        }

        [Fact]
        public void CharterFlightFromResearchStation_NoCardsInHand_ThrowsException()
        {
            StateManager state = new StateManager(Testing: true);
            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.BuildResearchStation();
            OperationsExpert player = new OperationsExpert(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => player.PlayFirstSpecialAbility());
        }

        [Fact]
        public void CharterFlightFromResearchStation_OneCardInHand_Succeeds()
        {
            StateManager state = new StateManager(Testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);

            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.BuildResearchStation();
            City nextCity = new City("Miami", Colors.Yellow);

            state.Cities = new Dictionary<string, City> {
                    {"Atlanta", currentCity },
                    { "Miami", nextCity } };

            OperationsExpert player = new OperationsExpert(currentCity, 0, state, txtMgr);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);
            player.Hand.Add(card);

            player.PlayFirstSpecialAbility();

            Assert.Equal(nextCity, player.CurrentCity);
            Assert.Empty(player.Hand);
            Assert.True(player.UsedSpecialAbility);
            Assert.Contains(currentCity, state.Cities.Values);
        }

        [Fact]
        public void CharterFlightFromResearchStation_MultipleCardsInHand_Succeeds()
        {
            StateManager state = new StateManager(Testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);

            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.BuildResearchStation();
            City nextCity = new City("Miami", Colors.Yellow);

            state.Cities = new Dictionary<string, City> {
                    {"Atlanta", currentCity },
                    { "Miami", nextCity } };

            OperationsExpert player = new OperationsExpert(currentCity, 0, state, txtMgr);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);
            PlayerCard card2 = new CityCard("Miami", Colors.Yellow);
            player.Hand.Add(card);
            player.Hand.Add(card2);

            player.PlayFirstSpecialAbility();

            Assert.Equal(nextCity, player.CurrentCity);
            Assert.Single(player.Hand);
            Assert.Contains(card2, player.Hand);
            Assert.True(player.UsedSpecialAbility);
            Assert.Contains(currentCity, state.Cities.Values);
        }
    }
}
