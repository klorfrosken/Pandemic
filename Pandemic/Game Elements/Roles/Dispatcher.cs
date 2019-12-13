using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Cards;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Game_Elements.Roles
{
    public class Dispatcher : Role
    {
        readonly static string Title = "Dispatcher";

        public Dispatcher (City StartingCity, int PlayerID) : base(PlayerID, Title, StartingCity) 
        {
            SpecialActions = 2;
        }

        public override void PrintSpecialAbilities()
        {
            int i = TextManager.AvailableStandardActions;
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine($"{i + 1}: CONNECT PAWNS - Move any pawn, if its owner agrees, to any city containing another pawn.");
            Console.WriteLine($"{i + 2}: MOVE ANOTHER PLAYER - Move another player's pawn, if its owner agrees, as if it were their own.");
            Console.WriteLine("When mvoing a player's pawn as if it were your own, discard cards from _your_ hand when applicable. You may not use other players' special ability for the movement.");
        }

        void PrintMoveAnotherPlayer()
        {
            Console.WriteLine("You have the following available ways to move another player: \n");
            Console.WriteLine("1: DRIVE/FERRY - Move another to a city connected by a white line to their current city");
            Console.WriteLine("2: DIRECT FLIGHT - Discard one of your city cards to move another player to the city named on that card");
            Console.WriteLine("3: CHARTER FLIGHT - Discard the city card that matches the city another player is in to move that player to _any_ city");
            Console.WriteLine("4: SHUTTLE FLIGHT - Move another player from a city with a research station to any other city that has a research station \n");

            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("Which one of the above would you like to do?");
        }

        public override void PlayFirstSpecialAbility(StateManager State)
        {
            int ChoiceOfOtherPlayer = TextManager.ChooseItemFromList(State.Roles, "move");
            Role OtherPlayer = State.Roles[ChoiceOfOtherPlayer];

            List<Role> EligibleTargetPlayers = new List<Role>(State.Roles);
            EligibleTargetPlayers.Remove(OtherPlayer);

            int ChoiceOfTargetPlayer = TextManager.ChooseItemFromList(EligibleTargetPlayers, "move to");
            Role TargetPlayer = EligibleTargetPlayers[ChoiceOfTargetPlayer];

            try
            {
                ConnectPawns(OtherPlayer, TargetPlayer, State);
            }
            catch { throw; }    //throws illegalmoveexception if the two players are already in the same city.
        }

        public override void PlaySecondSpecialAbility(StateManager State)
        {

            Boolean ActionPerformed = false;
            do
            {            
                PrintMoveAnotherPlayer();
                int ChosenIndex = TextManager.GetValidInteger(1, 4);

                switch (ChosenIndex)
                {
                    case 1:
                        //DRIVE/FERRY - Move another to a city connected by a white line to their current city
                        try
                        {
                            int ChoiceOfOtherPlayer = TextManager.ChooseItemFromList(State.Roles, "move");
                            Role OtherPlayer = State.Roles[ChoiceOfOtherPlayer];

                            int ChoiceOfTargetCity = TextManager.ChooseItemFromList(OtherPlayer.CurrentCity.ConnectedCities, "move them to");
                            City NextCity = OtherPlayer.CurrentCity.ConnectedCities[ChoiceOfTargetCity];

                            DriveFerryForPlayer(NextCity, OtherPlayer, State);
                            ActionPerformed = true;
                        }
                        catch (IllegalMoveException ex)
                        {
                            ex.ToString();
                        }
                        catch { throw; }
                        break;
                    case 2:
                        //DIRECT FLIGHT - Discard one of your city cards to move another player to the city named on that card"
                        try
                        {
                            List<CityCard> EligibleCards = new List<CityCard>();
                            foreach (Card CurrentCard in Hand)
                            {
                                if (CurrentCard is CityCard)
                                {
                                    EligibleCards.Add(CurrentCard as CityCard);
                                }
                            }

                            int ChoiceOfTargetCity = -1;
                            if (EligibleCards.Count == 0)
                            {
                                throw new IllegalMoveException("You don't have any cards you can discard for a direct flight");
                            }
                            else if (EligibleCards.Count == 1)
                            {
                                ChoiceOfTargetCity = 0;
                            }
                            else
                            {
                                ChoiceOfTargetCity = TextManager.ChooseItemFromList(EligibleCards, "move another player to");
                            }

                            City NextCity = State.GetCity(EligibleCards[ChoiceOfTargetCity]);

                            int ChoiceOfOtherPlayer = TextManager.ChooseItemFromList(State.Roles, "players to move");
                            Role OtherPlayer = State.Roles[ChoiceOfOtherPlayer];

                            DirectFlightForPlayer(NextCity, OtherPlayer, State);
                            ActionPerformed = true;
                        }
                        catch (IllegalMoveException ex)
                        {
                            ex.ToString();
                        }
                        catch { throw; }
                        break;
                    case 3:
                        //CHARTER FLIGHT - Discard the city card that matches the city another player is in to move that player to _any_ city"
                        try
                        {
                            int ChoiceOfOtherPlayer = TextManager.ChooseItemFromList(State.Roles, " players to move");
                            Role OtherPlayer = State.Roles[ChoiceOfOtherPlayer];

                            if (CardInHand(OtherPlayer.CurrentCity.Name))
                            {
                                int ChoiceOfTargetCity = TextManager.ChooseItemFromList(State.Cities, "go to");
                                City NextCity = State.Cities[ChoiceOfTargetCity];
                                CharterFlightForPlayer(NextCity, OtherPlayer, State);
                                ActionPerformed = true;
                            }
                            else
                            {
                                throw new IllegalMoveException($"You need to have the City Card for your current city, {OtherPlayer.CurrentCity.Name} in your hand in order to charter a flight from there.");
                            }
                        }
                        catch (IllegalMoveException ex)
                        {
                            ex.ToString();
                        }
                        catch { throw; }
                        break;
                    case 4:
                        //SHUTTLE FLIGHT - Move another player from a city with a research station to any other city that has a research station
                        try
                        {
                            int ChoiceOfOtherPlayer = TextManager.ChooseItemFromList(State.Roles, " players to move");
                            Role OtherPlayer = State.Roles[ChoiceOfOtherPlayer];

                            List<City> ResearchStations = State.GetCitiesWithResearchStation();
                            if (!OtherPlayer.CurrentCity.ResearchStation)
                            {
                                throw new IllegalMoveException("The player you want to move must be in a city with a research station in order for you to shuttle a flight for them.");
                            } else if (ResearchStations.Count == 0)
                            {
                                throw new IllegalMoveException("There are no other research stations for that player to shuttle to.");
                            } else
                            {
                                ResearchStations.Remove(OtherPlayer.CurrentCity);
                                int Choice = -1;
                                if (ResearchStations.Count == 1)
                                {
                                    Choice = 0;
                                } else
                                {
                                    Choice = TextManager.ChooseItemFromList(ResearchStations, "move the player to");
                                }

                                City NextCity = ResearchStations[Choice];
                                ShuttleFlightForPlayer(NextCity, OtherPlayer, State);
                                ActionPerformed = true;
                            }
                        }
                        catch (IllegalMoveException ex)
                        {
                            ex.ToString();
                        }
                        catch { throw; }
                        break;
                    default:
                        throw new UnexpectedBehaviourException($"The program crashed unexpectedly due to an invalid argument in the PlaySecondSpecialAbility method of {Title}. \nThe switch received an invalid case");
                }
            } while (!ActionPerformed);
        }

        void ConnectPawns(Role OtherPlayer, Role TargetPlayer, StateManager State)
        {
            if (OtherPlayer.CurrentCity != TargetPlayer.CurrentCity)
            {
                OtherPlayer.ChangeCity(TargetPlayer.CurrentCity, State);
                RemainingActions--;
            }
            else
            {
                throw new IllegalMoveException($"The {OtherPlayer.RoleName} is already in the same city as the {TargetPlayer.RoleName}");
            }
        }

        void DriveFerryForPlayer(City NextCity, Role OtherPlayer, StateManager State)
        {
            if (OtherPlayer.CurrentCity.IsConnectedTo(NextCity))
            {
                OtherPlayer.ChangeCity(NextCity, State);
                RemainingActions--;
            }
            else
            {
                throw new IllegalMoveException($"{NextCity.Name} is not connected to {CurrentCity.Name} and you cannot move there.");
            }
        }

        void DirectFlightForPlayer(City NextCity, Role OtherPlayer, StateManager State)
        {
            if (CardInHand(NextCity.Name))
            {
                OtherPlayer.ChangeCity(NextCity, State);
                Discard(NextCity.Name);
                RemainingActions--;
             }
            else 
            {
                throw new IllegalMoveException($"You need to have the {NextCity.Name} City Card in _your_ hand in order to move another player to {NextCity.Name}");
            }
        }

        void CharterFlightForPlayer(City NextCity, Role OtherPlayer, StateManager State)
        {
            if (CardInHand(CurrentCity.Name))
            {
                Discard(CurrentCity.Name);
                OtherPlayer.ChangeCity(NextCity, State);
                RemainingActions--;
            }
            else
            {
                throw new IllegalMoveException($"You need to have the {CurrentCity.Name} City Card in _your_ hand in order to charter a flight for another player");

            }
        }

        void ShuttleFlightForPlayer(City NextCity, Role OtherPlayer, StateManager State)
        {
            if (OtherPlayer.CurrentCity.ResearchStation && NextCity.ResearchStation)
            {
                OtherPlayer.ChangeCity(NextCity, State);
                RemainingActions--;
            }
            else
            {
                throw new IllegalMoveException("There needs to be a research station in both the city you are moving from and the city you are moving to, for you to build a research station.");
            }
        }
    }
}
