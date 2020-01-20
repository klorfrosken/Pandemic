using System;
using System.Collections.Generic;
using Xunit;
using Pandemic.Cards;
using Pandemic.Managers;
using Pandemic.Exceptions;
using Pandemic.Game;
using Pandemic.UnitTests.TestClasses;

namespace Pandemic.UnitTests.Cards
{
    public class InfectionDeckTests
    {
        [Fact]
        public void InfectionDeck_Succeeds()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard,
            };

            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            Assert.Collection(actualDeck, 
                item => { Assert.Equal(bottomCard, item); },
                item => { Assert.Equal(middleCard, item); },
                item => { Assert.Equal(topCard, item); }
                );
        }

        [Fact]
        public void Draw_DrawOneCard_TopCardDrawn()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            InfectionCard drawnCard = actualDeck.Draw();

            Assert.Equal(topCard, drawnCard);
        }

        [Fact]
        public void Draw_NoCardsInDeck_ThrowsException()
        {
            InfectionDeck actualDeck = new InfectionDeck();

            Assert.Throws<UnexpectedBehaviourException>(() => actualDeck.Draw());
        }

        [Fact]
        public void Draw_DrawMultipleCards_TopCardsDrawn()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);
            InfectionCard undrawnCard = new InfectionCard("UndrawnCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard,
                undrawnCard
            };

            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            List<InfectionCard> drawnCards = actualDeck.Draw(3);

            Assert.Collection(drawnCards,
                item => { Assert.Equal(topCard, item); },
                item => { Assert.Equal(middleCard, item); },
                item => { Assert.Equal(bottomCard, item); }
                );
        }

        [Fact]
        public void Draw_DrawZeroCards_ThrowsException()
        {
            InfectionDeck actualDeck = new InfectionDeck();

            Assert.Throws<UnexpectedBehaviourException>(() => actualDeck.Draw(0));
        }

        [Fact]
        public void Shuffle_CardsAreShuffled()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);
            InfectionCard undrawnCard = new InfectionCard("UndrawnCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard,
                undrawnCard
            };

            InfectionDeck testDeck = new InfectionDeck(infectionDeckCards);
            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            testDeck.Shuffle();

            Assert.NotEqual(actualDeck, testDeck);
        }

        [Fact]
        public void Remove_specificCardEntered_cardIsRemoved()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            Boolean actual = actualDeck.Remove(middleCard);

            Assert.True(actual);
            Assert.DoesNotContain(middleCard, actualDeck);
            Assert.Contains(topCard, actualDeck);
            Assert.Contains(bottomCard, actualDeck);
        }

        [Fact]
        public void Remove_cardNotInDeck_ThrowsException()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
            };

            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            Boolean actual = actualDeck.Remove(bottomCard);

            Assert.False(actual);
        }

        [Fact]
        public void Remove_cardIndexSpecified_cardIsRemoved()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            actualDeck.RemoveAt(1);

            Assert.DoesNotContain(middleCard, actualDeck);
            Assert.Contains(topCard, actualDeck);
            Assert.Contains(bottomCard, actualDeck);
        }

        [Fact]
        public void Remove_specifiedIndexOutOfRange_throwsException()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            Assert.Throws<UnexpectedBehaviourException>(() => actualDeck.RemoveAt(3));
        }

        [Fact]
        public void Remove_noCardsInDeck_throwsException()
        {
            InfectionDeck actualDeck = new InfectionDeck();

            Assert.Throws<UnexpectedBehaviourException>(() => actualDeck.RemoveAt(1));
        }

        [Fact]
        public void Count_ReturnsCorrectNumberOfCards()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            int cardCount = actualDeck.Count();

            Assert.Equal(3, cardCount);
        }

        [Fact]
        public void AddCard_CardAdded()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
            };

            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            actualDeck.AddCard(bottomCard);

            Assert.Collection(actualDeck,
                item => { Assert.Equal(middleCard, item); },
                item => { Assert.Equal(topCard, item); },
                item => { Assert.Equal(bottomCard, item); }
                );
        }

        [Fact]
        public void AddCards_cardsAddedInCorrectOrder()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            InfectionCard firstCard = new InfectionCard("firstCard", Colors.Blue);
            InfectionCard secondCard = new InfectionCard("secondCard", Colors.Blue);
            InfectionCard thirdCard = new InfectionCard("thirdCard", Colors.Blue);

            List<InfectionCard> newCards = new List<InfectionCard>
            {
                firstCard,
                secondCard, 
                thirdCard
            };

            actualDeck.AddCards(newCards);

            Assert.Collection(actualDeck,
                item => { Assert.Equal(bottomCard, item); },
                item => { Assert.Equal(middleCard, item); },
                item => { Assert.Equal(topCard, item); },
                item => { Assert.Equal(thirdCard, item); },
                item => { Assert.Equal(secondCard, item); },
                item => { Assert.Equal(firstCard, item); }
                );
        }

        [Fact]
        public void InsertOnTop_Succeeds()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard
            };
            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            InfectionCard discard1 = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard discard2 = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard discard3 = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> discardPileCards = new List<InfectionCard>
            {
                discard1,
                discard2,
                discard3
            };
            InfectionDeck infectionDiscard = new InfectionDeck(discardPileCards);

            actualDeck.InsertOnTop(infectionDiscard);

            Assert.Collection(actualDeck,
                item => { Assert.Equal(bottomCard, item); },
                item => { Assert.Equal(middleCard, item); },
                item => { Assert.Equal(topCard, item); }, 
                item => { Assert.Equal(discard3, item); },
                item => { Assert.Equal(discard2, item); },
                item => { Assert.Equal(discard1, item); }
                );
            Assert.Empty(infectionDiscard);
        }

        [Fact]
        public void RemoveBottomCard_cardIsRemoved()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard
            };
            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            InfectionCard actualCard = actualDeck.RemoveBottomCard();

            Assert.Equal(bottomCard, actualCard);
            Assert.Collection(actualDeck,
                item => { Assert.Equal(middleCard, item); },
                item => { Assert.Equal(topCard, item); }
                );
        }

        [Fact]
        public void Clear_Succeeds()
        {
            InfectionCard topCard = new InfectionCard("TopCard", Colors.Blue);
            InfectionCard middleCard = new InfectionCard("MiddleCard", Colors.Blue);
            InfectionCard bottomCard = new InfectionCard("BottomCard", Colors.Blue);

            List<InfectionCard> infectionDeckCards = new List<InfectionCard>
            {
                topCard,
                middleCard,
                bottomCard
            };
            InfectionDeck actualDeck = new InfectionDeck(infectionDeckCards);

            actualDeck.Clear();

            Assert.Empty(actualDeck);
        }

        [Fact]
        public void Infect_CityIsInfected()
        {
            StateManager state = new StateManager(Testing: true);
            ITextManager textMgr = new TestTextManager();

            Colors currentColor = Colors.Blue;
            City cityToInfect = new City("Atlanta", currentColor, state, textMgr);
            InfectionCard actualCard = new InfectionCard("Atlanta", currentColor, state);
            state.Cities["Atlanta"] = cityToInfect;
            state.InfectionDeck.AddCard(actualCard);

            state.InfectionDeck.Infect();

            Assert.Equal(1, cityToInfect.DiseaseCubes[currentColor]);
        }
    }
}
