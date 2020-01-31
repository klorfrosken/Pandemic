using System;
using System.Collections.Generic;
using Pandemic.Game;
using Pandemic.Exceptions;
using Pandemic.Cards;

namespace Pandemic.Managers
{
    public class GameManager
    {
        StateManager _state;
        readonly ITextManager _textManager = new TextManager();

        public List<User> Users = new List<User>
        {
            new User(0, "Ragna Rekkverk"),
            new User(1, "Frida Frosk"),
            new User(2, "Sandra Salamander"),
            new User(3, "Pelle Parafin")
        };

        public GameManager(bool testing = false, StateManager state = null, ITextManager txtMgr = null)
        {
            if(txtMgr != null)
            {
                _textManager = txtMgr;
            } 

            if(state != null)
            {
                _state = state;
            }

            if (!testing)
            {
                InitiateGame();
            }
        }

        void InitiateGame()
        {
            int numberOfEpidemics = _textManager.GetDifficulty();
            _state = new StateManager(numberOfEpidemics: numberOfEpidemics, users: Users, textManager: _textManager);
            _textManager.PrintBeginGame(Users);

            try
            {
                PlayGame();
            }
            catch (GameWonException ex)
            {
                ex.ToString();
                foreach (User currentUser in Users)
                {
                    currentUser.UpdateStatistics("Won");
                }
            }
            catch (TheWorldIsDeadException ex)
            {
                ex.ToString();
                foreach (User currentUser in Users)
                {
                    currentUser.UpdateStatistics("Lost");
                }
            }
            catch (UnexpectedBehaviourException ex)
            {
                ex.ToString();
                foreach (User currentUser in Users)
                {
                    currentUser.UpdateStatistics("Error");
                }
            }
        }

        void PlayGame()
        {
            Boolean donePlaying = false;
            do
            {
                foreach (User currentUser in Users)
                {
                    DoActions(currentUser.CurrentRole);
                    try
                    {
                        currentUser.CurrentRole.Draw();
                        currentUser.CurrentRole.Draw();
                    }
                    catch
                    {
                        throw new TheWorldIsDeadException("There are no more cards in the player deck. You were too slow and the world is now suffering for it");
                    }

                    int infectionRate = _state.InfectionRates[_state.InfectionIndex];
                    for (int i = 0; i<infectionRate; i++)
                    {
                        _state.InfectionDeck.Infect();
                    }
                }
            } while (!donePlaying);
        }

        void DoActions(Role currentPlayer)
        {
            _textManager.PrintNewTurn(_state);

            do
            {
                _textManager.AvailableActions(currentPlayer);

                int availableActions = _textManager.AvailableStandardActions + currentPlayer.SpecialActions;
                int chosenAction = _textManager.GetValidInteger(1, availableActions);

                try
                {

                    switch (chosenAction)
                    {
                        case 1:
                            DriveFerry(currentPlayer);
                            break;
                        case 2:
                            DirectFlight(currentPlayer);
                            break;
                        case 3:
                            CharterFlight(currentPlayer);
                            break;
                        case 4:
                            ShuttleFlight(currentPlayer);
                            break;
                        case 5:
                            BuildResearchStation(currentPlayer);
                            break;
                        case 6:
                            TreatDisease(currentPlayer);
                            break;
                        case 7:
                            DiscoverCure(currentPlayer);
                            break;
                        case 8:
                            ShareKnowledge(currentPlayer);
                            break;
                        case 9:
                            FirstSpecialAbility(currentPlayer);
                            break;
                        case 10:
                            SecondSpecialAbility(currentPlayer);
                            break;
                        case 11:
                            PlayEvent(currentPlayer);
                            break;
                        default:
                            throw new UnexpectedBehaviourException("The program crashed unexpectedly due to an invalid argument in the DoActions method of GameManager. \nThe switch received an invalid case");
                    }
                }catch (IllegalMoveException ex)
                {
                    Console.WriteLine("|---------------------------------------------------------------------------------------|");
                    Console.WriteLine(ex.Message);
                }

            } while (currentPlayer.RemainingActions != 0) ;
        }

