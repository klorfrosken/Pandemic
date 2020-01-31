using System.Collections.Generic;
using Xunit;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Cards;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;
using Pandemic.Cards.EventCards;

namespace Pandemic.UnitTests.GameElements
{
    public class RoleTests
    {
        [Fact]
        public void Discard_cardToBeDiscarded_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            List<PlayerCard> cards = new List<PlayerCard>
            {
                new CityCard("Atlanta", Colors.Blue),
                new CityCard("Miami", Colors.Yellow),
            };
            ITextManager txtMgr = new TestTextManager(discard_PlayInteger: 0);
            Role player = new QuarantineSpecialist(currentCity, 0, null, txtMgr);
            player.Hand = new List<PlayerCard>(cards);

            player.TestDiscard();

            Assert.Single(player.Hand);
            Assert.Contains(cards[1], player.Hand);
        }

        [Fact]
        public void Discard_cardToBePlayed_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            List<PlayerCard> cards = new List<PlayerCard>
            {
                new TestEvent(),
                new CityCard("Miami", Colors.Yellow),
            };
            ITextManager txtMgr = new TestTextManager(discard_PlayInteger: 0);
            Role player = new QuarantineSpecialist(currentCity, 0, null, txtMgr);
            player.Hand = new List<PlayerCard>(cards);

            player.TestDiscard();

