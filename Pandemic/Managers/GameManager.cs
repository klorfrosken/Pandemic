﻿using System;
using System.Collections.Generic;
using Pandemic.Game;
using Pandemic.Exceptions;
using Pandemic.Cards;

namespace Pandemic.Managers
{
    class GameManager
    {
        StateManager State;
        List<User> Users = new List<User>
        {
            new User(0, "Ragna Rekkverk"),
            new User(1, "Frida Frosk"),
            new User(2, "Sandra Salamander"),
            new User(3, "Pelle Parafin")
        };

        public GameManager()
        {
            InitiateGame();

            try
            {
                PlayGame(State);
            }
            catch (GameWonException ex)
            {
                ex.ToString();
                foreach (User CurrentUser in Users)
                {
                    CurrentUser.UpdateStatistics("Won");
                }
            } catch (TheWorldIsDeadException ex)
            {
                ex.ToString();
                foreach (User CurrentUser in Users)
                {
                    CurrentUser.UpdateStatistics("Lost");
                }
            } catch (UnexpectedBehaviourException ex)
            {
                ex.ToString();
                foreach (User CurrentUser in Users)
                {
                    CurrentUser.UpdateStatistics("Error");
                }
            }
        }

        void InitiateGame()
        {
            int NumberOfEpidemics = TextManager.GetDifficulty();
            State = new StateManager(NumberOfEpidemics: NumberOfEpidemics, Users: Users);
            TextManager.BeginGame(State, Users);
        }

        void PlayGame(StateManager State)
        {
            Boolean DonePlaying = false;
            do
            {
                foreach (User CurrentUser in Users)
                {
                    try
                    {
                        DoActions(CurrentUser.CurrentRole);
                        CurrentUser.CurrentRole.Draw(State);
                        CurrentUser.CurrentRole.Draw(State);

                        int infectionRate = State.InfectionRates[State.InfectionIndex];
                        for (int i = 0; i<infectionRate; i++)
                        {
                            State.InfectionDeck.Infect(State);
                        }
                    }
                    catch (UnexpectedBehaviourException ex)
                    {
                        ex.ToString();
                    }
                    catch { throw; }
                }
            } while (!DonePlaying);
        }

