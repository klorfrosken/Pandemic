using System;
using System.Collections.Generic;
using Pandemic.Cards;
using Pandemic.Managers;
using Pandemic.Exceptions;
using Pandemic.Cards.EventCards;
using Pandemic.Game_Elements.Roles;

namespace Pandemic.Game
{
    public abstract class Role
    {
        public int PlayerID { get; private set; } 
        public string RoleName { get; private set; }
        public int MaxActions = 4;
        public int SpecialActions = 0;
        protected int CardsNecessaryForCure = 5;
        public int RemainingActions { get; private protected set; } 
        public City CurrentCity { get; private protected set; }
        public List<Card> Hand = new List<Card>();
        protected internal StateManager _state;

        public Role (int PlayerID, String RoleName, City StartingCity, StateManager state)
        {
            this.PlayerID = PlayerID;
            this.RoleName = RoleName;
            CurrentCity = StartingCity;
            RemainingActions = MaxActions;
            _state = state;
        }

        public void Draw()
        {
            Card drawnCard = _state.PlayerDeck.Draw();

            if (drawnCard is EpidemicCard)
            {
                drawnCard.Play(this);
            } else
            {
                Hand.Add(drawnCard);
            }
        }

        public void Draw(Deck Deck, int NumberOfCards)
        {
            Hand.AddRange(Deck.Draw(NumberOfCards));
        }

        public virtual void ChangeCity(City NextCity)
        {
            CurrentCity = NextCity;
        }

        public void DriveFerry(City NextCity)
        {
            if (CurrentCity.IsConnectedTo(NextCity)){
                CurrentCity = NextCity;
                RemainingActions--;
            } else
            {
                throw new IllegalMoveException($"{NextCity.Name} is not connected to {CurrentCity.Name} and you cannot move there.");
            }
        }

        public void DirectFlight(City NextCity)
        {
            if (CardInHand(NextCity.Name))
            {
                ChangeCity(NextCity);
                Discard(NextCity.Name);
                RemainingActions--;
            } else {
                throw new IllegalMoveException($"You need to have the City Card in your hand in order to use a direct flight to a city. You do not have {NextCity.Name} in your hand.");
            }
        }

        public void CharterFlight(City NextCity)
        {
            if (CardInHand(CurrentCity.Name))
            {
                Discard(CurrentCity.Name);
                ChangeCity(NextCity);
                RemainingActions--;
            } else 
            {
                throw new IllegalMoveException($"You need to have the City Card for your current city, {CurrentCity.Name} in your hand in order to charter a flight from there.");
            }
        }

        public void ShuttleFlight(City NextCity)
        {
            if (CurrentCity.ResearchStation && NextCity.ResearchStation)
            {
                ChangeCity(NextCity);
                RemainingActions--;
            } else
            {
                throw new IllegalMoveException("There needs to be a research station in both the city you are moving from and the city you are moving to, for you to build a research station.");
            }
        }

        public virtual void BuildResearchStation()
        {
            if (!CurrentCity.ResearchStation)
            {
                if (CardInHand(CurrentCity.Name))
                {
                    CurrentCity.BuildResearchStation();
                    Discard(CurrentCity.Name);
                    RemainingActions--;
                    _state.BuildResearchStation();
                } else
                {
                    throw new IllegalMoveException($"You need to have the city card for {CurrentCity.Name} in your hand, in order to build a research station in this city");
                }
            } else
            {
                throw new IllegalMoveException("There is already a research station in the city you're in.");
            }
        }

        public virtual void TreatDisease(Colors Color)
        {
            try
            {
                CurrentCity.TreatDisease(Color);
                RemainingActions--;
            } catch { throw; }      //unexpectedbehaviour exception if Colors == None || illegalmoveexception if no cubes of that color
        }

        public void DiscoverCure()
        {
            List<Card> CardsOfSameColor = new List<Card>();
            int[] CardCount = new int[5];
            Colors CureColor = Colors.None;
            foreach (Card CurrentCard in Hand)
            {
                CardCount[(int)CurrentCard.Color]++;
                if (CardCount[(int)CurrentCard.Color]==CardsNecessaryForCure)
                {
                    CureColor = CurrentCard.Color;
                }
            }

            if(CureColor == Colors.None)
            {
                throw new IllegalMoveException($"You don't have enough cards of the same color in your hand. You need {CardsNecessaryForCure} cards of the same color, to discover a cure.");
            }
            else if(_state.Cures[CureColor] == true)
            {
                throw new IllegalMoveException($"The {CureColor} cure has already been discovered");
            }
            else
            {
                List<Card> AvailableCardsForCure = null;
                foreach (Card CurrentCard in Hand)
                {
                    if (CurrentCard.Color == CureColor)
                    {
                        AvailableCardsForCure.Add(CurrentCard);
                    }
                }

                if (AvailableCardsForCure.Count > CardsNecessaryForCure)
                {
                    int Choice = TextManager.ChooseItemFromList(AvailableCardsForCure, "keep");
                    AvailableCardsForCure.RemoveAt(Choice);
                }

                Discard(AvailableCardsForCure);
                _state.Cures[CureColor] = true;
                if (GameWon())
                {
                    throw new GameWonException();
                }
                else
                {
                    RemainingActions--;
                }
            }
        }