        //Action methods
        void DriveFerry(Role currentRole)
        {
            //DRIVE/FERRY - Move to a city connected by a white line
            int choice = _textManager.ChooseItemFromList(currentRole.CurrentCity.ConnectedCities, "go to");
            City nextCity = currentRole.CurrentCity.ConnectedCities[choice];
            currentRole.DriveFerry(nextCity);
            _textManager.PrintPlayerMoved(currentRole, nextCity);
        }

        void DirectFlight(Role currentPlayer)
        {
            //DIRECT FLIGHT - Discard a city card to move to the city named on that card
            List<CityCard> eligibleCards = new List<CityCard>();
            foreach (Card currentCard in currentPlayer.Hand)
            {
                if (currentCard is CityCard)
                {
                    eligibleCards.Add(currentCard as CityCard);
                }
            }

            int choice = -1;
            if (eligibleCards.Count == 0)
            {
                throw new IllegalMoveException("You don't have any cards you can discard for a direct flight");
            }
            else if (eligibleCards.Count == 1)
            {
                choice = 0;
            }
            else
            {
                choice = _textManager.ChooseItemFromList(eligibleCards, "go to");
            }

            City nextCity = _state.GetCity(eligibleCards[choice]);
            currentPlayer.DirectFlight(nextCity);
            _textManager.PrintPlayerMoved(currentPlayer, nextCity);
        }

        void CharterFlight(Role currentPlayer)
        {
            //CHARTER FLIGHT - Discard the city card that matches the city you are in to move to _any_ city
            if (currentPlayer.CardInHand(currentPlayer.CurrentCity.Name))
            {
                int choice = _textManager.ChooseItemFromList(_state.Cities.Values, "go to");
                City nextCity = _state.GetCity(choice);
                currentPlayer.CharterFlight(nextCity);
                _textManager.PrintPlayerMoved(currentPlayer, nextCity);
            }
            else
            {
                throw new IllegalMoveException($"You need to have the City Card for your current city, {currentPlayer.CurrentCity} in your hand in order to charter a flight from there.");
            }
        }

        void ShuttleFlight(Role currentPlayer)
        {
            //SHUTTLE FLIGHT - Move from a city with a research station to any other city that has a research station
            List<City> researchStations = _state.GetCitiesWithResearchStation();
            if (!currentPlayer.CurrentCity.HasResearchStation)
            {
                throw new IllegalMoveException("There has to be a research station in the city you are in for you to shuttle a flight.");
            }
            else if (researchStations.Count == 1)
            {
                throw new IllegalMoveException("There needs to be a research station in both the city someone is moving from and the city they are moving to, for you to shuttle a flight. \nCurrently there's only one research station in the game...");
            }
            else
            {
                int choice = _textManager.ChooseItemFromList(researchStations, "go to");
                City nextCity = researchStations[choice];
                currentPlayer.ShuttleFlight(nextCity);
                _textManager.PrintPlayerMoved(currentPlayer, nextCity);
            }
        }

        void BuildResearchStation(Role currentPlayer)
        {
            //BUILD A RESEARCH STATION - Discard the city card that _matches_ the city that you are in to place a research station there
            currentPlayer.BuildResearchStation();
            _textManager.PrintResearchStationBuilt(currentPlayer);
        }

        void TreatDisease(Role currentPlayer)
        {
            StateManager state = currentPlayer.state;
            //TREAT DISEASE - Remove one disease cube from the city you are in. If this color is cured remove all cubes _of that color_ from the city
            if (!currentPlayer.CurrentCity.MultipleDiseases)
            {
                currentPlayer.TreatDisease(currentPlayer.CurrentCity.Color);
                _textManager.PrintDiseaseTreated(currentPlayer, currentPlayer.CurrentCity, currentPlayer.CurrentCity.Color, state);

            }
            else
            {
                List<string> printLines = new List<string>();
                List<Colors> diseaseColors = new List<Colors>();

                string screenText;
                int numberOfColors = currentPlayer.CurrentCity.DiseaseCubes.Values.Count;
                for (int i = 1; i <= numberOfColors; i++)
                {
                    if (currentPlayer.CurrentCity.DiseaseCubes[(Colors)i] != 0)
                    {
                        diseaseColors.Add((Colors)i);
                        screenText = $"{(Colors)i}: {currentPlayer.CurrentCity.DiseaseCubes[(Colors)i]} disease cubes";
                        printLines.Add(screenText);
                    }
                }

                int choice = _textManager.ChooseItemFromList(printLines, "cure");
                currentPlayer.TreatDisease(diseaseColors[choice]);
                _textManager.PrintDiseaseTreated(currentPlayer, currentPlayer.CurrentCity, diseaseColors[choice], state);
            }
        }

