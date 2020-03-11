using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Pandemic.Cards;
using Pandemic.Game;
using Pandemic.Cards.EventCards;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using System.Reflection;

namespace Pandemic.Managers
{
    public class StateManager
    {
        readonly string citiesFilePath;
        readonly string startingCityName;

        //Constants to be set upon initialization
        public int NumberOfPlayers { get; private set; }
        public int NumberOfEpidemics { get; private set; }
        public bool RandomRoles { get; private set; }
        public bool QuarantineSpecialistInGame { get; private set; }
        public bool MedicInGame { get; private set; }
        public List<Role> Roles = new List<Role>();
        public Dictionary<string, City> Cities = new Dictionary<string, City>();
        public int MaxPlayerActions;

        //Global game constants
        public int[] InfectionRates = new int[] { 2, 2, 2, 3, 3, 4, 4 };
        public int InfectionIndex { get; private set; }
        public int MaxOutbreaks { get; private set; }
        public int MaxCubesInCubePool { get; private set; }

        //States
        public int RemainingResearchStations { get; private set; }
        public int Outbreaks;
        public Dictionary<Colors, int> CubePools;
        public Dictionary<Colors, bool> Cures = new Dictionary<Colors, bool>
        {
            {Colors.Yellow, false },
            {Colors.Red, false },
            {Colors.Blue, false },
            {Colors.Black, false }
        };

        public List<City> OutbreakThisChain = new List<City>();

        //Decks
        public InfectionDeck InfectionDeck = new InfectionDeck();
        public InfectionDeck InfectionDiscard = new InfectionDeck();

        public PlayerDeck PlayerDeck = new PlayerDeck();
        public PlayerDeck PlayerDiscard = new PlayerDeck();

        public StateManager(
            string citiesFilePath = "Pandemic.Managers.Cities.txt",
            string startingCityName = "Atlanta",
            int numberOfEpidemics = 5,
            bool randomRoles = true,
            bool quarantineSpecialistInGame = false,
            bool medicInGame = false,
            List<Role> roles = null,
            int infectionIndex = 0,
            int maxOutbreaks = 8,
            int maxCubesInCubePool = 24,
            int remainingResearchStations = 6,
            int outbreaks = 0,
            int maxPlayerActions = 4,

            Dictionary<string, City> cities = null,
            Dictionary<Colors, bool> cures = null,
            Dictionary<Colors, int> cubePools = null,
            List<City> outbreakThisChain = null,
            InfectionDeck infectionDeck = null,
            InfectionDeck infectionDiscard = null,
            PlayerDeck playerDeck = null,
            PlayerDeck playerDiscard = null,
            bool testing = false,
            List<User> users = null, 
            ITextManager textManager = null
            )
        {

            this.citiesFilePath = citiesFilePath;
            this.startingCityName = startingCityName;
            this.NumberOfEpidemics = numberOfEpidemics;
            this.RandomRoles = randomRoles;
            this.QuarantineSpecialistInGame = quarantineSpecialistInGame;
            this.MedicInGame = medicInGame;
            if (roles != null) { this.Roles = roles; NumberOfPlayers = this.Roles.Count; }
            this.InfectionIndex = infectionIndex;
            this.MaxOutbreaks = maxOutbreaks;
            this.MaxCubesInCubePool = maxCubesInCubePool;
            this.RemainingResearchStations = remainingResearchStations;
            this.Outbreaks = outbreaks;
            this.MaxPlayerActions = maxPlayerActions;
            if (cities != null) { this.Cities = cities; }
            if (cures != null) { this.Cures = cures; }
            if (outbreakThisChain != null) { this.OutbreakThisChain = outbreakThisChain; }
            if (infectionDeck != null) { this.InfectionDeck = infectionDeck; }
            if (infectionDiscard != null) { this.InfectionDiscard = infectionDiscard; }
            if (playerDeck != null) { this.PlayerDeck = playerDeck; }
            if (playerDiscard != null) { this.PlayerDiscard = playerDiscard; }
            if (users != null) { NumberOfPlayers = users.Count; }

            if (cubePools != null)
            {
                this.CubePools = cubePools;
            }
            else
            {
                this.CubePools = new Dictionary<Colors, int>
                {
                    {Colors.Yellow, maxCubesInCubePool },
                    {Colors.Red, maxCubesInCubePool },
                    {Colors.Blue, maxCubesInCubePool },
                    {Colors.Black, maxCubesInCubePool }
                };
            }

            if(!testing)
            {
                Setup(users, textManager);
            } 
        }

