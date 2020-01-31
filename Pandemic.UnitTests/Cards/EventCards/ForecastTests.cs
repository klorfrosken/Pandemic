using Xunit;
using System.Collections.Generic;
using Pandemic.Managers;
using Pandemic.Cards;
using Pandemic.Cards.EventCards;
using Pandemic.Game;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;

namespace Pandemic.UnitTests.Cards.EventCards
{
    public class ForecastTests
    {
        [Fact]
        public void Play_CardNotInHand_ThrowsException()
        {
            City currentCity = new City("Atlanta", Colors.Blue);
            Scientist player = new Scientist(currentCity, 0);
            Forecast card = new Forecast();

            Assert.Throws<IllegalMoveException>(() => card.Play(player));
        }

        [Fact]
        public void Play_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1);
            Forecast eventCard = new Forecast(state, txtMgr);

            List<InfectionCard> cards = new List<InfectionCard>
            {
                new InfectionCard("card6", Colors.Blue, state),
                new InfectionCard("card5", Colors.Blue, state),
                new InfectionCard("card4", Colors.Blue, state),
                new InfectionCard("card3", Colors.Blue, state),
                new InfectionCard("card2", Colors.Blue, state),
                new InfectionCard("card1", Colors.Blue, state)
            };
            state.InfectionDeck.AddCards(new List<InfectionCard>(cards));

            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            Scientist player = new Scientist(currentCity, 0, state, txtMgr);
            player.Hand.Add(eventCard);

            eventCard.Play(player);

            Assert.Collection(state.InfectionDeck,
                 item => { Assert.Equal(cards[0], item); },
                 item => { Assert.Equal(cards[5], item); },
                 item => { Assert.Equal(cards[4], item); },
                 item => { Assert.Equal(cards[3], item); },
                 item => { Assert.Equal(cards[2], item); },
                 item => { Assert.Equal(cards[1], item); });
            Assert.Empty(player.Hand);
        }

        [Fact]
        public void Play_lessThanSixCardsInDeck_Succeeds()
        {
            StateManager state = new StateManager(testing: true);
            ITextManager txtMgr = new TestTextManager(itemNumber: 1);
            Forecast eventCard = new Forecast(state, txtMgr);

            List<InfectionCard> cards = new List<InfectionCard>
            {
                new InfectionCard("card4", Colors.Blue, state),
                new InfectionCard("card3", Colors.Blue, state),
                new InfectionCard("card2", Colors.Blue, state),
                new InfectionCard("card1", Colors.Blue, state)
            };
            state.InfectionDeck.AddCards(new List<InfectionCard>(cards));

            City currentCity = new City("Atlanta", Colors.Blue, state, txtMgr);
            Scientist player = new Scientist(currentCity, 0, state, txtMgr);
            player.Hand.Add(eventCard);

            eventCard.Play(player);

            Assert.Collection(state.InfectionDeck,
                 item => { Assert.Equal(cards[0], item); },
                 item => { Assert.Equal(cards[3], item); },
                 item => { Assert.Equal(cards[2], item); },
                 item => { Assert.Equal(cards[1], item); });
            Assert.Empty(player.Hand);
        }
    }
}
