using System;
using System.Collections.Generic;
using Xunit;
using Pandemic.Cards;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.UnitTests.Cards
{
    public class PlayerDeckTests
    {
        [Fact]
        public void Draw_DrawOneCard_TopCardDrawn()
        {
            PlayerCard topCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard middleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard bottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> playerDeckCards = new List<PlayerCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            PlayerDeck testDeck = new PlayerDeck(playerDeckCards);

            PlayerCard drawnCard = testDeck.Draw();

            Assert.Equal(topCard, drawnCard);
        }

        [Fact]
        public void Draw_NoCardsInDeck_ThrowsException()
        {
            PlayerDeck testDeck = new PlayerDeck();

            Assert.Throws<UnexpectedBehaviourException>(() => testDeck.Draw());
        }

        [Fact]
        public void Draw_DrawMultipleCards_TopCardsDrawn()
        {
            PlayerCard topCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard middleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard bottomCard = new PlayerCard("BottomCard", Colors.Blue);
            PlayerCard undrawnCard = new PlayerCard("UndrawnCard", Colors.Blue);

            List<PlayerCard> playerDeckCards = new List<PlayerCard>
            {
                topCard,
                middleCard,
                bottomCard, 
                undrawnCard
            };

            PlayerDeck testDeck = new PlayerDeck(playerDeckCards);

            List<PlayerCard> drawnCards = testDeck.Draw(3);

            Assert.Collection(drawnCards,
                item => { Assert.Equal(topCard, item); },
                item => { Assert.Equal(middleCard, item); },
                item => { Assert.Equal(bottomCard, item); }
                );
            Assert.Collection(testDeck,
                item => { Assert.Equal(undrawnCard, item); });
        }

        [Fact]
        public void Draw_DrawZeroCards_ThrowsException()
        {
            PlayerDeck testDeck = new PlayerDeck();

            Assert.Throws<UnexpectedBehaviourException>(() => testDeck.Draw(0));
        }

        [Fact]
        public void Shuffle_CardsAreShuffled()
        {
            //Note that this test might, in rare cases, fail due to the deck being reshuffled into the exact same order as it was before shuffling
            PlayerCard topCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard middleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard bottomCard = new PlayerCard("BottomCard", Colors.Blue);
            PlayerCard undrawnCard = new PlayerCard("UndrawnCard", Colors.Blue);

            List<PlayerCard> playerDeckCards = new List<PlayerCard>
            {
                topCard,
                middleCard,
                bottomCard,
                undrawnCard
            };

            PlayerDeck testDeck = new PlayerDeck(playerDeckCards);
            PlayerDeck actualDeck = new PlayerDeck(playerDeckCards);

            testDeck.Shuffle();

            Assert.NotEqual(actualDeck, testDeck);
        }

        [Fact]
        public void Remove_specificCardEntered_cardIsRemoved()
        {
            PlayerCard topCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard middleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard bottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> playerDeckCards = new List<PlayerCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            PlayerDeck testDeck = new PlayerDeck(playerDeckCards);

            Boolean actual = testDeck.Remove(middleCard);

            Assert.True(actual);
            Assert.DoesNotContain(middleCard, testDeck);
            Assert.Contains(topCard, testDeck);
            Assert.Contains(bottomCard, testDeck);
        }

        [Fact]
        public void Remove_cardNotInDeck_ThrowsException()
        {
            PlayerCard topCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard middleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard bottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> playerDeckCards = new List<PlayerCard>
            {
                topCard,
                middleCard,
            };

            PlayerDeck testDeck = new PlayerDeck(playerDeckCards);

            Boolean actual = testDeck.Remove(bottomCard);

            Assert.False(actual);
        }

        [Fact]
        public void Remove_cardIndexSpecified_cardIsRemoved()
        {
            PlayerCard topCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard middleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard bottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> playerDeckCards = new List<PlayerCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            PlayerDeck testDeck = new PlayerDeck(playerDeckCards);

            testDeck.RemoveAt(1);

            Assert.DoesNotContain(middleCard, testDeck);
            Assert.Contains(topCard, testDeck);
            Assert.Contains(bottomCard, testDeck);
        }

        [Fact]
        public void Remove_specifiedIndexOutOfRange_throwsException()
        {
            PlayerCard topCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard middleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard bottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> playerDeckCards = new List<PlayerCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            PlayerDeck testDeck = new PlayerDeck(playerDeckCards);

            Assert.Throws<UnexpectedBehaviourException>(() => testDeck.RemoveAt(3));
        }

        [Fact]
        public void Remove_noCardsInDeck_throwsException()
        {
            PlayerDeck testDeck = new PlayerDeck();

            Assert.Throws<UnexpectedBehaviourException>(() => testDeck.RemoveAt(1));
        }

        [Fact]
        public void Count_ReturnsCorrectNumberOfCards()
        {
            PlayerCard topCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard middleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard bottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> playerDeckCards = new List<PlayerCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            PlayerDeck testDeck = new PlayerDeck(playerDeckCards);

            int actual = testDeck.Count();

            Assert.Equal(3, actual);
        }

        [Fact]
        public void AddCard_singleCardAdded_succeeds()
        {
            PlayerDeck testDeck = new PlayerDeck();
            PlayerCard newCard = new PlayerCard("testCard", Colors.Blue);

            testDeck.AddCard(newCard);

            Assert.Contains(newCard, testDeck);
        }

        [Fact]
        public void AddCard_cardIsNull_Fails()
        {
            PlayerDeck testDeck = new PlayerDeck();
            PlayerCard newCard = null;

            Assert.Throws<UnexpectedBehaviourException>(() => testDeck.AddCard(newCard));
        }

        [Fact]
        public void AddCards_ListOfCardsAdded_Succeeds()
        {
            PlayerCard topCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard middleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard bottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> playerDeckCards = new List<PlayerCard>
            {
                topCard,
                middleCard,
                bottomCard
            };

            PlayerDeck testDeck = new PlayerDeck();
            testDeck.AddCards(playerDeckCards);

            Assert.Collection(playerDeckCards,
                item => { Assert.Equal(topCard, item); },
                item => { Assert.Equal(middleCard, item); },
                item => { Assert.Equal(bottomCard, item); }
                );
        }

        [Fact]
        public void AddCards_CardsIsNull_ThrowsException()
        {
            List<PlayerCard> playerDeckCards = null;

            PlayerDeck testDeck = new PlayerDeck();

            Assert.Throws<UnexpectedBehaviourException>(() => testDeck.AddCards(playerDeckCards));
        }

        [Fact]
        public void AddCards_NoCardsInList_ThrowsException()
        {
            List<PlayerCard> playerDeckCards = new List<PlayerCard>();

            PlayerDeck testDeck = new PlayerDeck();

            Assert.Throws<UnexpectedBehaviourException>(() => testDeck.AddCards(playerDeckCards));
        }

        [Fact]
        public void CombineDecks_DecksCombined_Succeeds()
        {
            PlayerCard firstTopCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard firstMiddleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard firstBottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> firstPile = new List<PlayerCard>
            {
                firstTopCard,
                firstMiddleCard,
                firstBottomCard
            };

            PlayerDeck firstDeck = new PlayerDeck(firstPile);

            PlayerCard secondTopCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard secondMiddleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard secondBottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> secondPile = new List<PlayerCard>
            {
                secondTopCard,
                secondMiddleCard,
                secondBottomCard
            };

            PlayerDeck secondDeck = new PlayerDeck(secondPile);

            firstDeck.CombineDecks(secondDeck);

            Assert.Collection(firstDeck,
                item => { Assert.Equal(secondBottomCard, item); },
                item => { Assert.Equal(secondMiddleCard, item); },
                item => { Assert.Equal(secondTopCard, item); },
                item => { Assert.Equal(firstBottomCard, item); },
                item => { Assert.Equal(firstMiddleCard, item); },
                item => { Assert.Equal(firstTopCard, item); }
                );
        }

        [Fact]
        public void CombineDecks_DecksIsEmpty_Succeeds()
        {
            PlayerCard firstTopCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard firstMiddleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard firstBottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> firstPile = new List<PlayerCard>
            {
                firstTopCard,
                firstMiddleCard,
                firstBottomCard
            };

            PlayerDeck firstDeck = new PlayerDeck(firstPile);

            PlayerDeck secondDeck = new PlayerDeck();

            Assert.Throws<UnexpectedBehaviourException>(() => firstDeck.CombineDecks(secondDeck));
        }

        [Fact]
        public void Clear_DeckEmptied()
        {
            PlayerCard firstTopCard = new PlayerCard("TopCard", Colors.Blue);
            PlayerCard firstMiddleCard = new PlayerCard("MiddleCard", Colors.Blue);
            PlayerCard firstBottomCard = new PlayerCard("BottomCard", Colors.Blue);

            List<PlayerCard> firstPile = new List<PlayerCard>
            {
                firstTopCard,
                firstMiddleCard,
                firstBottomCard
            };

            PlayerDeck firstDeck = new PlayerDeck(firstPile);

            firstDeck.Clear();

            Assert.Empty(firstDeck);
        }
    }
}
