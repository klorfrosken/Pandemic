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
        Boolean HasPickedEvent = false;
        EventCard StoredCard;

        public ContingencyPlanner(City StartingCity, int PlayerID, StateManager state = null, TextManager textManager = null) : base (PlayerID, Title, StartingCity, state, textManager) 
        {
            SpecialActions = 1;
        }

        public override void PrintSpecialAbilities()
        {
            int i = TextManager.AvailableStandardActions;
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine($"{i+1}: TAKE EVENT CARD - Take an event card from anywhere in the Player Discard Pile and store it. Only one event card can be stored at a time.");
            Console.WriteLine("When the stored event card is played it is removed from the game instead of discarded.");
        }

        public override void PlayFirstSpecialAbility()
        {
            try
            {
                PickEventcardFromDiscard(State.PlayerDiscard);
            }
            catch { throw; }    //Throws illegalmoveexception if there are no event cards in the playerDiscard or haspickedevent
        }

        public void PickEventcardFromDiscard(PlayerDeck PlayerDiscard)
        {
            if (!HasPickedEvent)
            {            
                List<EventCard> EligibleCards = new List<EventCard>();
                foreach (Card  CurrentCard in PlayerDiscard)
                {
                    if (CurrentCard is EventCard)
                    {
                        EligibleCards.Add(CurrentCard as EventCard);
                    }
                }
                
                int Choice = -1;
                if (EligibleCards.Count == 0)
                {
                    throw new IllegalMoveException($"There are no Event cards in the discard pile for you to take");
                } else if (EligibleCards.Count == 1)
                {
                    Choice = 0;
                } else
                {
                    Choice = TextManager.ChooseItemFromList(EligibleCards, "take");
                }

                StoredCard = EligibleCards[Choice];
                HasPickedEvent = true;

            } else
            {
                throw new IllegalMoveException($"You already have {StoredCard.Name} stored. You need to use it before you can pick a new Event Card to store.");
            }
        }

        public void UseStoredCard()
        {
            if (StoredCard == null)
            {
                throw new IllegalMoveException("You don't have an Event Card stored to play");
            }
            else
            {
                StoredCard.Play(this); //Consider whether the Play method should put the card in the discard pile. If so, the contingency planner must remove it again. 
                StoredCard = null;
                HasPickedEvent = false;
            }
        }
    }
}
