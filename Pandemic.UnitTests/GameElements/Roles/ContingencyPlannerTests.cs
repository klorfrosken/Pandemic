using Xunit;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;
using Pandemic.Cards;

namespace Pandemic.UnitTests.GameElements.Roles
{
    public class ContingencyPlannerTests
    {
        [Fact]
        public void PickEventCardFromDiscard_HasNotPickedEvent_EventAvailable_Succeeds()
        {
            StateManager state = new StateManager(
                Testing: true,
                PlayerDiscard: new PlayerDeck());
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            City currentCity = new City("Atlanta", Colors.Blue);
            ContingencyPlanner role = new ContingencyPlanner(currentCity, 0, state, txtMgr);

            TestEvent card = new TestEvent();
            state.PlayerDiscard.AddCard(card);

            role.PlayFirstSpecialAbility();

            Assert.True(role.hasPickedEvent);
            Assert.Equal(card, role.storedCard);
        }

        [Fact]
        public void PickEventCardFromDiscard_HasNotPickedEvent_NoEventAvailable_ThrowsException()
        {
            StateManager state = new StateManager(
                Testing: true,
                PlayerDiscard: new PlayerDeck());
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            City currentCity = new City("Atlanta", Colors.Blue);
            ContingencyPlanner role = new ContingencyPlanner(currentCity, 0, state, txtMgr);

            Assert.Throws<IllegalMoveException>(() => role.PlayFirstSpecialAbility());
        }

        [Fact]
        public void PickEventCardFromDiscard_HasAlreadyPickedEvent_ThrowsException()
        {
            StateManager state = new StateManager(
                Testing: true,
                PlayerDiscard: new PlayerDeck());
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            City currentCity = new City("Atlanta", Colors.Blue);
            ContingencyPlanner role = new ContingencyPlanner(currentCity, 0, state, txtMgr);

            TestEvent card = new TestEvent();
            state.PlayerDiscard.AddCard(card);

            role.PlayFirstSpecialAbility();

            Assert.Throws<IllegalMoveException>(() => role.PlayFirstSpecialAbility());
        }

        [Fact]
        public void UseStoredCard_NoCardStored_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            ContingencyPlanner role = new ContingencyPlanner(currentCity, 0);

            Assert.Throws<IllegalMoveException>(() => role.UseStoredCard());
        }

        [Fact]
        public void UseStoredCard_UseStoredCard_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            ContingencyPlanner role = new ContingencyPlanner(currentCity, 0);
            role.storedCard = new TestEvent();

            role.UseStoredCard();

            Assert.Null(role.storedCard);
            Assert.False(role.hasPickedEvent);
        }
    }
}
