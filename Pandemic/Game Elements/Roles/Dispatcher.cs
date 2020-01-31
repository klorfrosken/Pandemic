using System;
using System.Collections.Generic;
using Pandemic.Cards;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Exceptions;

namespace Pandemic.Game_Elements.Roles
{
    public class Dispatcher : Role
    {
        readonly static string Title = "Dispatcher";

        public Dispatcher(City StartingCity, int PlayerID, StateManager state = null, ITextManager textManager = null) : base(PlayerID, Title, StartingCity, state, textManager) 
        {
            SpecialActions = 2;
        }

        public override void PrintSpecialAbilities()
        {
            int i = textManager.AvailableStandardActions;
            Console.WriteLine("SPECIAL ABILITIES:");
            Console.WriteLine($"{i + 1}: CONNECT PAWNS - Move any pawn, if its owner agrees, to any city containing another pawn.");
            Console.WriteLine($"{i + 2}: MOVE ANOTHER PLAYER - Move another player's pawn, if its owner agrees, as if it were your own.");
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

        public override void PlayFirstSpecialAbility()
        {
            int choiceOfOtherPlayer = textManager.ChooseItemFromList(state.Roles, "move");
            Role OtherPlayer = state.Roles[choiceOfOtherPlayer];

            List<Role> eligibleTargetPlayers = new List<Role>(state.Roles);
            eligibleTargetPlayers.Remove(OtherPlayer);

            int choiceOfTargetPlayer = textManager.ChooseItemFromList(eligibleTargetPlayers, "move to");
            Role targetPlayer = eligibleTargetPlayers[choiceOfTargetPlayer];

            ConnectPawns(OtherPlayer, targetPlayer);     //throws illegalmoveexception if the two players are already in the same city.
        }

        public override void PlaySecondSpecialAbility()
        {         
            PrintMoveAnotherPlayer();
            int chosenIndex = textManager.GetValidInteger(1, 4);

            switch (chosenIndex)
            {
                case 1:
                    //DRIVE/FERRY - Move another to a city connected by a white line to their current city
                    {
                        int choiceOfOtherPlayer = textManager.ChooseItemFromList(state.Roles, "move");
                        Role otherPlayer = state.Roles[choiceOfOtherPlayer];

                        int choiceOfTargetCity = textManager.ChooseItemFromList(otherPlayer.CurrentCity.ConnectedCities, "move them to");
                        City nextCity = otherPlayer.CurrentCity.ConnectedCities[choiceOfTargetCity];

                        DriveFerryForPlayer(nextCity, otherPlayer);
                    }
                    break;
                case 2:
                    //DIRECT FLIGHT - Discard one of your city cards to move another player to the city named on that card"
                    {
                        List<CityCard> eligibleCards = new List<CityCard>();
                        foreach (PlayerCard currentCard in Hand)
                        {
                            if (currentCard is CityCard)
                            {
                                eligibleCards.Add(currentCard as CityCard);
                            }
                        }

                        int choiceOfTargetCity = -1;
                        if (eligibleCards.Count == 0)
                        {
                            throw new IllegalMoveException("You don't have any cards you can discard for a direct flight");
                        }
                        else if (eligibleCards.Count == 1)
                        {
                            choiceOfTargetCity = 0;
                        }
                        else
                        {
                            choiceOfTargetCity = textManager.ChooseItemFromList(eligibleCards, "move another player to");
                        }

                        City nextCity = state.GetCity(eligibleCards[choiceOfTargetCity]);

                        int choiceOfOtherPlayer = textManager.ChooseItemFromList(state.Roles, "players to move");
                        Role otherPlayer = state.Roles[choiceOfOtherPlayer];

                        DirectFlightForPlayer(nextCity, otherPlayer);
                    }
                    break;
                case 3:
                    //CHARTER FLIGHT - Discard the city card that matches the city another player is in to move that player to _any_ city"
                    {
                        int choiceOfOtherPlayer = textManager.ChooseItemFromList(state.Roles, "move");
                        Role otherPlayer = state.Roles[choiceOfOtherPlayer];

                        int choiceOfTargetCity = textManager.ChooseItemFromList(state.Cities.Values, "go to");
                        City nextCity = state.GetCity(choiceOfTargetCity);
                        CharterFlightForPlayer(nextCity, otherPlayer);
                    }
                    break;
                case 4:
                    //SHUTTLE FLIGHT - Move another player from a city with a research station to any other city that has a research station
                    {
                        List<Role> eligiblePlayers = new List<Role>();
                        foreach (Role currentPlayer in state.Roles)
                        {
                            if (currentPlayer.CurrentCity.HasResearchStation)
                            {
                                eligiblePlayers.Add(currentPlayer);
                            }
                        }

                        List<City> researchStationCities = state.GetCitiesWithResearchStation();

                        if (eligiblePlayers.Count == 0)
                        {
                            throw new IllegalMoveException("No players have research stations in the cities they are in, so they cannot be shuttled anywhere");
                        } else if (researchStationCities.Count == 1)
                        {
                            throw new IllegalMoveException("There needs to be a research station in both the city someone is moving from and the city they are moving to, for you to shuttle a flight. \nCurrently there's only one research station in the game...");
                        } else
                        {
                            int choiceOfOtherPlayer;
                            Role otherPlayer;
                            if (eligiblePlayers.Count == 1)
                            {
                                choiceOfOtherPlayer = 0;
                            } else
                            {
                                choiceOfOtherPlayer = textManager.ChooseItemFromList(eligiblePlayers, " players to move");
                            }

                            otherPlayer = eligiblePlayers[choiceOfOtherPlayer];

                            researchStationCities.Remove(otherPlayer.CurrentCity);
                            int choiceOfNextCity;
                            if (researchStationCities.Count == 1)
                            {
                                choiceOfNextCity = 0;
                            }
                            else
                            {
                                choiceOfNextCity = textManager.ChooseItemFromList(researchStationCities, "move the player to");
                            }

                            City nextCity = researchStationCities[choiceOfNextCity];
                            ShuttleFlightForPlayer(nextCity, otherPlayer);
                        }
                    }
                    break;
                default:
                    throw new UnexpectedBehaviourException($"The program crashed unexpectedly due to an invalid argument in the PlaySecondSpecialAbility method of {Title}. \nThe switch received an invalid case");
            }
        }

        void ConnectPawns(Role otherPlayer, Role targetPlayer)
        {
            if (otherPlayer.CurrentCity != targetPlayer.CurrentCity)
            {
                otherPlayer.ChangeCity(targetPlayer.CurrentCity);
                RemainingActions--;
            }
            else
            {
                throw new IllegalMoveException($"The {otherPlayer.RoleName} is already in the same city as the {targetPlayer.RoleName}");
            }
        }

        void DriveFerryForPlayer(City nextCity, Role otherPlayer)
        {
            if (otherPlayer.CurrentCity.IsConnectedTo(nextCity))
            {
                otherPlayer.ChangeCity(nextCity);
                RemainingActions--;
            }
            else
            {
                throw new IllegalMoveException($"{nextCity.Name} is not connected to {otherPlayer.CurrentCity} and they cannot be moved there.");
            }
        }

        void DirectFlightForPlayer(City nextCity, Role otherPlayer)
        {
            if (CardInHand(nextCity.Name))
            {
                otherPlayer.ChangeCity(nextCity);
                Discard(nextCity.Name);
                RemainingActions--;
             }
            else 
            {
                throw new IllegalMoveException($"You need to have the {nextCity} City Card in _your_ hand in order to move another player to {nextCity}");
            }
        }

        void CharterFlightForPlayer(City nextcity, Role otherPlayer)
        {
            if (CardInHand(CurrentCity.Name))
            {
                Discard(CurrentCity.Name);
                otherPlayer.ChangeCity(nextcity);
                RemainingActions--;
            }
            else
            {
                throw new IllegalMoveException($"You need to have the {otherPlayer.CurrentCity} City Card in _your_ hand in order to charter a flight for another player");

            }
        }

        void ShuttleFlightForPlayer(City nextCity, Role otherPlayer)
        {
            if (!otherPlayer.CurrentCity.HasResearchStation)
            {
                throw new IllegalMoveException($"There needs to be a research station in both the city someone is moving from and the city they are moving to, for you to charter a flight for another player. \n{otherPlayer.CurrentCity}, where {otherPlayer} is, doesn't have one");
            } else if (!nextCity.HasResearchStation)
            {
                throw new IllegalMoveException($"There needs to be a research station in both the city someone is moving from and the city they are moving to, for you to charter a flight for another player. \n{nextCity}, where {otherPlayer} is to go, doesn't have one");
            }
            else
            {
                otherPlayer.ChangeCity(nextCity);
                RemainingActions--;
            }
        }

        //Unit test methods
        public void TestConnectPawns(Role otherPlayer, Role targetPlayer)
        {
            ConnectPawns(otherPlayer, targetPlayer);
        }
        public void TestDriveFerryForPlayer(City nextCity, Role otherPlayer)
        {
            DriveFerryForPlayer(nextCity, otherPlayer);
        }
        public void TestDirectFlightForPlayer(City nextCity, Role otherPlayer)
        {
            DirectFlightForPlayer(nextCity, otherPlayer);
        }
        public void TestCharterFlightForPlayer(City nextCity, Role otherPlayer)
        {
            CharterFlightForPlayer(nextCity, otherPlayer);
        }
        public void TestShuttleFlightForPlayer(City nextCity, Role otherPlayer)
        {
            ShuttleFlightForPlayer(nextCity, otherPlayer);
        }
    }
}
