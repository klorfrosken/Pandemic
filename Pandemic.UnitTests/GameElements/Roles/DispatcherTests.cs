using Xunit;
using Pandemic.Managers;
using Pandemic.Cards;
using Pandemic.Cards.EventCards;
using Pandemic.Game;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;
using System;
using System.Collections.Generic;

namespace Pandemic.UnitTests.GameElements.Roles
{
    public class DispatcherTests
    {
        [Fact]
        public void ConnectPawns_PlayersInSameCity_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            Scientist targetPlayer = new Scientist(currentCity, 1);
            Researcher otherPlayer = new Researcher(currentCity, 2);

            Exception ex = Assert.Throws<IllegalMoveException>(() => dispatcher.TestConnectPawns(otherPlayer, targetPlayer));
            Assert.Equal($"The {otherPlayer.RoleName} is already in the same city as the {targetPlayer.RoleName}", ex.Message);
        }

        [Fact]
        public void ConnectPawns_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City otherCity = new City("Lima", Colors.Yellow);
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            Scientist targetPlayer = new Scientist(otherCity, 1);
            Researcher otherPlayer = new Researcher(currentCity, 2);

            dispatcher.TestConnectPawns(otherPlayer, targetPlayer);

            Assert.Equal(targetPlayer.CurrentCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
        }

        [Fact]
        public void DriveFerryForPlayer_CitiesNotConnected_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            Researcher otherPlayer = new Researcher(currentCity, 1);