        void DiscoverCure(Role currentPlayer)
        {
            //DISCOVER A CURE - At _any_ research station, discard 5 city cards of the same disease color to cure that disease
            if (!currentPlayer.CurrentCity.HasResearchStation)
            {
                throw new IllegalMoveException($"There's no research station in {currentPlayer.CurrentCity}. There has to be a research station in your location for you to discover a cure.");
            }
            else
            {
                currentPlayer.DiscoverCure();
                _textManager.PrintCureDiscovered(currentPlayer);
            }
        }

        void ShareKnowledge(Role currentPlayer)
        {
            //SHARE KNOWLEDGE - Either: _give_ the card that _matches_ the city that you are in to another player, or _take_ that card from another player. The other player must also be in the city with you
            List<Role> playersInCity = new List<Role>();
            foreach (Role currentOtherPlayer in _state.Roles)
            {
                if (currentOtherPlayer.CurrentCity == currentPlayer.CurrentCity)
                {
                    if (currentOtherPlayer != currentPlayer)
                    {
                        playersInCity.Add(currentOtherPlayer);
                    }
                }
            }

            Role otherPlayer;
            if (playersInCity.Count == 0)
            {
                throw new IllegalMoveException($"There are no other players in {currentPlayer.CurrentCity} for you to share knowledge with.");
            }
            else if (playersInCity.Count == 1)
            {
                otherPlayer = playersInCity[0];
            }
            else
            {
                int choice = _textManager.ChooseItemFromList(playersInCity, "share knowledge with");
                otherPlayer = playersInCity[choice];
            }

            currentPlayer.ShareKnowledge(otherPlayer);
            _textManager.PrintKnowledgeShared(currentPlayer, otherPlayer);
        }

        void PlayEvent(Role currentPlayer)
        {
            throw new NotImplementedException("Play Event has not been implemented");
        }

        void FirstSpecialAbility(Role currentPlayer)
        {
            //First special ability
            currentPlayer.PlayFirstSpecialAbility();
            _textManager.PrintUsedSpecialAbility(currentPlayer);

        }

        void SecondSpecialAbility(Role currentPlayer)
        { 
            //Second special ability
            currentPlayer.PlaySecondSpecialAbility();
            _textManager.PrintUsedSpecialAbility(currentPlayer);
        }


        //Unit test methods
        public void TestDriveFerry(Role currentRole) { DriveFerry(currentRole); }
        public void TestDirectFlight(Role currentRole) { DirectFlight(currentRole); }
        public void TestCharterFlight(Role currentRole) { CharterFlight(currentRole); }
        public void TestShuttleFlight(Role currentRole) { ShuttleFlight(currentRole); }
        public void TestBuilldResearchStation(Role currentRole) { BuildResearchStation(currentRole); }
        public void TestTreatDisease(Role currentRole) { TreatDisease(currentRole); }
        public void TestDiscoverCure(Role currentRole) { DiscoverCure(currentRole); }
        public void TestShareKnowledge(Role currentRole) { ShareKnowledge(currentRole); }
        public void TestPlayEvent(Role currentRole) { PlayEvent(currentRole); }
        public void TestFirstSpecialAbility(Role currentRole) { FirstSpecialAbility(currentRole); }
        public void TestSecondSpecialAbility(Role currentRole) { SecondSpecialAbility(currentRole); }
        public void TestPlayGame() { PlayGame(); }
    }
}
