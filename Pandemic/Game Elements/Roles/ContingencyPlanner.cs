using System;
using System.Collections.Generic;
using Pandemic.Cards;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;
using Pandemic.Cards.EventCards;

namespace Pandemic.Game_Elements.Roles
{
    public class ContingencyPlanner : Role
    {
        readonly static string Title = "Contingency Planner";
        public Boolean hasPickedEvent = false;
        public EventCard storedCard;

        public ContingencyPlanner(City StartingCity, int PlayerID, StateManager state = null, ITextManager textManager = null) : base (PlayerID, Title, StartingCity, state, textManager) 
        {
            SpecialActions = 1;
        }

        public override void PrintSpecialAbilities()
        {
            int i = textManager.AvailableStandardActions;
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine($"{i+1}: TAKE EVENT CARD - Take an event card from anywhere in the Player Discard Pile and store it. Only one event card can be stored at a time.");
            Console.WriteLine("When the stored event card is played it is removed from the game instead of discarded.");
        }

        public override void PlayFirstSpecialAbility()
        {
            try
            {
                PickEventcardFromDiscard(state.PlayerDiscard);
            }
            catch { throw; }    //Throws illegalmoveexception if there are no event cards in the playerDiscard or haspickedevent
        }

        void PickEventcardFromDiscard(PlayerDeck PlayerDiscard)
        {
            if (!hasPickedEvent)
            {            
                List<EventCard> eligibleCards = new List<EventCard>();
                foreach (Card  currentCard in PlayerDiscard)
                {
                    if (currentCard is EventCard)
                    {
                        eligibleCards.Add(currentCard as EventCard);
                    }
                }
                
                int choice = -1;
                if (eligibleCards.Count == 0)
                {
                    throw new IllegalMoveException($"There are no Event cards in the discard pile for you to take");
                } else if (eligibleCards.Count == 1)
                {
                    choice = 0;
                } else
                {
                    choice = textManager.ChooseItemFromList(eligibleCards, "take");
                }

                storedCard = eligibleCards[choice];
                hasPickedEvent = true;

            } else
            {
                throw new IllegalMoveException($"You already have {storedCard.Name} stored. You need to use it before you can pick a new Event Card to store.");
            }
        }

        public void UseStoredCard()
        {
            if (storedCard == null)
            {
                throw new IllegalMoveException("You don't have an Event Card stored to play");
            }
            else
            {
                storedCard.Play(this); 
                storedCard = null; //Note that the event is now removed from the game
                hasPickedEvent = false;
            }
        }
    }
}