            Exception ex = Assert.Throws<IllegalMoveException>(() => dispatcher.TestDriveFerryForPlayer(nextCity, otherPlayer));
            Assert.Equal($"{nextCity.Name} is not connected to {otherPlayer.CurrentCity} and they cannot be moved there.", ex.Message);
        }

        [Fact]
        public void DriveFerryForPlayer_CitiesConnected_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Chicago", Colors.Blue);
            currentCity.ConnectedCities = new List<City>
            {
                nextCity,
                new City("Miami", Colors.Yellow),
                new City("Washington", Colors.Blue)
            };
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            Researcher otherPlayer = new Researcher(currentCity, 1);

            dispatcher.TestDriveFerryForPlayer(nextCity, otherPlayer);

            Assert.Equal(nextCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
        }

        [Fact]
        public void DirectFlightForPlayer_CardNotInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            Researcher otherPlayer = new Researcher(currentCity, 1);

            Exception ex = Assert.Throws<IllegalMoveException>(() => dispatcher.TestDirectFlightForPlayer(nextCity, otherPlayer));
            Assert.Equal($"You need to have the {nextCity} City Card in _your_ hand in order to move another player to {nextCity.Name}", ex.Message);
        }

        [Fact]
        public void DirectFlightForPlayer_CardInHand_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            CityCard card = new CityCard("Lima", Colors.Yellow);
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            dispatcher.Hand.Add(card);
            Researcher otherPlayer = new Researcher(currentCity, 1);

            dispatcher.TestDirectFlightForPlayer(nextCity, otherPlayer);

            Assert.Equal(nextCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
            Assert.Empty(dispatcher.Hand);
        }

        [Fact]
        public void CharterFlightForPlayer_CardNotInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            Researcher otherPlayer = new Researcher(currentCity, 1);

            Exception ex = Assert.Throws<IllegalMoveException>(() => dispatcher.TestCharterFlightForPlayer(nextCity, otherPlayer));
            Assert.Equal($"You need to have the {otherPlayer.CurrentCity} City Card in _your_ hand in order to charter a flight for another player", ex.Message);
        }

        [Fact]
        public void CharterFlightForPlayer_CardInHand_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            CityCard card = new CityCard("Atlanta", Colors.Blue);
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            dispatcher.Hand.Add(card);
            Researcher otherPlayer = new Researcher(currentCity, 1);

            dispatcher.TestCharterFlightForPlayer(nextCity, otherPlayer);

            Assert.Equal(nextCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
            Assert.Empty(dispatcher.Hand);
        }

        [Fact]
        public void ShuttleFlightForPlayer_ResearchStationNotInFirstCity_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            nextCity.HasResearchStation = true;
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            Researcher otherPlayer = new Researcher(currentCity, 1);

            Exception ex = Assert.Throws<IllegalMoveException>(() => dispatcher.TestShuttleFlightForPlayer(nextCity, otherPlayer));
            Assert.Equal($"There needs to be a research station in both the city someone is moving from and the city they are moving to, for you to charter a flight for another player. \n{otherPlayer.CurrentCity}, where {otherPlayer} is, doesn't have one", ex.Message);
        }

        [Fact]
        public void ShuttleFlightForPlayer_ResearchStationNotInSecondCity_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            currentCity.HasResearchStation = true;
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            Researcher otherPlayer = new Researcher(currentCity, 1);

            Exception ex = Assert.Throws<IllegalMoveException>(() => dispatcher.TestShuttleFlightForPlayer(nextCity, otherPlayer));
            Assert.Equal($"There needs to be a research station in both the city someone is moving from and the city they are moving to, for you to charter a flight for another player. \n{nextCity}, where {otherPlayer} is to go, doesn't have one", ex.Message);
        }

        [Fact]
        public void ShuttleFlightForPlayer_ResearchStationsPresent_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            currentCity.HasResearchStation = true;
            nextCity.HasResearchStation = true;
            Dispatcher dispatcher = new Dispatcher(currentCity, 0);
            Researcher otherPlayer = new Researcher(currentCity, 1);

            dispatcher.TestShuttleFlightForPlayer(nextCity, otherPlayer);

            Assert.Equal(nextCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
        }

        [Fact]
        public void PlayFirstSpecialAbility_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1);
            City currentCity = new City("Atlanta", Colors.Blue);
            City otherCity = new City("Lima", Colors.Yellow);
            Dispatcher dispatcher = new Dispatcher(currentCity, 0, state, txtMgr);
            Scientist targetPlayer = new Scientist(otherCity, 1);
            Researcher otherPlayer = new Researcher(currentCity, 2);
            state.Roles = new List<Role>
            {
                dispatcher,
                otherPlayer,
                targetPlayer
            };

            dispatcher.PlayFirstSpecialAbility();

            Assert.Equal(targetPlayer.CurrentCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
            Assert.Equal(3, state.Roles.Count);
        }

        [Fact]
        public void PlaySecondSpecialAbility_DriveFerryForPlayer_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1, validInteger: 1);
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Chicago", Colors.Blue);
            currentCity.ConnectedCities = new List<City>
            {
                new City("Miami", Colors.Yellow),
                nextCity,
                new City("Washington", Colors.Blue)
            };
            Dispatcher dispatcher = new Dispatcher(currentCity, 0, state, txtMgr);
            Researcher otherPlayer = new Researcher(currentCity, 1);
            state.Roles = new List<Role>
            {
                dispatcher,
                otherPlayer,
            };

            dispatcher.PlaySecondSpecialAbility();

            Assert.Equal(nextCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
            Assert.Equal(2, state.Roles.Count);
        }

        [Fact]
        public void PlaySecondAbility_DirectFlightForPlayer_NoCardsInHand_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(validInteger: 2);

            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);

            Dispatcher dispatcher = new Dispatcher(currentCity, 0, state, txtMgr);
            Researcher otherPlayer = new Researcher(currentCity, 1);

            Exception ex = Assert.Throws<IllegalMoveException>(() => dispatcher.PlaySecondSpecialAbility());
            Assert.Equal("You don't have any cards you can discard for a direct flight", ex.Message);
        }

        [Fact]
        public void PlaySecondAbility_DirectFlightForPlayer_SingleCardInHand_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1, validInteger: 2);

            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            state.Cities["Lima"] = nextCity;

            Researcher otherPlayer = new Researcher(currentCity, 1);
            Dispatcher dispatcher = new Dispatcher(currentCity, 0, state, txtMgr);
            state.Roles = new List<Role>
            {
                dispatcher,
                otherPlayer
            };

            CityCard card = new CityCard("Lima", Colors.Yellow);
            dispatcher.Hand.Add(card);

            dispatcher.PlaySecondSpecialAbility();

            Assert.Equal(nextCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
            Assert.Empty(dispatcher.Hand);
        }

        [Fact]
        public void PlaySecondAbility_DirectFlightForPlayer_MultipleCardsInHand_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1, validInteger: 2);

            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            state.Cities["Lima"] = nextCity;

            Dispatcher dispatcher = new Dispatcher(currentCity, 0, state, txtMgr);
            Researcher otherPlayer = new Researcher(currentCity, 1);
            state.Roles = new List<Role>
            {
                dispatcher,
                otherPlayer
            };

            CityCard card1 = new CityCard("Atlanta", Colors.Blue);
            EventCard card2 = new TestEvent();
            CityCard card3 = new CityCard("Lima", Colors.Yellow);
            CityCard card4 = new CityCard("Paris", Colors.Blue);
            dispatcher.Hand = new List<PlayerCard>
            {
                card1,
                card2,
                card3,
                card4
            };

            dispatcher.PlaySecondSpecialAbility();

            Assert.Equal(nextCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
            Assert.DoesNotContain(card3, dispatcher.Hand);
            Assert.Equal(3, dispatcher.Hand.Count);
        }

        [Fact]
        public void PlaySecondAbility_CharterFlightForPlayer_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1, validInteger: 3);

            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            state.Cities["Atlanta"] = currentCity;
            state.Cities["Lima"] = nextCity;

            Dispatcher dispatcher = new Dispatcher(currentCity, 0, state, txtMgr);
            Researcher otherPlayer = new Researcher(currentCity, 1);
            state.Roles = new List<Role>
            {
                dispatcher,
                otherPlayer
            };

            CityCard card = new CityCard("Atlanta", Colors.Blue);
            dispatcher.Hand.Add(card);

            dispatcher.PlaySecondSpecialAbility();

            Assert.Equal(nextCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
            Assert.Empty(dispatcher.Hand);
        }

        [Fact]
        public void PlaySecondAbility_ShuttleFlightForPlayer_NoPlayersInCitiyWithResearchStation_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1, validInteger: 4);

            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            state.Cities["Atlanta"] = currentCity;
            state.Cities["Lima"] = nextCity;
            nextCity.HasResearchStation = true;

            Dispatcher dispatcher = new Dispatcher(currentCity, 0, state, txtMgr);
            Researcher otherPlayer = new Researcher(currentCity, 1);
            state.Roles = new List<Role>
            {
                dispatcher,
                otherPlayer
            };

            Exception ex = Assert.Throws<IllegalMoveException>(() => dispatcher.PlaySecondSpecialAbility());
            Assert.Equal("No players have research stations in the cities they are in, so they cannot be shuttled anywhere", ex.Message);
        }

        [Fact]
        public void PlaySecondAbility_ShuttleFlightForPlayer_OnlyOneResearchStation_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1, validInteger: 4);

            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            state.Cities["Atlanta"] = currentCity;
            state.Cities["Lima"] = nextCity;
            currentCity.HasResearchStation = true;

            Dispatcher dispatcher = new Dispatcher(currentCity, 0, state, txtMgr);
            Researcher otherPlayer = new Researcher(currentCity, 1);
            state.Roles = new List<Role>
            {
                dispatcher,
                otherPlayer
            };

            Exception ex = Assert.Throws<IllegalMoveException>(() => dispatcher.PlaySecondSpecialAbility());
            Assert.Equal("There needs to be a research station in both the city someone is moving from and the city they are moving to, for you to shuttle a flight. \nCurrently there's only one research station in the game...", ex.Message);
        }

        [Fact]
        public void PlaySecondAbility_ShuttleFlightForPlayer_OnlyOneOtherResearchStationOnlyOneEligiblePlayer_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(validInteger: 4);

            City dispatcherCity = new City("Washington", Colors.Blue);
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            state.Cities["Atlanta"] = currentCity;
            state.Cities["Lima"] = nextCity;
            currentCity.HasResearchStation = true;
            nextCity.HasResearchStation = true;

            Dispatcher dispatcher = new Dispatcher(dispatcherCity, 0, state, txtMgr);
            Researcher otherPlayer = new Researcher(currentCity, 1);
            state.Roles = new List<Role>
            {
                dispatcher,
                otherPlayer
            };

            dispatcher.PlaySecondSpecialAbility();

            Assert.Equal(nextCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
        }

        [Fact]
        public void PlaySecondAbility_ShuttleFlightForPlayer_MultipleOptions_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1, validInteger: 4);

            City dispatcherCity = new City("Washington", Colors.Blue);
            City currentCity = new City("Atlanta", Colors.Blue);
            City nextCity = new City("Lima", Colors.Yellow);
            state.Cities["Washington"] = dispatcherCity;
            state.Cities["Atlanta"] = currentCity;
            state.Cities["Lima"] = nextCity;
            dispatcherCity.HasResearchStation = true;
            currentCity.HasResearchStation = true;
            nextCity.HasResearchStation = true;

            Dispatcher dispatcher = new Dispatcher(dispatcherCity, 0, state, txtMgr);
            Researcher otherPlayer = new Researcher(currentCity, 1);
            state.Roles = new List<Role>
            {
                dispatcher,
                otherPlayer
            };

            dispatcher.PlaySecondSpecialAbility();

            Assert.Equal(nextCity, otherPlayer.CurrentCity);
            Assert.Equal(3, dispatcher.RemainingActions);
        }

        [Fact]
        public void PlaySecondAbility_InvalidSwitchInput_ThrowsException()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(validInteger: 5);

            City dispatcherCity = new City("Washington", Colors.Blue);
            City currentCity = new City("Atlanta", Colors.Blue);

            Dispatcher dispatcher = new Dispatcher(dispatcherCity, 0, state, txtMgr);

            Exception ex = Assert.Throws<UnexpectedBehaviourException>(() => dispatcher.PlaySecondSpecialAbility());
            Assert.Equal($"The program crashed unexpectedly due to an invalid argument in the PlaySecondSpecialAbility method of Dispatcher. \nThe switch received an invalid case", ex.Message);
        }
    }
}