        //Setup methods
        void Setup(List<User> users, ITextManager textManager)
        {
            List<string> InputStrings = GetInput();
            CleanInput(InputStrings);

            //City Setup
            Cities = CreateCities(InputStrings, textManager);
            AddConnectedCities(InputStrings);

            //Creating Decks
            AddCitiesToDecks();
            AddEventsToPlayerDeck(textManager);
            InfectionDeck.Shuffle();
            PlayerDeck.Shuffle();

            //Add Player Roles
            City StartingCity = Cities[startingCityName];
            StartingCity.HasResearchStation = true;
            RemainingResearchStations--;
            AssignPlayerRoles(users, textManager);
            DealPlayerCards();
   
            //Initial Infection
            InitialInfection();
            AddEpidemicsToDeck();
        }

        List<string> GetInput(Stream citiesResource = null)
        {
            if (citiesResource == null)
            {
                citiesResource = Assembly.GetExecutingAssembly().GetManifestResourceStream(citiesFilePath);
            }

            List<string> InputStrings = new List<string>();
            try
            {
                StreamReader stream = new StreamReader(citiesResource);
                using (stream)
                {
                    string Line;
                    while(stream.Peek() > 0)
                    {
                        Line = stream.ReadLine();
                        InputStrings.Add(Line);
                    }
                }
            } catch (Exception ex)
            {
                throw new UnexpectedBehaviourException("There was an error in reading the file of cities... Error in GetInput of StateManager", ex);
            }

            if (InputStrings.Count == 0)
            {
                throw new UnexpectedBehaviourException("The input file is empty. Please check the file and its path. Error in GetInput of StateManager");
            } else
            {
                return InputStrings;
            }
        }

        void CleanInput(List<string> inputStrings)
        {
            List<int> RowsToRemove = new List<int>();
            int Counter = 0;
            foreach(string CurrentString in inputStrings)
            {
                bool IsComment = CurrentString.StartsWith('*');
                bool IsEmpty = string.IsNullOrWhiteSpace(CurrentString);
                bool IsNewColor = CurrentString.StartsWith('/');
                if(IsComment || IsEmpty)
                {
                    RowsToRemove.Add(Counter);
                }
                else
                {
                    if (!IsNewColor)
                    {
                        List<string> CityNames = new List<string>();
                        string[] SplitLine = CurrentString.Split(" - ");
                        string CityName = SplitLine[0];
                        string[] ConnectedCities = SplitLine[1].Split(", ");
                        CityNames.Add(CityName);
                        CityNames.AddRange(ConnectedCities);

                        //Checks that the CityName begins with a capital letter and is followed by a lower case letter
                        Regex CheckString = new Regex(@"^([A-Z])(?=[a-z])"); 
                        foreach (string CurrentName in CityNames)
                        {
                            if (!CheckString.IsMatch(CurrentName))
                            {
                                throw new UnexpectedBehaviourException($"{CurrentName} is not a valid City Name, it must start with a capital and lower case letter. Error in CleanInput in StateManager");
                            }
                        }
                    }
                }
                Counter++;
            }

            for (int i = RowsToRemove.Count-1; i>=0; i--)
            {
                inputStrings.RemoveAt(RowsToRemove[i]);
                
                if (inputStrings.Count == 0)
                {
                    throw new UnexpectedBehaviourException("There are no valid Strings of Cities and connected cities in your input file. Error in CleanInput in StateManager");
                }
            }
        }

        Dictionary<string, City> CreateCities(List<string> inputStrings, ITextManager textManager)
        {
            Dictionary<string, City> cities = new Dictionary<string, City>();

            Colors currentColor = Colors.None;
            foreach(string currentString in inputStrings)
            {
               bool isNewColor = currentString.StartsWith('/');
               if (isNewColor)
                {
                    currentColor++;
                }
                else
                {
                    string[] SplitLine = currentString.Split(" - ");
                    string cityName = SplitLine[0];

                    City newCity = new City(cityName, currentColor, this, textManager);
                    cities.Add(cityName, newCity);
                }
            }
            return cities;
        }

        void AddConnectedCities(List<string> inputStrings)
        {
            foreach (String currentString in inputStrings)
            {
                bool isNewColor = currentString.StartsWith('/');
                if (!isNewColor)
                {
                    string[] splitLine = currentString.Split(" - ");

                    string cityName = splitLine[0];
                    City currentCity = Cities[cityName];
                    
                    string connectedCitiesString = splitLine[1];
                    string[] connectedCities = connectedCitiesString.Split(", ");

                    foreach(String currentConnectedCity in connectedCities)
                    {
                        try
                        {
                            City newConnectedCity = Cities[currentConnectedCity];
                            currentCity.ConnectedCities.Add(newConnectedCity);
                        }
                        catch (Exception ex)
                        {
                            throw new UnexpectedBehaviourException($"A connected city, {currentConnectedCity}, was not found to attach to {cityName} in the list of cities in AddConnectedCities in StateManager. Please check the Input list for errors.", ex);
                        }
                    }
                }
            }
        }

