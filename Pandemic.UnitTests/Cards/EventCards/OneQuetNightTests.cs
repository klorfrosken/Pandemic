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
    public class OneQuetNightTests
    {
        [Fact]
        public void Play_CardNotInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            Scientist player = new Scientist(currentCity, 0);
            OneQuietNight card = new OneQuietNight();

            Exception ex = Assert.Throws<IllegalMoveException>(() => card.Play(player));
            Assert.Equal($"The {player.RoleName} does not have One Quiet Night in their hand to play.", ex.Message);
        }

        [Fact]
        public void Play_Succeeds_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            Scientist player = new Scientist(currentCity, 0);
            ITextManager txtMgr = new TestTextManager();
            OneQuietNight card = new OneQuietNight(textManager: txtMgr);
            player.Hand.Add(card);

            Exception ex = Assert.Throws<OneQuietNightException>(() => card.Play(player));
            Assert.Equal($"Thanks to the {player.RoleName} it was a calm and quiet night.", ex.Message);
        }
    }
}
