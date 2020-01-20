using System.Collections.Generic;
using Pandemic.Game;
using Pandemic.Cards;
using Pandemic.Cards.EventCards;

namespace Pandemic.Managers
{
    public interface ITextManager
    {
        int AvailableStandardActions { get; }

        //Print methods
        void PrintBeginGame(List<User> Users);

        void PrintStatus(StateManager state);

        void PrintHand(Role currentRole);

        void PrintEventDescription(EventCard card);

        void PrintNewTurn(StateManager state);

        void AvailableActions(Role currentRole);

        void PrintPlayerMoved(Role movedRole, City newCity);

        void PrintResearchStationBuilt(Role currentRole);

        void PrintDiseaseTreated(Role currentRole, City treatedCity, Colors cubeColor, StateManager state);

        void PrintCureDiscovered(Role currentRole);

        void PrintKnowledgeShared(Role currentRole, Role otherPlayer);

        void PrintUsedSpecialAbility(Role currentRole);

        void PrintInfection(City currentCity, Colors cubeColor);

        //Choice methods
        int GetDifficulty();
        
        int ShareKnowledgeWithResearcher();

        int ChooseItemFromList<T>(IEnumerable<T> ItemList, string PickOneOfTheFollowingTo);

        int GetValidInteger(int LowerRange, int UpperRange);

        int DiscardOrPlay(List<PlayerCard> Hand);
    }
}