using Xunit;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using System;

namespace Pandemic.UnitTests.Managers
{
    public class UserTests
    {
        [Fact]
        public void UpdateStatistics_invalidString_ThrowsException()
        {
            User player = new User(0, "testUser");

            Exception ex = Assert.Throws<UnexpectedBehaviourException>(() => player.UpdateStatistics("test"));
            Assert.Equal("An unexpected error occured during the updating of the statistics", ex.Message);
        }

        [Fact]
        public void UpdateStatistics_GameWon_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            User player = new User(0, "testUser");
            player.CurrentRole = new Medic(currentCity, player.UserID);

            player.UpdateStatistics("Won");

            Assert.Equal(1, player.RoleStatistics["Medic"].GamesWon);
            Assert.Equal(0, player.RoleStatistics["Medic"].GamesLost);
            Assert.Equal(0, player.RoleStatistics["Medic"].GamesNotCompleted);
            Assert.Equal(1, player.RoleStatistics["Medic"].GamesPlayed);
        }

        [Fact]
        public void UpdateStatistics_GameLost_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            User player = new User(0, "testUser");
            player.CurrentRole = new Medic(currentCity, player.UserID);

            player.UpdateStatistics("Lost");

            Assert.Equal(0, player.RoleStatistics["Medic"].GamesWon);
            Assert.Equal(1, player.RoleStatistics["Medic"].GamesLost);
            Assert.Equal(0, player.RoleStatistics["Medic"].GamesNotCompleted);
            Assert.Equal(1, player.RoleStatistics["Medic"].GamesPlayed);
        }

        [Fact]
        public void UpdateStatistics_GameError_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            User player = new User(0, "testUser");
            player.CurrentRole = new Medic(currentCity, player.UserID);

            player.UpdateStatistics("Error");

            Assert.Equal(0, player.RoleStatistics["Medic"].GamesWon);
            Assert.Equal(0, player.RoleStatistics["Medic"].GamesLost);
            Assert.Equal(1, player.RoleStatistics["Medic"].GamesNotCompleted);
            Assert.Equal(1, player.RoleStatistics["Medic"].GamesPlayed);
        }

    }
}