            Assert.Single(player.Hand);
            Assert.Contains(cards[1], player.Hand);
        }

        [Fact]
        public void Draw_cardIsCityCard_Succeeds()
        {
            StateManager state = new StateManager(testing: true);

            City atlanta = new City("Atlanta", Colors.Blue, state);
            PlayerCard testCard = new CityCard("Atlanta", Colors.Blue, state);
            state.PlayerDeck.AddCard(testCard);

            Role testRole = new Scientist(atlanta, 0, state);

            testRole.Draw();

            Assert.Contains(testCard, testRole.Hand);
        }

        [Fact]
        public void Draw_cardIsEpidemic_Succeeds()
        {
            StateManager state = new StateManager(
                testing: true,
                infectionIndex: 2);

            List<PlayerCard> cards = new List<PlayerCard>
            {
                new EpidemicCard(state)
            };
            state.PlayerDeck.AddCards(cards);
            state.InfectionDeck.AddCard(new InfectionCard("Atlanta", Colors.Blue, state));

            ITextManager textMgr = new TestTextManager();
            City atlanta = new City("Atlanta", Colors.Blue, state, textMgr);
            state.Cities["Atlanta"] = atlanta;
            Role testRole = new Scientist(atlanta, 0, state);

            testRole.Draw();

            Assert.Equal(3, state.InfectionIndex);
        }

        [Fact]
        public void Draw_PlayerDeckIsEmpty_ThrowsException()
        {
            StateManager state = new StateManager(
                testing: true);

            City atlanta = new City("Atlanta", Colors.Blue, state);
            Role testRole = new Scientist(atlanta, 0, state);

            Assert.Throws<TheWorldIsDeadException>(() => testRole.Draw());
        }
        [Fact]
        public void Draw_multipleCardsDrawn_Succeeds()
        {
            StateManager state = new StateManager(
                testing: true,
                infectionIndex: 2);

            List<PlayerCard> cards = new List<PlayerCard>
            {
                new CityCard("Atlanta", Colors.Blue),
                new CityCard("Miami", Colors.Yellow)
            };
            state.PlayerDeck.AddCards(cards);

            ITextManager textMgr = new TestTextManager();
            City atlanta = new City("Atlanta", Colors.Blue, state, textMgr);
            Role testRole = new Scientist(atlanta, 0, state);

            testRole.Draw(state.PlayerDeck, 2);

            Assert.Equal(2, testRole.Hand.Count);
            Assert.Contains(cards[0], testRole.Hand);
            Assert.Contains(cards[1], testRole.Hand);
        }

        [Fact]
        public void ChangeCity_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Miami", Colors.Yellow);

            Role testRole = new Scientist(currentCity, 0);

            testRole.ChangeCity(nextCity);

            Assert.Equal(nextCity, testRole.CurrentCity);
        }

        [Fact]
        public void DriveFerry_citiesAreConnected_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Miami", Colors.Yellow);
            currentCity.ConnectedCities.Add(nextCity);

            Role testRole = new Scientist(currentCity, 0);

            testRole.DriveFerry(nextCity);

            Assert.Equal(nextCity, testRole.CurrentCity);
            Assert.Equal(3, testRole.RemainingActions);
        }

        [Fact]
        public void DriveFerry_citiesAreNotConnected_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Miami", Colors.Yellow);

            Role testRole = new Scientist(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => testRole.DriveFerry(nextCity));
        }

        [Fact]
        public void DirectFlight_hasCardInHand_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Miami", Colors.Yellow);

            PlayerCard card = new CityCard("Miami", Colors.Yellow);

            Role testRole = new Scientist(currentCity, 0);
            testRole.Hand.Add(card);

            testRole.DirectFlight(nextCity);

            Assert.Equal(nextCity, testRole.CurrentCity);
            Assert.Empty(testRole.Hand);
            Assert.Equal(3, testRole.RemainingActions);
        }

        [Fact]
        public void DirectFlight_cardNotInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Miami", Colors.Yellow);

            Role testRole = new Scientist(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => testRole.DirectFlight(nextCity));
        }

        [Fact]
        public void CharterFlight_hasCardInHand_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Miami", Colors.Yellow);

            PlayerCard card = new CityCard("Atlanta", Colors.Yellow);

            Role testRole = new Scientist(currentCity, 0);
            testRole.Hand.Add(card);

            testRole.CharterFlight(nextCity);

            Assert.Equal(nextCity, testRole.CurrentCity);
            Assert.Empty(testRole.Hand);
            Assert.Equal(3, testRole.RemainingActions);
        }

        [Fact]
        public void CharterFlight_cardNotInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Miami", Colors.Yellow);

            Role testRole = new Scientist(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => testRole.CharterFlight(nextCity));
        }

        [Fact]
        public void ShuttleFlight_Succeeds()
        {
            StateManager state = new StateManager(testing: true);

            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.BuildResearchStation();
            City nextCity = new City("Miami", Colors.Yellow, state);
            nextCity.BuildResearchStation();

            Role testRole = new Scientist(currentCity, 0);

            testRole.ShuttleFlight(nextCity);

            Assert.Equal(nextCity, testRole.CurrentCity);
            Assert.Equal(3, testRole.RemainingActions);
        }

        [Fact]
        public void ShuttleFlight_currentCityWithoutResearchStation_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);

            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Miami", Colors.Yellow, state);
            nextCity.BuildResearchStation();

            Role testRole = new Scientist(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => testRole.ShuttleFlight(nextCity));
        }

        [Fact]
        public void ShuttleFlight_nextCityWithoutResearchStation_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);

            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.BuildResearchStation();
            City nextCity = new City("Miami", Colors.Yellow);

            Role testRole = new Scientist(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => testRole.ShuttleFlight(nextCity));
        }

        [Fact]
        public void BuildResearchStation_Succeeds()
        {
            StateManager state = new StateManager(
                testing: true,
                remainingResearchStations: 4);
            City currentCity = new City("Atlanta", Colors.Blue, state);
            Role testRole = new Scientist(currentCity, 0, state);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);
            testRole.Hand.Add(card);

            testRole.BuildResearchStation();

            Assert.True(currentCity.HasResearchStation);
            Assert.Empty(testRole.Hand);
            Assert.Equal(3, testRole.RemainingActions);
            Assert.Equal(3, state.RemainingResearchStations);
        }

        [Fact]
        public void BuildResearchStation_cardNotInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            Role testRole = new Scientist(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => testRole.BuildResearchStation());
        }

        [Fact]
        public void BuildResearchStation_alreadyResearchStationInCity_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);
            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.BuildResearchStation();
            Role testRole = new Scientist(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => testRole.BuildResearchStation());
        }

        [Theory]
        [InlineData(Colors.Yellow)]
        [InlineData(Colors.Red)]
        [InlineData(Colors.Blue)]
        [InlineData(Colors.Black)]
        public void TreatDisease_validColorGiven_cubeRemoved(Colors color)
        {
            Colors currentColor = color;
            StateManager state = new StateManager(
                testing: true,
                maxCubesInCubePool: 12);
            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.DiseaseCubes[currentColor]++;

            Role currentRole = new Scientist(currentCity, 0, state);

            currentRole.TreatDisease(currentColor);

            Assert.Equal(0, currentCity.DiseaseCubes[currentColor]);
            Assert.Equal(13, state.CubePools[currentColor]);
            Assert.Equal(3, currentRole.RemainingActions);
        }

        [Fact]
        public void TreatDisease_colorIsNone_ThrowsException()
        {
            Colors currentColor = Colors.None;
            StateManager state = new StateManager(
                testing: true,
                maxCubesInCubePool: 12);
            City currentCity = new City("Atlanta", Colors.Blue, state);

            Role currentRole = new Scientist(currentCity, 0, state);

            Assert.Throws<UnexpectedBehaviourException>(() => currentRole.TreatDisease(currentColor));
        }

        [Fact]
        public void TreatDisease_noCubesOfThatColorInCity_ThrowsException()
        {
            Colors currentColor = Colors.Yellow;
            StateManager state = new StateManager(
                testing: true,
                maxCubesInCubePool: 12);
            City currentCity = new City("Atlanta", Colors.Blue, state);

            Role currentRole = new Scientist(currentCity, 0, state);

            Assert.Throws<IllegalMoveException>(() => currentRole.TreatDisease(currentColor));
        }

        [Fact]
        public void DiscoverCure_noResearchStationInCity_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);

            Role currentRole = new Researcher(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => currentRole.DiscoverCure());
        }

        [Fact]
        public void DiscoverCure_HasAppropiateNumberOfCardsInHand_Succeeds()
        {
            Colors currentColor = Colors.Yellow;
            StateManager state = new StateManager(
                testing: true);
            ITextManager txtMgr = new TestTextManager();
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            currentCity.BuildResearchStation();
            state.Cities["Atlanta"] = currentCity;

            List<PlayerCard> cards = new List<PlayerCard>
            {
                new CityCard("card1", currentColor, state),
                new CityCard("card2", currentColor, state),
                new CityCard("card3", currentColor, state),
                new CityCard("card4", currentColor, state),
                new CityCard("card5", currentColor, state)
            };

            Role currentRole = new Researcher(currentCity, 0, state);
            currentRole.Hand = cards;

            currentRole.DiscoverCure();

            Assert.True(state.Cures[currentColor]);
            Assert.Empty(currentRole.Hand);
            Assert.Equal(3, currentRole.RemainingActions);
        }

        [Fact]
        public void DiscoverCure_InsufficientNumberOfCardsInHand_ThrowsException()
        {
            Colors currentColor = Colors.Yellow;
            StateManager state = new StateManager(
                testing: true);
            ITextManager txtMgr = new TestTextManager();
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            currentCity.BuildResearchStation();
            state.Cities["Atlanta"] = currentCity;

            List<PlayerCard> cards = new List<PlayerCard>
            {
                new CityCard("card1", currentColor, state),
                new CityCard("card2", currentColor, state),
                new CityCard("card3", currentColor, state),
                new CityCard("card4", currentColor, state)
            };

            Role currentRole = new Researcher(currentCity, 0, state);
            currentRole.Hand = cards;

            Assert.Throws<IllegalMoveException>(() => currentRole.DiscoverCure());
        }

        [Fact]
        public void DiscoverCure_CureAlreadyDiscovered_ThrowsException()
        {
            Colors currentColor = Colors.Yellow;
            StateManager state = new StateManager(
                testing: true,
                cures: new Dictionary<Colors, bool>{
                    {currentColor, true } });
            ITextManager txtMgr = new TestTextManager();
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            currentCity.BuildResearchStation();
            state.Cities["Atlanta"] = currentCity;

            List<PlayerCard> cards = new List<PlayerCard>
            {
                new CityCard("card1", currentColor, state),
                new CityCard("card2", currentColor, state),
                new CityCard("card3", currentColor, state),
                new CityCard("card4", currentColor, state),
                new CityCard("card5", currentColor, state)
            };

            Role currentRole = new Researcher(currentCity, 0, state);
            currentRole.Hand = cards;

            Assert.Throws<IllegalMoveException>(() => currentRole.DiscoverCure());
        }

        [Fact]
        public void DiscoverCure_HasExtraCardsInHand_Succeeds()
        {
            Colors currentColor = Colors.Yellow;
            StateManager state = new StateManager(
                testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1);
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            currentCity.BuildResearchStation();
            state.Cities["Atlanta"] = currentCity;

            List<PlayerCard> cards = new List<PlayerCard>
            {
                new CityCard("card1", currentColor, state),
                new CityCard("card2", currentColor, state),
                new CityCard("card3", currentColor, state),
                new CityCard("card4", currentColor, state),
                new CityCard("card5", currentColor, state),
                new CityCard("card6", currentColor, state)
            };

            Role currentRole = new Researcher(currentCity, 0, state, txtMgr);
            currentRole.Hand = cards;

            currentRole.DiscoverCure();

            Assert.True(state.Cures[currentColor]);
            Assert.Single(currentRole.Hand);
            Assert.Equal(3, currentRole.RemainingActions);
        }


        [Fact]
        public void DiscoverCure_LastCureIsFound_ThrowsException()
        {
            Colors currentColor = Colors.Yellow;
            StateManager state = new StateManager(
                testing: true,
                cures: new Dictionary<Colors, bool>{
                    {Colors.Yellow, false },
                    {Colors.Red, true },
                    {Colors.Blue, true },
                    {Colors.Black, true } });
            ITextManager txtMgr = new TestTextManager();
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            currentCity.BuildResearchStation();
            state.Cities["Atlanta"] = currentCity;

            List<PlayerCard> cards = new List<PlayerCard>
            {
                new CityCard("card1", currentColor, state),
                new CityCard("card2", currentColor, state),
                new CityCard("card3", currentColor, state),
                new CityCard("card4", currentColor, state),
                new CityCard("card5", currentColor, state),
            };

            Role currentRole = new Researcher(currentCity, 0, state, txtMgr);
            currentRole.Hand = cards;

            Assert.Throws<GameWonException>(() => currentRole.DiscoverCure());
        }

        [Fact]
        public void ReceiveCard_HandBelowLimit_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            ITextManager txtMgr = new TestTextManager(discard_PlayInteger: 0);
            Role player = new QuarantineSpecialist(currentCity, 0, null, txtMgr);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);

            player.ReceiveCard(card);

            Assert.Single(player.Hand);
        }

        [Fact]
        public void ReceiveCard_HandAboveLimit_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            ITextManager txtMgr = new TestTextManager(discard_PlayInteger: 0);
            Role player = new QuarantineSpecialist(currentCity, 0, null, txtMgr);
            player.Hand = new List<PlayerCard>
            {
                new CityCard("card", Colors.Blue),
                new CityCard("card", Colors.Blue),
                new CityCard("card", Colors.Blue),
                new CityCard("card", Colors.Blue),
                new CityCard("card", Colors.Blue),
                new CityCard("card", Colors.Blue),
                new CityCard("card", Colors.Blue)
            };
            PlayerCard newCard = new CityCard("Atlanta", Colors.Blue);

            player.ReceiveCard(newCard);

            Assert.Equal(7, player.Hand.Count);
            Assert.Contains(newCard, player.Hand);
        }

        [Fact]
        public void GiveCard_cardToGiveIsInHand_Succeeds()
        {
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);
            City atlanta = new City("Atlanta", Colors.Blue);
            Role currentRole = new Scientist(atlanta, 0);
            Role otherRole = new Scientist(atlanta, 1);
            currentRole.Hand.Add(card);

            currentRole.GiveCard(otherRole);

            Assert.Empty(currentRole.Hand);
            Assert.Single(otherRole.Hand);
            Assert.Contains(card, otherRole.Hand);
        }

        [Fact]
        public void GiveCard_cardToGiveIsInNotHand_ThrowsException()
        {
            City atlanta = new City("Atlanta", Colors.Blue);
            Role currentRole = new Scientist(atlanta, 0);
            Role otherRole = new Scientist(atlanta, 1);

            Assert.Throws<IllegalMoveException>(() => currentRole.GiveCard(otherRole));
        }

        [Fact]
        public void ShareKnowledge_otherRoleIsNotInSameCity_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City otherCity = new City("Miami", Colors.Yellow);
            Role currentRole = new OperationsExpert(currentCity, 0);
            Role otherRole = new OperationsExpert(otherCity, 1);

            Assert.Throws<IllegalMoveException>(() => currentRole.ShareKnowledge(otherRole));
        }

        [Fact]
        public void ShareKnowledge_otherRoleNotResearcher_otherRoleCardInHand_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);
            Role currentRole = new OperationsExpert(currentCity, 0);
            Role otherRole = new OperationsExpert(currentCity, 1);
            otherRole.Hand.Add(card);

            currentRole.ShareKnowledge(otherRole);

            Assert.Empty(otherRole.Hand);
            Assert.Single(currentRole.Hand);
            Assert.Contains(card, currentRole.Hand);
            Assert.Equal(3, currentRole.RemainingActions);
        }

        [Fact]
        public void ShareKnowledge_otherRoleNotResearcher_currentRoleCardInHand_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);
            Role currentRole = new OperationsExpert(currentCity, 0);
            Role otherRole = new OperationsExpert(currentCity, 1);
            currentRole.Hand.Add(card);

            currentRole.ShareKnowledge(otherRole);

            Assert.Empty(currentRole.Hand);
            Assert.Single(otherRole.Hand);
            Assert.Contains(card, otherRole.Hand);
            Assert.Equal(3, currentRole.RemainingActions);
        }

        [Fact]
        public void ShareKnowledge_otherRoleNotResearcher_NoRoleCardInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);
            Role currentRole = new OperationsExpert(currentCity, 0);
            Role otherRole = new OperationsExpert(currentCity, 1);

            Assert.Throws<IllegalMoveException>(() => currentRole.ShareKnowledge(otherRole));
        }

        [Fact]
        public void ShareKnowledge_otherRoleIsResearcher_ResearcherGivesCard_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);

            ITextManager playerTxtMgr = new TestTextManager(shareKnowledge: 1);
            Role currentRole = new OperationsExpert(currentCity, 0, null, playerTxtMgr);

            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            Role otherRole = new Researcher(currentCity, 1, null, txtMgr);
            otherRole.Hand.Add(card);

            currentRole.ShareKnowledge(otherRole);

            Assert.Empty(otherRole.Hand);
            Assert.Single(currentRole.Hand);
            Assert.Contains(card, currentRole.Hand);
            Assert.Equal(3, currentRole.RemainingActions);
        }

        [Fact]
        public void ShareKnowledge_otherRoleIsResearcher_ResearcherReceivesCard_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);

            ITextManager playerTxtMgr = new TestTextManager(shareKnowledge: 2);
            Role currentRole = new OperationsExpert(currentCity, 0, null, playerTxtMgr);

            Role otherRole = new Researcher(currentCity, 1);
            currentRole.Hand.Add(card);

            currentRole.ShareKnowledge(otherRole);

            Assert.Empty(currentRole.Hand);
            Assert.Single(otherRole.Hand);
            Assert.Contains(card, otherRole.Hand);
            Assert.Equal(3, currentRole.RemainingActions);
        }

        [Fact]
        public void NumberOfCityCardsInHand_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            Role currentRole = new OperationsExpert(currentCity, 0);
            currentRole.Hand = new List<PlayerCard>
            {
                new CityCard("Atlanta", Colors.Blue),
                new OneQuietNight(),
                new CityCard("Miami", Colors.Yellow)
            };

            int actual = currentRole.NumberOfCityCardsInHand();

            Assert.Equal(2, actual);
        }
    }
}
