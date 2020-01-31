using Xunit;
using System;
using System.Collections.Generic;
using Pandemic.Cards;
using Pandemic.Game;
using Pandemic.Cards.EventCards;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.Managers;
using Pandemic.UnitTests.TestClasses;


namespace Pandemic.UnitTests.Managers
{
    public class GameManagerTests
    {
        [Fact]
        void DriveFerry_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1);
            GameManager game = new GameManager(testing: true, txtMgr: txtMgr);

            City nextCity = new City("Washington", Colors.Blue, state, txtMgr);
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            currentCity.ConnectedCities = new List<City>
            {
                new City("Miami", Colors.Yellow, state, txtMgr),
                nextCity,
                new City("Chicago", Colors.Blue, state, txtMgr)
            };

            Researcher player = new Researcher(currentCity, 0, state, txtMgr);

            game.TestDriveFerry(player);

            Assert.Equal(nextCity, player.CurrentCity);
        }

        [Fact]
        void DirectFlight_NoEligibleCards_ThrowsException()
        {
            GameManager game = new GameManager(testing: true);

            City currentCity = new City("Atlanta", Colors.Blue);
            Researcher player = new Researcher(currentCity, 0);

            Exception ex = Assert.Throws<IllegalMoveException>(() => game.TestDirectFlight(player));
            Assert.Equal("You don't have any cards you can discard for a direct flight", ex.Message);
        }

        [Fact]
        void DirectFlight_SingleEligibleCard_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1);
            GameManager game = new GameManager(testing: true, state, txtMgr);

            City nextCity = new City("Lima", Colors.Yellow, state, txtMgr);
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            state.Cities["Lima"] = nextCity;
            CityCard card = new CityCard("Lima", Colors.Yellow, state);

            Researcher player = new Researcher(currentCity, 0, state, txtMgr);
            player.Hand.Add(card);

            game.TestDirectFlight(player);

            Assert.Equal(nextCity, player.CurrentCity);
            Assert.Empty(player.Hand);
        }

        [Fact]
        void DirectFlight_MultipleEligibleCards_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1);
            GameManager game = new GameManager(testing: true, state, txtMgr);

            City nextCity = new City("Lima", Colors.Yellow, state, txtMgr);
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            state.Cities["Lima"] = nextCity;

            Researcher player = new Researcher(currentCity, 0, state, txtMgr);

            CityCard card = new CityCard("Lima", Colors.Yellow, state);
            CityCard otherCard = new CityCard("Atlanta", Colors.Blue);
            player.Hand.Add(otherCard);
            player.Hand.Add(card);

            game.TestDirectFlight(player);

            Assert.Equal(nextCity, player.CurrentCity);
            Assert.Single(player.Hand);
            Assert.Contains(otherCard, player.Hand);
        }

        [Fact]
        public void CharterFlight_NoEligibleCards_ThrowsException()
        {
            GameManager game = new GameManager(testing: true);

            City currentCity = new City("Atlanta", Colors.Blue);
            Researcher player = new Researcher(currentCity, 0);

            Exception ex = Assert.Throws<IllegalMoveException>(() => game.TestCharterFlight(player));
            Assert.Equal($"You need to have the City Card for your current city, {player.CurrentCity} in your hand in order to charter a flight from there.", ex.Message);
        }

        [Fact]
        void CharterFlight_CardInHand_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            GameManager game = new GameManager(testing: true, state, txtMgr);

            City nextCity = new City("Lima", Colors.Yellow, state, txtMgr);
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            state.Cities["Lima"] = nextCity;
            CityCard card = new CityCard("Atlanta", Colors.Yellow, state);

            Researcher player = new Researcher(currentCity, 0, state, txtMgr);
            player.Hand.Add(card);

            game.TestCharterFlight(player);

            Assert.Equal(nextCity, player.CurrentCity);
            Assert.Empty(player.Hand);
        }

        [Fact]
        public void ShuttleFlight_NoResearchStationInCity_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);
            GameManager game = new GameManager(testing: true, state);

            City nextCity = new City("Lima", Colors.Yellow);
            City currentCity = new City("Atlanta", Colors.Blue);
            nextCity.HasResearchStation = true;
            state.Cities["Lima"] = nextCity;
            Researcher player = new Researcher(currentCity, 0);

            Exception ex = Assert.Throws<IllegalMoveException>(() => game.TestShuttleFlight(player));
            Assert.Equal("There has to be a research station in the city you are in for you to shuttle a flight.", ex.Message);
        }

        [Fact]
        public void ShuttleFlight_NoResearchStationInOtherCities_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);
            GameManager game = new GameManager(testing: true, state);

            City nextCity = new City("Lima", Colors.Yellow);
            City currentCity = new City("Atlanta", Colors.Blue);
            currentCity.HasResearchStation = true;
            state.Cities["Lima"] = nextCity;
            state.Cities["Atlanta"] = currentCity;
            Researcher player = new Researcher(currentCity, 0);

            Exception ex = Assert.Throws<IllegalMoveException>(() => game.TestShuttleFlight(player));
            Assert.Equal("There needs to be a research station in both the city someone is moving from and the city they are moving to, for you to shuttle a flight. \nCurrently there's only one research station in the game...", ex.Message);
        }

        [Fact]
        public void ShuttleFlight_ResearchStationsPresent_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            GameManager game = new GameManager(testing: true, state, txtMgr);

            City nextCity = new City("Lima", Colors.Yellow);
            City currentCity = new City("Atlanta", Colors.Blue);
            currentCity.HasResearchStation = true;
            nextCity.HasResearchStation = true;

            state.Cities["Lima"] = nextCity;
            state.Cities["Atlanta"] = currentCity;

            Researcher player = new Researcher(currentCity, 0);

            game.TestShuttleFlight(player);

            Assert.Equal(nextCity, player.CurrentCity);
        }

        [Fact]
        public void BuildResearchStation_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager();
            GameManager game = new GameManager(testing: true, state, txtMgr);

            City currentCity = new City("Atlanta", Colors.Blue, state);
            state.Cities["Atlanta"] = currentCity;

            CityCard card = new CityCard("Atlanta", Colors.Blue);
            Researcher player = new Researcher(currentCity, 0);
            player.Hand.Add(card);

            game.TestBuilldResearchStation(player);

            Assert.True(currentCity.HasResearchStation);
        }

        [Fact]
        public void TreatDisease_SingleTypeOfDisease_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager();
            GameManager game = new GameManager(testing: true, state, txtMgr);

            Colors currentColor = Colors.Blue;
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            currentCity.DiseaseCubes[currentColor] = 3;

            Researcher player = new Researcher(currentCity, 0);

            game.TestTreatDisease(player);

            Assert.Equal(2, currentCity.DiseaseCubes[currentColor]);
        }

        [Fact]
        public void TreatDisease_MultipleTypesOfDisease_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            GameManager game = new GameManager(testing: true, state, txtMgr);

            Colors currentColor = Colors.Blue;
            Colors otherColor = Colors.Yellow;
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            currentCity.DiseaseCubes[currentColor] = 3;
            currentCity.DiseaseCubes[otherColor] = 3;

            Researcher player = new Researcher(currentCity, 0);

            game.TestTreatDisease(player);

            Assert.Equal(2, currentCity.DiseaseCubes[currentColor]);
            Assert.Equal(3, currentCity.DiseaseCubes[otherColor]);
        }

        [Fact]
        public void DiscoverCure_NoResearchStationInCity_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager();
            GameManager game = new GameManager(testing: true, state, txtMgr);

            City currentCity = new City("Atlanta", Colors.Blue, state);

            Researcher player = new Researcher(currentCity, 0);

            Exception ex = Assert.Throws<IllegalMoveException>(() => game.TestDiscoverCure(player));
            Assert.Equal($"There's no research station in {player.CurrentCity}. There has to be a research station in your location for you to discover a cure.", ex.Message);
        }

        [Fact]
        public void DiscoverCure_CureDiscovered_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager();
            GameManager game = new GameManager(testing: true, state, txtMgr);

            Colors currentColor = Colors.Blue;
            state.Cures[currentColor] = false;

            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.HasResearchStation = true;

            Researcher player = new Researcher(currentCity, 0, state);
            player.Hand = new List<PlayerCard>
            {
                new CityCard("Atlanta", currentColor),
                new CityCard("Washington", currentColor),
                new CityCard("Paris", currentColor),
                new CityCard("Essen", currentColor),
                new CityCard("Chicago", currentColor)
            };

            game.TestDiscoverCure(player);

            Assert.True(state.Cures[currentColor]);
        }

        [Fact]
        public void ShareKnowledge_NoOtherPlayersInCity_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);
            GameManager game = new GameManager(testing: true, state);

            City currentCity = new City("Atlanta", Colors.Blue);
            City otherCity = new City("Lima", Colors.Yellow);

            QuarantineSpecialist player = new QuarantineSpecialist(currentCity, 0);
            Scientist otherPlayer = new Scientist(otherCity, 1);

            Exception ex = Assert.Throws<IllegalMoveException>(() => game.TestShareKnowledge(player));
            Assert.Equal($"There are no other players in {player.CurrentCity} for you to share knowledge with.", ex.Message);
        }

        [Fact]
        public void ShareKnowledge_SingleOtherPlayerInCity_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            GameManager game = new GameManager(testing: true, state);

            City currentCity = new City("Atlanta", Colors.Blue);

            QuarantineSpecialist player = new QuarantineSpecialist(currentCity, 0);
            Scientist otherPlayer = new Scientist(currentCity, 1);
            state.Roles = new List<Role>
            {
                player,
                otherPlayer
            };

            CityCard card = new CityCard("Atlanta", Colors.Blue);
            player.Hand.Add(card);

            game.TestShareKnowledge(player);

            Assert.Single(otherPlayer.Hand);
            Assert.Contains(card, otherPlayer.Hand);
        }

        [Fact]
        public void ShareKnowledge_MultipleOtherPlayersInCity_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            GameManager game = new GameManager(testing: true, state, txtMgr);

            City currentCity = new City("Atlanta", Colors.Blue);

            QuarantineSpecialist player = new QuarantineSpecialist(currentCity, 0);
            Scientist otherPlayer = new Scientist(currentCity, 1);
            Medic thirdPlayer = new Medic(currentCity, 2);
            state.Roles = new List<Role>
            {
                player,
                otherPlayer,
                thirdPlayer
            };

            CityCard card = new CityCard("Atlanta", Colors.Blue);
            player.Hand.Add(card);

            game.TestShareKnowledge(player);

            Assert.Single(otherPlayer.Hand);
            Assert.Contains(card, otherPlayer.Hand);
        }

        [Fact]
        public void PlayGame_NoCardsToDraw_ThrowsException()
        {
            StateManager state = new StateManager(testing: true, maxPlayerActions: 1);
            ITextManager txtMgr = new TestTextManager(itemNumber: 0, validInteger: 1);
            GameManager game = new GameManager(testing: true, state, txtMgr);

            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            currentCity.ConnectedCities = new List<City>
            {
                new City("Miami", Colors.Yellow, state, txtMgr),
                new City("Washington", Colors.Blue, state, txtMgr),
                new City("Chicago", Colors.Blue, state, txtMgr)
            };

            User user = new User(0, "test user");
            user.CurrentRole = new Researcher(currentCity, 0, state, txtMgr);
            game.Users = new List<User> { user };

            Exception ex = Assert.Throws<TheWorldIsDeadException>(() => game.TestPlayGame());
            Assert.Equal("There are no more cards in the player deck. You were too slow and the world is now suffering for it", ex.Message);
        }
    }
}