        public virtual void ShareKnowledge(Role OtherPlayer)
        {
            //Dette sjekkes jo når jeg tar ibruk funksjonen, men kanskje greit å ha en ekstra sjekk allikevel?
            if (OtherPlayer.CurrentCity != CurrentCity)
            {
                throw new IllegalMoveException($"You need to be in the same city in order to share knowledge. {OtherPlayer.RoleName} is not in {CurrentCity.Name}.");
            }

            Role GivingPlayer;
            Role ReceivingPlayer;
            if(OtherPlayer is Scientist)
            {
                int Choice = TextManager.ShareKnowledgeWithScientist();
                if (Choice == 1)
                {
                    GivingPlayer = OtherPlayer;
                    ReceivingPlayer = this;
                } else if (Choice == 2)
                {
                    GivingPlayer = this;
                    ReceivingPlayer = OtherPlayer;
                } else
                {
                    throw new UnexpectedBehaviourException("An error occured in ShareKnowledge while deciding who should be receiving a card");
                }
            } 
            else
            {
                if (OtherPlayer.CardInHand(CurrentCity.Name))
                {
                    GivingPlayer = OtherPlayer;
                    ReceivingPlayer = this;
                }
                else if (this.CardInHand(CurrentCity.Name))
                {
                    GivingPlayer = this;
                    ReceivingPlayer = OtherPlayer;
                }
                else
                {
                    throw new IllegalMoveException($"Neither of you have the card for {CurrentCity.Name} in you hand. You need to be in the same city as the card you want to exchange in order to share knowledge. ");
                }
            }

            GivingPlayer.GiveCard(ReceivingPlayer);
            RemainingActions--;
        }

        public virtual void GiveCard(Role OtherPlayer)
        {
            Card CardToGive = Hand.Find(Card => Card.Name == CurrentCity.Name);
            if (CardToGive == null)
            {
                throw new IllegalMoveException($"The {RoleName} doesn't have the City Card for {CurrentCity.Name} in their hand to give");
            }

            Hand.Remove(CardToGive);
            OtherPlayer.ReceiveCard(CardToGive);
        }

        public void ReceiveCard(Card Card)
        {
            Hand.Add(Card);

            if (Hand.Count > 7)
            {
                Discard();
            }
        }

        public Boolean CardInHand(string Name)
        {
            return Hand.Exists(Card => Card.Name == Name);
        }

        public int NumberOfCityCardsInHand()
        {
            int Counter = 0;
            foreach(Card CurrentCard in Hand)
            {
                if(CurrentCard is CityCard)
                {
                    Counter++;
                }
            }
            return Counter;
        }

        void Discard()
        {
            int Choice = TextManager.DiscardOrPlay(Hand);
            
            if (Hand[Choice] is EventCard)
            {
                Hand[Choice].Play(this);
            } else
            {
                Discard(Choice);
            }
        }

        public void Discard(List<Card> CardsToDiscard)
        {
            foreach (Card CurrentCard in CardsToDiscard)
            {
                Hand.Remove(CurrentCard);
            }
        }

        public void Discard (String CardName)
        {
            Card TempCard = Hand.Find(Card => Card.Name == CardName);
            Hand.Remove(TempCard);
        }

        public void Discard (int index)
        {
            Hand.RemoveAt(index);
        }

        public virtual void PrintSpecialAbilities()
        {
            throw new Exception($"The PrintSpecialAbilities method for {RoleName} is missing");
        }

        public virtual void PlayFirstSpecialAbility()
        {
            throw new IllegalMoveException($"The {RoleName} don't have any active special abilities to play");
        }

        public virtual void PlaySecondSpecialAbility(  )
        {
            throw new IllegalMoveException($"The {RoleName} don't have any active special abilities to play");
        }

        public virtual void Reset()
        {
            RemainingActions = MaxActions;
        }

        protected Boolean GameWon()
        {
            for (int i = 0; i<4; i++)
            {
                if (!_state.Cures[(Colors)i])
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            return RoleName;
        }
    }
}