        void AddCitiesToDecks()
        {
            foreach(City currentCity in Cities.Values)
            {
                string currentName = currentCity.Name;
                Colors currentColor = currentCity.Color;

                CityCard cityCard = new CityCard(currentName, currentColor, this);
                PlayerDeck.AddCard(cityCard);

                InfectionCard infectionCard = new InfectionCard(currentName, currentColor, this);
                InfectionDeck.AddCard(infectionCard);
            }
        }

        void AddEventsToPlayerDeck(ITextManager textManager)
        {
            List<PlayerCard> EventCards = new List<PlayerCard>
            {
                new Airlift(this, textManager),
                new Forecast(this, textManager),
                new GovernmentGrant(this, textManager),
                new OneQuietNight(this, textManager),
                new ResilientPopulation(this, textManager)
            };

            PlayerDeck.AddCards(EventCards);
        }

        void AssignPlayerRoles(List<User> users, ITextManager textManager)
        {
            City StartingCity = Cities[startingCityName];
            if (StartingCity == null)
            {
                throw new UnexpectedBehaviourException("Unable to find the starting city in the cities list in AssignPlayerRoles in StateManager.");
            } else
            {
                List<int> AvailableRoleIndexes = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
                Random rnd = new Random();
                int CurrentRoleIndex;
                foreach (User CurrentUser in users)
                {
                    CurrentRoleIndex = AvailableRoleIndexes[rnd.Next(AvailableRoleIndexes.Count)];
                    Role CurrentRole = null;
                    switch (CurrentRoleIndex)
                    {
                        case 0:
                            CurrentRole = new ContingencyPlanner(StartingCity, CurrentUser.UserID, this, textManager);
                            Roles.Add(CurrentRole);
                            break;
                        case 1:
                            CurrentRole = new Dispatcher(StartingCity, CurrentUser.UserID, this, textManager);
                            Roles.Add(CurrentRole);
                            break;
                        case 2:
                            CurrentRole = new Medic(StartingCity, CurrentUser.UserID, this, textManager);
                            Roles.Add(CurrentRole);
                            break;
                        case 3:
                            CurrentRole = new OperationsExpert(StartingCity, CurrentUser.UserID, this, textManager);
                            Roles.Add(CurrentRole);
                            break;
                        case 4:
                            CurrentRole = new QuarantineSpecialist(StartingCity, CurrentUser.UserID, this, textManager);
                            Roles.Add(CurrentRole);
                            break;
                        case 5:
                            CurrentRole = new Researcher(StartingCity, CurrentUser.UserID, this, textManager);
                            Roles.Add(CurrentRole);
                            break;
                        case 6:
                            CurrentRole = new Scientist(StartingCity, CurrentUser.UserID, this, textManager);
                            Roles.Add(CurrentRole);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"An error occured in AssignPlayerRoles method of State Manager. {CurrentRoleIndex} is not a vaild index for the switch");
                    }
                    CurrentUser.CurrentRole = CurrentRole;
                    AvailableRoleIndexes.Remove(CurrentRoleIndex);
                }
            }
        }
        
        void DealPlayerCards()
        {
            if (PlayerDeck.Count() == 0)
            {
                throw new UnexpectedBehaviourException("No Player Deck has not been generated. The DealPlayerCards method in StateManager could not deal any cards.");
            } else
            {
                int NumberOfCardsToStartWith;

                switch (NumberOfPlayers)
                {
                    case 4:
                        NumberOfCardsToStartWith = 2;
                        break;
                    case 3:
                        NumberOfCardsToStartWith = 3;
                        break;
                    case 2:
                        NumberOfCardsToStartWith = 4;
                        break;
                    default:
                        throw new UnexpectedBehaviourException($"Invalid number of Players, {NumberOfPlayers}, in DealPlayerCards in StateManager");
                }

                if(PlayerDeck.Count() < (NumberOfCardsToStartWith * NumberOfPlayers))
                {
                    throw new UnexpectedBehaviourException("There weren't enough cards in the Player Deck to deal to the participating players in the DealPlayerCards method in StateManager. Why?");
                } else
                {
                    foreach (Role CurrentRole in Roles)
                    {
                        CurrentRole.Draw(PlayerDeck, NumberOfCardsToStartWith);
                    }
                }
            }
        }

