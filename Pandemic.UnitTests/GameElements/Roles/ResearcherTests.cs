using Xunit;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;
using Pandemic.Cards;

namespace Pandemic.UnitTests.GameElements.Roles
{
    public class ResearcherTests
    {
        [Fact]
        public void GiveCard_cardIsGiven_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            Role player = new Researcher(currentCity, 0, null, txtMgr);
            Role otherPlayer = new QuarantineSpecialist(currentCity, 1);
            PlayerCard card = new CityCard("card", Colors.Blue);
            player.Hand.Add(card);

            player.GiveCard(otherPlayer);

            Assert.Empty(player.Hand);
            Assert.Single(otherPlayer.Hand);
            Assert.Contains(card, otherPlayer.Hand);
        }

        [Fact]
        public void GiveCard_cardIsNull_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            ITextManager txtMgr = new TestTextManager(itemNumber: 0);
            Role player = new Researcher(currentCity, 0, null, txtMgr);
            Role otherPlayer = new QuarantineSpecialist(currentCity, 1);
            PlayerCard card = null;
            player.Hand.Add(card);

            Assert.Throws<UnexpectedBehaviourException>(() => player.GiveCard(otherPlayer));
        }

        [Fact]
        public void ShareKnowledge_playersNotInSameCity_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            City otherCity = new City("Miami", Colors.Blue);

            Role player = new Researcher(currentCity, 0);
            Role otherPlayer = new QuarantineSpecialist(otherCity, 1);

            Assert.Throws<IllegalMoveException>(() => player.ShareKnowledge(otherPlayer));
        }

        [Fact]
        public void ShareKnowledge_playersInSameCity_otherPlayerGivesCard_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            ITextManager txtMgr = new TestTextManager(shareKnowledge: 1);
            Role player = new Researcher(currentCity, 0, null, txtMgr);
            Role otherPlayer = new QuarantineSpecialist(currentCity, 1);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);
            otherPlayer.Hand.Add(card);

            player.ShareKnowledge(otherPlayer);

            Assert.Empty(otherPlayer.Hand);
            Assert.Single(player.Hand);
            Assert.Contains(card, player.Hand);
            Assert.Equal(3, player.RemainingActions);
        }

        [Fact]
        public void ShareKnowledge_playersInSameCity_playerGivesCard_Succeeds()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            ITextManager txtMgr = new TestTextManager(shareKnowledge: 2, itemNumber: 0);
            Role player = new Researcher(currentCity, 0, null, txtMgr);
            Role otherPlayer = new QuarantineSpecialist(currentCity, 1);
            PlayerCard card = new CityCard("Atlanta", Colors.Blue);
            player.Hand.Add(card);

            player.ShareKnowledge(otherPlayer);

            Assert.Empty(player.Hand);
            Assert.Single(otherPlayer.Hand);
            Assert.Contains(card, otherPlayer.Hand);
            Assert.Equal(3, player.RemainingActions);
        }

        [Fact]
        public void ShareKnowledge_playersInSameCity_choiceError_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            ITextManager txtMgr = new TestTextManager(shareKnowledge: -1);
            Role player = new Researcher(currentCity, 0, null, txtMgr);
            Role otherPlayer = new QuarantineSpecialist(currentCity, 1);

            Assert.Throws<UnexpectedBehaviourException>(() => player.ShareKnowledge(otherPlayer));
        }
    }
}
