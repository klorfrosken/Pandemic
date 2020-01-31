using Xunit;
using Pandemic.Managers;
using Pandemic.Cards.EventCards;
using Pandemic.Game;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;
using System;

namespace Pandemic.UnitTests.Cards.EventCards
{
    public class GovernmentGrantTests
    {
        [Fact]
        public void Play_CardNotInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            Scientist player = new Scientist(currentCity, 0);
            GovernmentGrant card = new GovernmentGrant();

            Exception ex = Assert.Throws<IllegalMoveException>(() => card.Play(player));
            Assert.Equal($"The {player.RoleName} does not have Government Grant in their hand to play.", ex.Message);
        }

        [Fact]
        public void Play_NoMoreResearchStations_ThrowsException()
        {
            StateManager state = new StateManager(testing: true, remainingResearchStations: 0);
            City currentCity = new City("Atlanta", Colors.Blue);
            Scientist player = new Scientist(currentCity, 0);
            GovernmentGrant card = new GovernmentGrant(state);
            player.Hand.Add(card);

            Exception ex = Assert.Throws<IllegalMoveException>(() => card.Play(player));
            Assert.Equal("There are no research stations left to build. You'll have to make do with the ones you have.", ex.Message);
        }

        [Fact]
        public void Play_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber:0);
            City currentCity = new City("Atlanta", Colors.Blue, state);
            state.Cities["Atlanta"] = currentCity;
            Scientist player = new Scientist(currentCity, 0);
            GovernmentGrant card = new GovernmentGrant(state, txtMgr);
            player.Hand.Add(card);

            card.Play(player);

            Assert.True(currentCity.HasResearchStation);
            Assert.Empty(player.Hand);
        }
    }
}