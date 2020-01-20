using System.Collections.Generic;
using Pandemic.Game;
using Pandemic.Cards;
using Pandemic.Cards.EventCards;
using Pandemic.Managers;

namespace Pandemic.UnitTests.TestClasses
{
    public class TestTextManager : ITextManager
    {
        //Print methods
        public void PrintBeginGame(List<User> Users) { }

        public void PrintStatus(StateManager state) { }

        public void PrintHand(Role currentRole) { }

        public void PrintEventDescription(EventCard card) { }

        public void PrintNewTurn(StateManager state) { }

        public void AvailableActions(Role currentRole) { }

        public void PrintPlayerMoved(Role movedRole, City newCity) { }

        public void PrintResearchStationBuilt(Role currentRole) { }

        public void PrintDiseaseTreated(Role currentRole, City treatedCity, Colors cubeColor, StateManager state) { }

        public void PrintCureDiscovered(Role currentRole) { }

        public void PrintKnowledgeShared(Role currentRole, Role otherPlayer) { }

        public void PrintUsedSpecialAbility(Role currentRole) { }

        public void PrintInfection(City currentCity, Colors cubeColor) { }

        int difficulty;
        int shareKnowledge;
        int itemNumber;
        int validInteger;
        int discard_PlayInteger;
        public int AvailableStandardActions { get; }

        public TestTextManager(int difficulty = -1, int shareKnowledge = -1, int itemNumber = -1, int validInteger = -1, int discard_PlayInteger = -1)
        {
            this.difficulty = difficulty;
            this.shareKnowledge = shareKnowledge;
            this.itemNumber = itemNumber;
            this.validInteger = validInteger;
            this.discard_PlayInteger = discard_PlayInteger;
            AvailableStandardActions = 8;
        }

        //Choice methods
        public int GetDifficulty()
        {
            return difficulty;
        }

        public int ShareKnowledgeWithResearcher()
        {
            return shareKnowledge;
        }

        public int ChooseItemFromList<T>(IEnumerable<T> ItemList, string PickOneOfTheFollowingTo)
        {
            return itemNumber;
        }

        public int GetValidInteger(int LowerRange, int UpperRange)
        {
            return validInteger;
        }

        public int DiscardOrPlay(List<PlayerCard> Hand) 
        {
            return discard_PlayInteger;
        }
    }
}