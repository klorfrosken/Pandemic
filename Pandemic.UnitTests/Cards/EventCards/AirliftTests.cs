using Xunit;
using Pandemic.Managers;
using Pandemic.Cards.EventCards;
using Pandemic.Game;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;

namespace Pandemic.UnitTests.Cards.EventCards
{
    public class AirliftTests
    {
        [Fact]
        public void Play_CardNotInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            Airlift card = new Airlift();
            Scientist player = new Scientist(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => card.Play(player));
        }

        [Fact]
        public void Play_PlayerIsMoved_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            City currentCity = new City("Atlanta", Colors.Blue, state);
            City newCity = new City("Lima", Colors.Yellow, state);
            state.Cities["Atlanta"] = currentCity;
            state.Cities["Lima"] = newCity;

            Airlift card = new Airlift(state, txtMgr);
            Scientist player = new Scientist(currentCity, 0, state, txtMgr);
            state.Roles.Add(player);
            player.Hand.Add(card);

            card.Play(player);

            Assert.Equal(newCity, player.CurrentCity);
            Assert.Empty(player.Hand);
        }
    }
}
