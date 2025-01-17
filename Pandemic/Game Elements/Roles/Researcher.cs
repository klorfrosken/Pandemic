﻿using System;
using System.Collections.Generic;
using Pandemic.Game;
using Pandemic.Cards;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Game_Elements.Roles
{
    public class Researcher : Role
    {
        readonly static string Title = "Researcher";

        public Researcher(City StartingCity, int PlayerID) : base(PlayerID, Title, StartingCity) { }

        public override void PrintSpecialAbilities()
        {
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine("The researcher may give _any_ City card from their hand to another player in the same city as them.");
            Console.WriteLine("The transfer _must_ be from the researcher's hand, but can occur on either player's turn.");
        }

        public override void ShareKnowledge(Role OtherPlayer, StateManager state)
        {
            //Dette sjekkes jo når jeg tar ibruk funksjonen, men kanskje greit å ha en ekstra sjekk allikevel?
            if (OtherPlayer.CurrentCity != CurrentCity)
            {
                throw new IllegalMoveException($"You need to be in the same city in order to share knowledge. {OtherPlayer.RoleName} is not in {CurrentCity.Name}.");
            }

            Role GivingPlayer;
            Role ReceivingPlayer;
            int Choice = TextManager.ShareKnowledgeWithScientist();
            if (Choice == 1)
            {
                GivingPlayer = OtherPlayer;
                ReceivingPlayer = this;
            }
            else if (Choice == 2)
            {
                GivingPlayer = this;
                ReceivingPlayer = OtherPlayer;
            }
            else
            {
                throw new UnexpectedBehaviourException("An error occured in ShareKnowledge while deciding who should be receiving a card");
            }
           
            GivingPlayer.GiveCard(ReceivingPlayer, state);
            RemainingActions--;
        }

        public override void GiveCard(Role OtherPlayer, StateManager state)
        {
            List<Card> EligibleCards = Hand.FindAll(Card => Card is CityCard);
            int Choice = TextManager.ChooseItemFromList(EligibleCards, "give");
            Card CardToGive = Hand[Choice];
            if (CardToGive == null)
            {
                throw new UnexpectedBehaviourException("An unexpected error occured in GiveCard in the Player class. The City card was not found in Hand");
            }

            Hand.Remove(CardToGive);
            OtherPlayer.ReceiveCard(CardToGive, state);
        }
    }
}
