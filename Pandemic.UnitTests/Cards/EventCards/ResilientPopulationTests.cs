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

namespace Pandemic.UnitTests.Cards.EventCards
{
    public class ResilientPopulationTests
    {
        [Fact]
        public void Play_CardNotInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            Scientist player = new Scientist(currentCity, 0);
            ResilientPopulation card = new ResilientPopulation();

            Exception ex = Assert.Throws<IllegalMoveException>(() => card.Play(player));
            Assert.Equal($"The {player.RoleName} does not have Resilient Population in their hand to play.", ex.Message);
        }

        [Fact]
        public void Play_NoCardsInDiscard_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            Scientist player = new Scientist(currentCity, 0);
            StateManager state = new StateManager(testing: true);
            ResilientPopulation card = new ResilientPopulation(state);
            player.Hand.Add(card);

            Exception ex = Assert.Throws<IllegalMoveException>(() => card.Play(player));
            Assert.Equal("There are no cards in the discard pile for the infection deck for you to use Resilient Population on", ex.Message);
        }

        [Fact]
        public void Play_Succeeds()
        {
            ITextManager txtMgr = new TestTextManager(itemNumber: 1);
            StateManager state = new StateManager(testing: true);
            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            Scientist player = new Scientist(currentCity, 0, state, txtMgr);
            ResilientPopulation card = new ResilientPopulation(state, txtMgr);
            player.Hand.Add(card);

            List<InfectionCard> cards = new List<InfectionCard>
            {
                new InfectionCard("card1", Colors.Blue, state),
                new InfectionCard("card2", Colors.Blue, state),
                new InfectionCard("card3", Colors.Blue, state)
            };
            state.InfectionDiscard.AddCards(new List<InfectionCard>(cards));

            card.Play(player);

            Assert.Empty(player.Hand);
            Assert.Collection(state.InfectionDiscard,
                item => { Assert.Equal(cards[2], item); },
                item => { Assert.Equal(cards[0], item); });
        }
    }
}