        void AddEpidemicsToDeck()
        {
            if(PlayerDeck.Count() == 0)
            {
                throw new UnexpectedBehaviourException("There was no player deck created when AddEpidemicsToDeck in StateManager was run.");
            } else
            {
                int CardsInEachPile = PlayerDeck.Count() / NumberOfEpidemics;
                int Remainder = PlayerDeck.Count() % NumberOfEpidemics;

                PlayerDeck currentPile = new PlayerDeck();
                for (int i=0; i<NumberOfEpidemics; i++)
                {
                    int CardsInCurrentPile = CardsInEachPile;
                    if (Remainder > 0)
                    {
                        CardsInCurrentPile++;
                        Remainder--;
                    }

                    currentPile.AddCards(PlayerDeck.Draw(CardsInCurrentPile));
                    currentPile.AddCard(new EpidemicCard(this));
                    currentPile.Shuffle();

                    PlayerDeck.CombineDecks(currentPile);
                    currentPile.Clear();
                }
            }
        }

        void InitialInfection()
        {
            if (InfectionDeck.Count() == 0)
            {
                throw new UnexpectedBehaviourException("There was no infection deck created for InitialInfection in StateManager to draw cards from.");
            } else if (Cities.Count == 0)
            {
                throw new UnexpectedBehaviourException("There are no cities for InitialInfection in StateManager to infect!");
            } else
            {
                for (int i = 1; i<4; i++)
                {
                    for (int j = 0; j<3; j++)
                    {
                        InfectionCard CurrentCard = InfectionDeck.Draw();
                        InfectionDiscard.AddCard(CurrentCard);
                        City CurrentCity = Cities[CurrentCard.Name];
                        CurrentCity.DiseaseCubes[CurrentCard.Color] = i;
                        CubePools[CurrentCard.Color] -= i;
                    }
                }
            }
        }


        //public methods
        public City GetCity(Card cityToFind)
        {
            try
            {
                City foundCity = Cities[cityToFind.Name];
                return foundCity;
            }
            catch (Exception ex)
            {
                throw new UnexpectedBehaviourException($"{cityToFind.Name} wasn't found on the board in GetCity of StateManager.", ex);
            }
        }

        public City GetCity(int index)
        {
            if(index >= 0 && index < Cities.Values.Count)
            {
                int counter = 0;
                foreach (City currentCity in Cities.Values)
                {
                    if (counter == index)
                    {
                        return currentCity;
                    }
                    counter++;
                }
            } else
            {
                throw new UnexpectedBehaviourException("Could not retrieve requested city. The index requested was out of bounds.");
            }

            throw new UnexpectedBehaviourException("Something extremely unexpected happeded. To such a degree that this error message should never be displayed. The error was in GetCity(int index) of stateManager.");
        }

        public List<City> GetCitiesWithResearchStation()
        {
            List<City> currentCitiesWithResearchStation = new List<City>();

            foreach (City CurrentCity in Cities.Values)
            {
                if (CurrentCity.HasResearchStation)
                {
                    currentCitiesWithResearchStation.Add(CurrentCity);
                }
            }

            if (currentCitiesWithResearchStation.Count == 0)
            {
                throw new UnexpectedBehaviourException("There are no cities with researchstations in GetCitiesWithResearchStation of StateManager. Something must have gone wrong somewhere.");
            } else
            {
                return currentCitiesWithResearchStation;
            }
        }

        public void BuildResearchStation()
        {
            if (RemainingResearchStations == 0)
            {
                throw new UnexpectedBehaviourException("There weren't any researchstations left for BuildResearchStation in StateManager");
            } else
            {
                RemainingResearchStations--;
            }
        }

        public void IncreaseInfectionRate()
        {
            if (InfectionIndex >= InfectionRates.Length)
            {
                throw new UnexpectedBehaviourException("The InfectionIndex has exceeded it's limit in IncreaseInfectionRate in StateManager");
            } else
            {
                InfectionIndex++;
            }
        }



        //Unit Test methods
        public List<string> TestGetInput(Stream citiesResource)
        {
            return GetInput(citiesResource);
        }

        public void TestCleanInput(List<string> inputStrings)
        {
            CleanInput(inputStrings);
        }

        public Dictionary<string, City> TestCreateCities(List<string> inputStrings)
        {
            TextManager temp = new TextManager();
            return CreateCities(inputStrings, temp);
        }

        public void TestAddConnectedCities(List<String> inputStrings)
        {
            AddConnectedCities(inputStrings);
        }

        public void TestAddCitiesToDecks()
        {
            AddCitiesToDecks();
        }

        public void TestAddEventsToPlayerDeck()
        {
            TextManager temp = new TextManager();
            AddEventsToPlayerDeck(temp);
        }

        public void TestAssignPlayerRoles(List<User> users)
        {
            TextManager temp = new TextManager();
            AssignPlayerRoles(users, temp);
        }

        public void TestDealPlayerCards()
        {
            DealPlayerCards();
        }

        public void TestAddEpidemicsToDeck()
        {
            AddEpidemicsToDeck();
        }

        public void TestInitialInfection()
        {
            InitialInfection();
        }
    }
}