        void DoActions(Role CurrentPlayer)
        {
            do
            {
                TextManager.AvailableActions(CurrentPlayer);

                int AvailableActions = TextManager.AvailableStandardActions + CurrentPlayer.SpecialActions;
                int ChosenAction = TextManager.GetValidInteger(1, AvailableActions);

                switch (ChosenAction)
                {
                    case 1:
                        //DRIVE/FERRY - Move to a city connected by a white line
                        try
                        {
                            int Choice = TextManager.ChooseItemFromList(CurrentPlayer.CurrentCity.ConnectedCities, "go to");
                            City NextCity = CurrentPlayer.CurrentCity.ConnectedCities[Choice];
                            CurrentPlayer.DriveFerry(NextCity, State);
                        }
                        catch (IllegalMoveException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch { throw; }
                        break;
                    case 2:
                        //DIRECT FLIGHT - Discard a city card to move to the city named on that card
                        try
                        {
                            List<CityCard> EligibleCards = new List<CityCard>();
                            foreach(Card CurrentCard in CurrentPlayer.Hand)
                            {
                                if(CurrentCard is CityCard)
                                {
                                    EligibleCards.Add(CurrentCard as CityCard);
                                }
                            }

                            int Choice = -1;
                            if (EligibleCards.Count == 0)
                            {
                                throw new IllegalMoveException("You don't have any cards you can discard for a direct flight");
                            } else if (EligibleCards.Count == 1)
                            {
                                Choice = 0;
                            } else
                            {
                                Choice = TextManager.ChooseItemFromList(EligibleCards, "go to");
                            }
                           
                            City NextCity = State.GetCity(EligibleCards[Choice]);
                            CurrentPlayer.DirectFlight(NextCity, State);
                        }
                        catch (IllegalMoveException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch { throw; }
                        break;
                    case 3:
                        //CHARTER FLIGHT - Discard the city card that matches the city you are in to move to _any_ city
                        try
                        {
                            if (CurrentPlayer.CardInHand(CurrentPlayer.CurrentCity.Name))
                            {
                                int Choice = TextManager.ChooseItemFromList(State.Cities, "go to");
                                City NextCity = State.Cities[Choice];
                                CurrentPlayer.CharterFlight(NextCity, State);

                            } else
                            {
                                throw new IllegalMoveException($"You need to have the City Card for your current city, {CurrentPlayer.CurrentCity.Name} in your hand in order to charter a flight from there.");
                            }
                        }
                        catch (IllegalMoveException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch { throw; }
                        break;
                    case 4:
                        //SHUTTLE FLIGHT - Move from a city with a research station to any other city that has a research station
                        try
                        {
                            List<City> ResearchStations = State.GetCitiesWithResearchStation();

                            if (ResearchStations.Count == 0)
                            {
                                throw new IllegalMoveException("You must be in a city with a research station in order for you to shuttle somewhere.");
                            }
                            else if (ResearchStations.Count == 1)
                            {
                                throw new IllegalMoveException("There are no other research stations for you to shuttle to.");
                            } else
                            {
                                int Choice = TextManager.ChooseItemFromList(ResearchStations, "go to");
                                City NextCity = State.GetCity(CurrentPlayer.Hand[Choice]);
                                CurrentPlayer.ShuttleFlight(NextCity, State);
                            }
                            
                        }
                        catch (IllegalMoveException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch { throw; }
                        break;
                    case 5:
                        //BUILD A RESEARCH STATION - Discard the city card that _matches_ the city that you are in to place a research station there
                        try
                        {
                            CurrentPlayer.BuildResearchStation(State);
                        }
                        catch (IllegalMoveException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch { throw; }
                        break;
                    case 6:
                        //TREAT DISEASE - Remove one disease cube from the city you are in. If this color is cured remove all cubes _of that color_ from the city
                        try
                        {
                            if (!CurrentPlayer.CurrentCity.MultipleDiseases)
                            {
                                CurrentPlayer.TreatDisease(CurrentPlayer.CurrentCity.Color, State);
                            } else
                            {
                                List<string> PrintLines = new List<string>();
                                List<Colors> DiseaseColors = new List<Colors>();

                                string ScreenText;
                                for (int i=1; i < 5; i++)
                                {
                                    if (CurrentPlayer.CurrentCity.DiseaseCubes[(Colors)i] != 0)
                                    {
                                        DiseaseColors.Add((Colors)i);
                                        ScreenText = $"{(Colors)i}: {CurrentPlayer.CurrentCity.DiseaseCubes[(Colors)i]} disease cubes";
                                        PrintLines.Add(ScreenText);
                                    }
                                }

                                int Choice = TextManager.ChooseItemFromList(PrintLines, "cure");
                                CurrentPlayer.TreatDisease(DiseaseColors[Choice], State);
                            }
                        }
                        catch (IllegalMoveException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch { throw; }
                        break;
                    case 7:
                        //DISCOVER A CURE - At _any_ research station, discard 5 city cards of the same disease color to cure that disease
                        try
                        {
                            if (!CurrentPlayer.CurrentCity.ResearchStation)
                            {
                                throw new IllegalMoveException($"There's no research station in {CurrentPlayer.CurrentCity.Name}. There has to be a research station in your location for you to discover a cure.");
                            } else
                            {
                                CurrentPlayer.DiscoverCure(State);
                            }
                        }
                        catch (IllegalMoveException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch { throw; }
                        break;
                    case 8:
                        //SHARE KNOWLEDGE - Either: _give_ the card that _matches_ the city that you are in to another player, or _take_ that card from another player. The other player must also be in the city with you
                        try
                        {
                            List<Role> PlayersInCity = new List<Role>();
                            foreach (Role TempPlayer in State.Roles)
                            {
                                if (TempPlayer.CurrentCity == CurrentPlayer.CurrentCity)
                                {
                                    if (TempPlayer != CurrentPlayer)
                                    {
                                        PlayersInCity.Add(TempPlayer);
                                    }
                                }
                            }

                            Role OtherPlayer;
                            if (PlayersInCity.Count == 0)
                            {
                                throw new IllegalMoveException($"There are no other players in {CurrentPlayer.CurrentCity.Name} for you to share knowledge with.");
                            }
                            else if (PlayersInCity.Count == 1)
                            {
                                OtherPlayer = PlayersInCity[0];
                            } else
                            {
                                int Choice = TextManager.ChooseItemFromList(PlayersInCity, "share knowledge with");
                                OtherPlayer = PlayersInCity[Choice];
                            }

                            CurrentPlayer.ShareKnowledge(OtherPlayer, State);
                        }
                        catch (IllegalMoveException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch { throw; }
                        break;
                    case 9:
                        //First special ability
                        try
                        {
                            CurrentPlayer.PlayFirstSpecialAbility(State);
                        }
                        catch (IllegalMoveException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch { throw; }
                        break;
                    case 10:
                        //Second special ability
                        try
                        {
                            CurrentPlayer.PlaySecondSpecialAbility(State);
                        }
                        catch (IllegalMoveException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch { throw; }
                        break;
                    default:
                        throw new UnexpectedBehaviourException("The program crashed unexpectedly due to an invalid argument in the TakeTurn method of GameManager. \nThe switch received an invalid case");
                }
            } while (CurrentPlayer.RemainingActions != 0) ;
        }
    }
}
