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
        string CitiesFilePath;
        string StartingCityName;

        //Constants to be set upon initialization
        public int NumberOfPlayers { get; private set; }
        public int NumberOfEpidemics { get; private set; }
        public bool RandomRoles { get; private set; }
        public bool QuarantineSpecialistInGame { get; private set; }
        public bool MedicInGame { get; private set; }
        public List<Role> Roles = new List<Role>();
        public List<City> Cities = new List<City>();

        //Global game constants
        public int[] InfectionRates = new int[] { 2, 2, 2, 3, 3, 4, 4 };
        public int InfectionIndex { get; private set; }
        public int MaxOutbreaks { get; private set; }
        public int MaxCubesInCubePool { get; private set; }

        //States
        public int RemainingResearchStations { get; private set; }
        public int CurrentInfectionRateIndex;
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
            string CitiesFilePath = "Pandemic.Managers.Cities.txt",
            string StartingCityName = "Atlanta",
            int NumberOfEpidemics = 5,
            bool RandomRoles = true,
            bool QuarantineSpecialistInGame = false,
            bool MedicInGame = false,
            List<Role> Roles = null,
            int InfectionIndex = 0,
            int MaxOutbreaks = 8,
            int MaxCubesInCubePool = 24,
            int RemainingResearchStations = 6,
            int CurrentInfectionRateIndex = 0,
            int Outbreaks = 0,

            List<City> Cities = null,
            Dictionary<Colors, bool> Cures = null,
            List<City> OutbreakThisChain = null,
            InfectionDeck InfectionDeck = null,
            InfectionDeck InfectionDiscard = null,
            PlayerDeck PlayerDeck = null,
            PlayerDeck PlayerDiscard = null,
            bool Testing = false,
            List<User> Users = null
            )
        {
            this.CitiesFilePath = CitiesFilePath;
            this.StartingCityName = StartingCityName;
            this.NumberOfEpidemics = NumberOfEpidemics;
            this.RandomRoles = RandomRoles;
            this.QuarantineSpecialistInGame = QuarantineSpecialistInGame;
            this.MedicInGame = MedicInGame;
            if (Roles != null) { this.Roles.AddRange(Roles); NumberOfPlayers = this.Roles.Count; }
            this.InfectionIndex = InfectionIndex;
            this.MaxOutbreaks = MaxOutbreaks;
            this.MaxCubesInCubePool = MaxCubesInCubePool;
            this.RemainingResearchStations = RemainingResearchStations;
            this.CurrentInfectionRateIndex = CurrentInfectionRateIndex;
            this.Outbreaks = Outbreaks;
            if (Cities != null) { this.Cities = Cities; }
            if (Cures != null) { this.Cures = Cures; }
            if (OutbreakThisChain != null) { this.OutbreakThisChain = OutbreakThisChain; }
            if (InfectionDeck != null) { this.InfectionDeck = InfectionDeck; }
            if (InfectionDiscard != null) { this.InfectionDiscard = InfectionDiscard; }
            if (PlayerDeck != null) { this.PlayerDeck = PlayerDeck; }
            if (PlayerDiscard != null) { this.PlayerDiscard = PlayerDiscard; }
            if (Users != null) { NumberOfPlayers = Users.Count; }

            CubePools = new Dictionary<Colors, int>
            {
                {Colors.Yellow, MaxCubesInCubePool },
                {Colors.Red, MaxCubesInCubePool },
                {Colors.Blue, MaxCubesInCubePool },
                {Colors.Black, MaxCubesInCubePool }
            };
            
            if (!Testing)
            {
                Setup(Users);
            } 
        }

        //Setup methods
        void Setup(List<User> Users)
        {
            List<string> InputStrings = GetInput();
            CleanInput(InputStrings);

            //City Setup
            Cities = CreateCities(InputStrings);
            AddConnectedCities(InputStrings);

            //Creating Decks
            AddCitiesToDecks();
            AddEventsToPlayerDeck();
            InfectionDeck.Shuffle();
            PlayerDeck.Shuffle();

            //Add Player Roles
            City StartingCity = Cities.Find(City => City.Name == StartingCityName);
            StartingCity.ResearchStation = true;
            RemainingResearchStations--;
            AssignPlayerRoles(Users);
            DealPlayerCards();
   
            //Initial Infection
            InitialInfection();
            AddEpidemicsToDeck();
        }

        List<string> GetInput(Stream CitiesResource = null)
        {
            if (CitiesResource == null)
            {
                CitiesResource = Assembly.GetExecutingAssembly().GetManifestResourceStream(CitiesFilePath);
            }

            List<string> InputStrings = new List<string>();
            try
            {
                StreamReader stream = new StreamReader(CitiesResource);
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

        void CleanInput(List<string> InputStrings)
        {
            List<int> RowsToRemove = new List<int>();
            int Counter = 0;
            foreach(string CurrentString in InputStrings)
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
                InputStrings.RemoveAt(RowsToRemove[i]);
                
                if (InputStrings.Count == 0)
                {
                    throw new UnexpectedBehaviourException("There are no valid Strings of Cities and connected cities in your input file. Error in CleanInput in StateManager");
                }
            }
        }

        List<City> CreateCities(List<string> InputStrings)
        {
            List<City> TempCities = new List<City>();

            Colors CurrentColor = Colors.None;
            foreach(string CurrentString in InputStrings)
            {
               bool IsNewColor = CurrentString.StartsWith('/');
               if (IsNewColor)
                {
                    CurrentColor++;
                }
                else
                {
                    string[] SplitLine = CurrentString.Split(" - ");
                    string CityName = SplitLine[0];

                    City NewCity = new City(CityName, CurrentColor);
                    TempCities.Add(NewCity);
                }
            }
            return TempCities;
        }

        void AddConnectedCities(List<string> InputStrings)
        {
            foreach (String CurrentString in InputStrings)
            {
                bool IsNewColor = CurrentString.StartsWith('/');
                if (!IsNewColor)
                {
                    string[] SplitLine = CurrentString.Split(" - ");

                    string CityName = SplitLine[0];
                    City CurrentCity = Cities.Find(City => City.Name == CityName);
                    
                    string ConnectedCitiesString = SplitLine[1];
                    string[] ConnectedCities = ConnectedCitiesString.Split(", ");

                    foreach(String CurrentConnectedCity in ConnectedCities)
                    {
                        City TempCity = Cities.Find(City => City.Name == CurrentConnectedCity);
                        if (TempCity == null)
                        {
                            throw new UnexpectedBehaviourException($"A connected city, {CurrentConnectedCity}, was not found to attach to {CityName} in the list of cities in AddConnectedCities in StateManager. Please check the Input list for errors.");
                        } 
                        else
                        {
                            CurrentCity.ConnectedCities.Add(TempCity);
                        }
                    }
                }
            }
        }

        void AddCitiesToDecks()
        {
            foreach(City CurrentCity in Cities)
            {
                string CurrentName = CurrentCity.Name;
                Colors CurrentColor = CurrentCity.Color;

                CityCard CityCard = new CityCard(CurrentName, CurrentColor);
                PlayerDeck.AddCard(CityCard);

                InfectionCard InfectionCard = new InfectionCard(CurrentName, CurrentColor);
                InfectionDeck.AddCard(InfectionCard);
            }
        }

        void AddEventsToPlayerDeck()
        {
            List<Card> EventCards = new List<Card>
            {
                new Airlift(),
                new Forecast(),
                new GovernmentGrant(),
                new OneQuietNight(),
                new ResilientPopulation()
            };

            PlayerDeck.AddCards(EventCards);
        }

        void AssignPlayerRoles(List<User> Users)
        {
            City StartingCity = Cities.Find(City => City.Name == StartingCityName);
            if (StartingCity == null)
            {
                throw new UnexpectedBehaviourException("Unable to find the starting city in the cities list in AssignPlayerRoles in StateManager.");
            } else
            {
                List<int> AvailableRoleIndexes = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
                Random rnd = new Random();
                int CurrentRoleIndex = -1;
                foreach (User CurrentUser in Users)
                {
                    CurrentRoleIndex = AvailableRoleIndexes[rnd.Next(AvailableRoleIndexes.Count)];
                    Role CurrentRole = null;
                    switch (CurrentRoleIndex)
                    {
                        case 0:
                            CurrentRole = new ContingencyPlanner(StartingCity, CurrentUser.UserID);
                            Roles.Add(CurrentRole);
                            break;
                        case 1:
                            CurrentRole = new Dispatcher(StartingCity, CurrentUser.UserID);
                            Roles.Add(CurrentRole);
                            break;
                        case 2:
                            CurrentRole = new Medic(StartingCity, CurrentUser.UserID);
                            Roles.Add(CurrentRole);
                            break;
                        case 3:
                            CurrentRole = new OperationsExpert(StartingCity, CurrentUser.UserID);
                            Roles.Add(CurrentRole);
                            break;
                        case 4:
                            CurrentRole = new QuarantineSpecialist(StartingCity, CurrentUser.UserID);
                            Roles.Add(CurrentRole);
                            break;
                        case 5:
                            CurrentRole = new Researcher(StartingCity, CurrentUser.UserID);
                            Roles.Add(CurrentRole);
                            break;
                        case 6:
                            CurrentRole = new Scientist(StartingCity, CurrentUser.UserID);
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
                    currentPile.AddCard(new EpidemicCard());
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
                        Card CurrentCard = InfectionDeck.Draw();
                        InfectionDiscard.AddCard(CurrentCard as InfectionCard);
                        City CurrentCity = Cities.Find(City => City.Name == CurrentCard.Name);
                        CurrentCity.DiseaseCubes[CurrentCard.Color] = i;
                        CubePools[CurrentCard.Color] -= i;
                    }
                }
            }

        }


        //public methods
        public City GetCity(Card CityToFind)
        {
            City TempCity = Cities.Find(City => City.Name == CityToFind.Name);
            if (TempCity == null)
            {
                throw new UnexpectedBehaviourException($"{CityToFind.Name} wasn't found on the board in GetCity of StateManager.");
            } else
            {
                return TempCity;
            }
        }

        public List<City> GetCitiesWithResearchStation()
        {
            List<City> TempCities = new List<City>();

            foreach (City CurrentCity in Cities)
            {
                if (CurrentCity.ResearchStation)
                {
                    TempCities.Add(CurrentCity);
                }
            }

            if (TempCities.Count == 0)
            {
                throw new UnexpectedBehaviourException("There are no cities with researchstations in GetCitiesWithResearchStation of StateManager. Something must have gone wrong somewhere.");
            } else
            {
                return TempCities;
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
        public List<string> TestGetInput(Stream CitiesResource)
        {
            return GetInput(CitiesResource);
        }

        public void TestCleanInput(List<string> InputStrings)
        {
            CleanInput(InputStrings);
        }

        public List<City> TestCreateCities(List<string> InputStrings)
        {
            return CreateCities(InputStrings);
        }

        public void TestAddConnectedCities(List<String> InputStrings)
        {
            AddConnectedCities(InputStrings);
        }

        public void TestAddCitiesToDecks()
        {
            AddCitiesToDecks();
        }

        public void TestAddEventsToPlayerDeck()
        {
            AddEventsToPlayerDeck();
        }

        public void TestAssignPlayerRoles(List<User> Users)
        {
            AssignPlayerRoles(Users);
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
