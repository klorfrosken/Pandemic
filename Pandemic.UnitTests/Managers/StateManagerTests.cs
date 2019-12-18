using Xunit;
using Pandemic.Managers;
using Pandemic.Game;
using System.Collections.Generic;
using System;
using System.Linq;
using Pandemic.Cards;
using Pandemic.Cards.EventCards;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using System.IO;
using System.Reflection;

namespace Pandemic.UnitTests.Managers
{
    public class StateManagerTests
    {
        [Fact]
        public void StateManager_TestRoleIsNotNull_Succeeds()
        {
            StateManager State = new StateManager(
                Testing: true, 
                Roles: new List<Role>
                {
                    new Medic(new City("Atlanta", Colors.Blue), 0)
                });

            Boolean RolesAdded = State.Roles.Exists(Role => Role is Medic);
            Boolean NumberOfPlayersSet = State.NumberOfPlayers == 1;

            Assert.True(RolesAdded && NumberOfPlayersSet);
        }

        [Fact]
        public void StateManager_CitiesNotNull_Succeeds()
        {
            StateManager State = new StateManager(
                Testing: true,
                Cities: new List<City> { new City("Atlanta", Colors.Blue) }
                );

            Boolean CitiesAdded = State.Cities.Exists(City => City.Name.Equals("Atlanta"));

            Assert.True(CitiesAdded);
        }

        [Fact]
        public void StateManager_CuresNotNull_Succeeds()
        {
            Dictionary<Colors, bool> InitialCures = new Dictionary<Colors, bool>
            {
                { Colors.Yellow, true },
                { Colors.Red, false },
                { Colors.Blue, true },
                { Colors.Black, false }
            };

            StateManager State = new StateManager(
                Testing: true,
                Cures: InitialCures
                );

            Boolean CuresCorrect = State.Cures.Equals(InitialCures);

            Assert.True(CuresCorrect);
        }

        [Fact]
        public void StateManager_OutbreakThisChainNotNull_Succeeds()
        {
            StateManager State = new StateManager(
                Testing: true,
                OutbreakThisChain: new List<City> { new City("Atlanta", Colors.Blue) }
                );

            Boolean OutbreakCitiesAdded = State.OutbreakThisChain.Exists(City => City.Name.Equals("Atlanta"));

            Assert.True(OutbreakCitiesAdded);
        }

        [Fact]
        public void StateManager_InfectionDeckNotNull_Succeeds()
        {
            StateManager State = new StateManager(
                Testing: true,
                InfectionDeck: new InfectionDeck(new List<InfectionCard>
                {
                    new InfectionCard("Atlanta", Colors.Blue)
                })
                );

            Boolean InfectionDeckAdded = State.InfectionDeck.Contains("Atlanta");

            Assert.True(InfectionDeckAdded);
        }

        [Fact]
        public void StateManager_InfectionDiscardNotNull_Succeeds()
        {
            StateManager State = new StateManager(
                Testing: true,
                InfectionDiscard: new InfectionDeck(new List<InfectionCard>
                {
                    new InfectionCard("Atlanta", Colors.Blue)
                })
                );

            Boolean InfectionDiscardAdded = State.InfectionDiscard.Contains("Atlanta");

            Assert.True(InfectionDiscardAdded);
        }

        [Fact]
        public void StateManager_PlayerDeckNotNull_Succeeds()
        {
            StateManager State = new StateManager(
                Testing: true,
                PlayerDeck: new PlayerDeck(new List<Card>
                {
                    new CityCard("Atlanta", Colors.Blue)
                })
                );

            Boolean CityDeckAdded = State.PlayerDeck.Contains("Atlanta");

            Assert.True(CityDeckAdded);
        }

        [Fact]
        public void StateManager_PlayerDiscardNotNull_Succeeds()
        {
            StateManager State = new StateManager(
                Testing: true,
                PlayerDiscard: new PlayerDeck(new List<Card>
                {
                    new CityCard("Atlanta", Colors.Blue)
                })
                );

            Boolean PlayerDiscardAdded = State.PlayerDiscard.Contains("Atlanta");

            Assert.True(PlayerDiscardAdded);
        }

        [Fact]
        public void Statemanager_UsersNotNull_Succeds()
        {
            StateManager State = new StateManager(
                Testing: true,
                Users: new List<User>
                {
                    new User(0, "Testperson 1"),
                    new User(1, "Testperson 2")
                });

            Boolean NumberOfPlayersUpdated = State.NumberOfPlayers == 2;

            Assert.True(NumberOfPlayersUpdated);
        }

        [Fact]
        public void GetInput_FileImportedCorrectly_Succeeds()
        {
            string CitiesFilePath = "Pandemic.UnitTests.Managers.GameManager_TestFile.txt";
            Stream CitiesResource = Assembly.GetExecutingAssembly().GetManifestResourceStream(CitiesFilePath);
            StateManager State = new StateManager(Testing: true);
            
            List<string> ActualStrings = new List<string>
            {
                "*",
                "*",
                "*",
                "*",
                "//",
                "//",
                "",
                "",
                "TestCity1 - ConnCity1, ConnCity2, ConnCity3, ConnCity4",
                "TestCity2 - ConnCity1, ConnCity2, ConnCity3, ConnCity4"
            };

            List<string> InputStrings = State.TestGetInput(CitiesResource);

            Assert.True(InputStrings.SequenceEqual(ActualStrings));
        }

        [Fact]
        public void GetInput_InvalidFilePath_ThorwsException()
        {
            StateManager State = new StateManager(
                Testing: true,
                CitiesFilePath: @"C:\WrongFileName.txt");

            Assert.Throws<UnexpectedBehaviourException>(() => State.TestGetInput(null));
        }

        [Fact]
        public void GetInput_FileIsEmpty_ThorwsException()
        {
            string CitiesFilePath = "Pandemic.UnitTests.Managers.GameManager_EmptyFile.txt";
            Stream CitiesResource = Assembly.GetExecutingAssembly().GetManifestResourceStream(CitiesFilePath);
            StateManager State = new StateManager(
                Testing: true,
                CitiesFilePath: "Pandemic.UnitTests.Managers.GameManager_EmptyFile.txt");

            Assert.Throws<UnexpectedBehaviourException>(() => State.TestGetInput(CitiesResource));
        }

        [Fact]
        public void CleanInput_FileIsCleaned_Succeeds()
        {
            StateManager State = new StateManager(Testing: true);

            List<string> InputStrings = new List<string>
            {
                "*TestComment",
                "//TestNewColor",
                "",
                "         ",
                "TestCity1 - TestConnectedCity1, TestConnectedCity2, TestConnectedCity3",
                "TestCity2 - TestConnectedCity1, TestConnectedCity2",
                "TestCity3 - TestConnectedCity1"
            };

            List<string> CleanedList = new List<string>
            {
                "//TestNewColor",
                "TestCity1 - TestConnectedCity1, TestConnectedCity2, TestConnectedCity3",
                "TestCity2 - TestConnectedCity1, TestConnectedCity2",
                "TestCity3 - TestConnectedCity1"
            };

            State.TestCleanInput(InputStrings);

            Assert.True(InputStrings.SequenceEqual(CleanedList));
        }

        [Theory]
        [InlineData(" TestCity - TestConnectedCity1, TestConnectedCity2")]
        [InlineData("\tTestCity - TestConnectedCity1, TestConnectedCity2")]
        [InlineData("TTestCity - TestConnectedCity1, TestConnectedCity2")]
        [InlineData(";TestCity - TestConnectedCity1, TestConnectedCity2")]
        [InlineData("TestCity - TTestConnectedCity1, TestConnectedCity2")]
        [InlineData("TestCity - TestConnectedCity1, TTestConnectedCity2")]
        [InlineData("TestCity - ")]
        [InlineData("*Only comments or empty strings in file")]
        public void CleanInput_FileIncorrectlyFormatted_ThrowsException(string MisformattedString)
        {
            StateManager State = new StateManager(Testing: true);

            List<string> InputStrings = new List<string>{MisformattedString};

            Assert.Throws<UnexpectedBehaviourException>(() => State.TestCleanInput(InputStrings));
        }

        [Fact]
        public void CreateCities_DifferentColoredCities_Succeeds()
        {
            StateManager State = new StateManager(Testing: true);

            List<string> InputStrings = new List<string>
            {
                "//Yellow",
                "TestCity1 - TestConnectedCity1, TestConnectedCity2, TestConnectedCity3, TestConnectedCity4",
                "TestCity2 - TestConnectedCity1",
                "//Red",
                "TestCity3 - TestConnectedCity1, TestConnectedCity2, TestConnectedCity3, TestConnectedCity4",
                "//Blue",
                "TestCity4 - TestConnectedCity1, TestConnectedCity2, TestConnectedCity3, TestConnectedCity4",
                "//Black",
                "TestCity5 - TestConnectedCity1, TestConnectedCity2, TestConnectedCity3, TestConnectedCity4",

            };
            List<City> ActualCities = new List<City>
            {
                new City("TestCity1", Colors.Yellow),
                new City("TestCity2", Colors.Yellow),
                new City("TestCity3", Colors.Red),
                new City("TestCity4", Colors.Blue),
                new City("TestCity5", Colors.Black)
            };

            List<City> GeneratedCities = State.TestCreateCities(InputStrings);

            Assert.True(GeneratedCities.SequenceEqual(ActualCities));
        }

        [Fact]
        public void AddConnectedCities_CitiesAreAdded_Succeeds()
        {
            City TestCity = new City("Atlanta", Colors.Blue);
            List<City> ConnectedCities = new List<City>
            {
                new City("Chicago", Colors.Blue),
                new City("Washington", Colors.Blue),
                new City("Miami", Colors.Yellow)
            };
            List<City> Cities = new List<City>(ConnectedCities);
            Cities.Add(TestCity);

            StateManager State = new StateManager(
                Testing: true,
                Cities: new List<City>(Cities)
                );
     
            List<string> InputStrings = new List<string> 
            {
                "//Blue",
                "Atlanta - Chicago, Washington, Miami" 
            };

            State.TestAddConnectedCities(InputStrings);

            Assert.True(TestCity.ConnectedCities.SequenceEqual(ConnectedCities));
        }

        [Fact]
        public void AddConnectedCities_ConnectedCityDoesNotExist_ThrowsException()
        {
            StateManager State = new StateManager(
                Testing: true, 
                Cities: new List<City>
                {
                    new City("Atlanta", Colors.Blue),
                    new City("Chicago", Colors.Blue),
                    new City("Washington", Colors.Blue)
                });
       
            List<string> InputStrings = new List<string>
            {
                "//Blue",
                "Atlanta - Chicago, Washington, Miami"
            };

            Assert.Throws<UnexpectedBehaviourException>(() => State.TestAddConnectedCities(InputStrings));
        }

        [Fact]
        public void AddCitiesToDecs_DecksAreCreated_Succeeds()
        {
            StateManager State = new StateManager(
                Testing: true, 
                Cities: new List<City>
                {
                    new City("TestCity1", Colors.Yellow),
                    new City("TestCity2", Colors.Yellow),
                    new City("TestCity3", Colors.Red),
                    new City("TestCity4", Colors.Blue),
                    new City("TestCity5", Colors.Black)
                });

            State.TestAddCitiesToDecks();

            List<Card> CityCards = new List<Card>
            {
                new CityCard("TestCity1", Colors.Yellow),
                new CityCard("TestCity2", Colors.Yellow),
                new CityCard("TestCity3", Colors.Red),
                new CityCard("TestCity4", Colors.Blue),
                new CityCard("TestCity5", Colors.Black)
            };

            List<InfectionCard> InfectionCards = new List<InfectionCard>
            {
                new InfectionCard("TestCity1", Colors.Yellow),
                new InfectionCard("TestCity2", Colors.Yellow),
                new InfectionCard("TestCity3", Colors.Red),
                new InfectionCard("TestCity4", Colors.Blue),
                new InfectionCard("TestCity5", Colors.Black)
            };

            PlayerDeck ActualPlayerDeck = new PlayerDeck(CityCards);

            InfectionDeck ActualInfectionDeck = new InfectionDeck(InfectionCards);

            bool PlayerDeckCorrect = State.PlayerDeck.IsSequenceEqual(ActualPlayerDeck);
            bool InfectionDeckCorrect = State.InfectionDeck.IsSequenceEqual(ActualInfectionDeck);

            Assert.True(PlayerDeckCorrect && InfectionDeckCorrect);
        }

        [Fact]
        public void AddEventsToPlayerDeck_Succeeds()
        {
            StateManager State = new StateManager(Testing: true);

            State.TestAddEventsToPlayerDeck();

            PlayerDeck ActualPlayerDeck = new PlayerDeck(new List<Card>
            {
                new Airlift(),
                new Forecast(), 
                new GovernmentGrant(),
                new OneQuietNight(),
                new ResilientPopulation()
            });

            Assert.True(State.PlayerDeck.IsSequenceEqual(ActualPlayerDeck));
        }

        [Fact]
        public void AssignPlayerRoles_FourPlayersAreAssignedRandomNonEqualRoles()
        {
            StateManager State = new StateManager(
                Testing: true,
                Cities: new List<City> { new City("Atlanta", Colors.Blue) });

            List<User> TestUsers = new List<User>
            {
                new User(0, "Ragna Rekkverk"),
                new User(1, "Frida Frosk"),
                new User(2, "Sandra Salamander"),
                new User(3, "Pelle Parafin")
            };

            State.TestAssignPlayerRoles(TestUsers);

            Boolean DuplicateRoles = false;
            int NumberofRoles = 0;
            for (int i=0; i<TestUsers.Count; i++)
            {
                User currentUser = TestUsers[i];
                if (currentUser.CurrentRole != null)
                {
                    NumberofRoles++;
                } else { break; }

                for(int j=i+1; j<TestUsers.Count; j++)
                {
                    string currentUserRole = currentUser.CurrentRole.RoleName;
                    string otherUserRole = TestUsers[j].CurrentRole.RoleName;
                    if (currentUserRole.Equals(otherUserRole))
                    {
                        DuplicateRoles = true;
                        break;
                    }
                }
            }

            Assert.True(NumberofRoles == 4 && !DuplicateRoles);
        }

        [Fact]
        public void DealPlayerCards_TwoPlayers_GetFourCardsEach()
        {
            City StartingCity = new City("Atlanta", Colors.Blue);
            StateManager State = new StateManager(
                Testing: true,
                Roles: new List<Role> 
                {
                    new Medic(StartingCity, 0),
                    new QuarantineSpecialist(StartingCity, 1)
                },
                PlayerDeck: new PlayerDeck(new List<Card>
                {
                    new CityCard("TestCity1", Colors.Yellow),
                    new CityCard("TestCity2", Colors.Yellow),
                    new CityCard("TestCity3", Colors.Red),
                    new CityCard("TestCity4", Colors.Blue),
                    new CityCard("TestCity5", Colors.Black),
                    new CityCard("TestCity6", Colors.Black),
                    new CityCard("TestCity7", Colors.Black),
                    new CityCard("TestCity8", Colors.Black)
                }));

            State.TestDealPlayerCards();

            Boolean CorrectNumberOfCards = true;
            foreach (Role CurrentRole in State.Roles)
            {
                if (CurrentRole.Hand.Count != 4)
                {
                    CorrectNumberOfCards = false;
                    break;
                }
            }

            Assert.True(CorrectNumberOfCards);
        }

        [Fact]
        public void DealPlayerCards_ThreePlayers_GetThreeCardsEach()
        {
            City StartingCity = new City("Atlanta", Colors.Blue);
            StateManager State = new StateManager(
                Testing: true,
                Roles: new List<Role>
                {
                    new Medic(StartingCity, 0),
                    new QuarantineSpecialist(StartingCity, 1),
                    new Scientist(StartingCity, 2)
                },
                PlayerDeck: new PlayerDeck(new List<Card>
                {
                    new CityCard("TestCity1", Colors.Yellow),
                    new CityCard("TestCity2", Colors.Yellow),
                    new CityCard("TestCity3", Colors.Red),
                    new CityCard("TestCity4", Colors.Blue),
                    new CityCard("TestCity5", Colors.Black),
                    new CityCard("TestCity6", Colors.Black),
                    new CityCard("TestCity7", Colors.Black),
                    new CityCard("TestCity8", Colors.Black),
                    new CityCard("TestCity9", Colors.Black)
                }));

            State.TestDealPlayerCards();

            Boolean CorrectNumberOfCards = true;
            foreach (Role CurrentRole in State.Roles)
            {
                if (CurrentRole.Hand.Count != 3)
                {
                    CorrectNumberOfCards = false;
                    break;
                }
            }

            Assert.True(CorrectNumberOfCards);
        }

        [Fact]
        public void DealPlayerCards_FourPlayers_GetTwoCardsEach()
        {
            City StartingCity = new City("Atlanta", Colors.Blue);
            StateManager State = new StateManager(
                Testing: true,
                Roles: new List<Role>
                {
                    new Medic(StartingCity, 0),
                    new QuarantineSpecialist(StartingCity, 1),
                    new Scientist(StartingCity, 2),
                    new QuarantineSpecialist(StartingCity, 3)
                },
                PlayerDeck: new PlayerDeck(new List<Card>
                {
                    new CityCard("TestCity1", Colors.Yellow),
                    new CityCard("TestCity2", Colors.Yellow),
                    new CityCard("TestCity3", Colors.Red),
                    new CityCard("TestCity4", Colors.Blue),
                    new CityCard("TestCity5", Colors.Black),
                    new CityCard("TestCity6", Colors.Black),
                    new CityCard("TestCity7", Colors.Black),
                    new CityCard("TestCity8", Colors.Black)
                }));

            State.TestDealPlayerCards();

            Boolean CorrectNumberOfCards = true;
            foreach (Role CurrentRole in State.Roles)
            {
                if (CurrentRole.Hand.Count != 2)
                {
                    CorrectNumberOfCards = false;
                    break;
                }
            }

            Assert.True(CorrectNumberOfCards);
        }

        [Fact]
        public void DealPlayerCards_NoPlayerDeck_ThrowsException()
        {
            City StartingCity = new City("Atlanta", Colors.Blue);
            StateManager State = new StateManager(
                Testing: true);

            Assert.Throws<UnexpectedBehaviourException>(() => State.TestDealPlayerCards());
        }

        [Fact]
        public void DealPlayerCards_InvalidNumberOfPlayers_ThrowsException()
        {
            City StartingCity = new City("Atlanta", Colors.Blue);
            StateManager State = new StateManager(
                Testing: true,
                Roles: new List<Role>
                {
                    new Medic(StartingCity, 0),
                    new QuarantineSpecialist(StartingCity, 1),
                    new Scientist(StartingCity, 2),
                    new QuarantineSpecialist(StartingCity, 3),
                    new Researcher(StartingCity, 4)
                },
                PlayerDeck: new PlayerDeck(new List<Card>
                {
                    new CityCard("TestCity1", Colors.Yellow),
                    new CityCard("TestCity2", Colors.Yellow),
                    new CityCard("TestCity3", Colors.Red),
                    new CityCard("TestCity4", Colors.Blue),
                    new CityCard("TestCity5", Colors.Black),
                    new CityCard("TestCity6", Colors.Black),
                    new CityCard("TestCity7", Colors.Black),
                    new CityCard("TestCity8", Colors.Black)
                }));

            Assert.Throws<UnexpectedBehaviourException>(() => State.TestDealPlayerCards());
        }

        [Fact]
        public void DealPlayerCards_InvalidNumberOfCardsInDeck_ThrowsException()
        {
            City StartingCity = new City("Atlanta", Colors.Blue);
            StateManager State = new StateManager(
                Testing: true,
                Roles: new List<Role>
                {
                    new Medic(StartingCity, 0),
                    new QuarantineSpecialist(StartingCity, 1),
                },
                PlayerDeck: new PlayerDeck(new List<Card>
                {
                    new CityCard("TestCity1", Colors.Yellow),
                    new CityCard("TestCity2", Colors.Yellow),

                }));

            Assert.Throws<UnexpectedBehaviourException>(() => State.TestDealPlayerCards());
        }

        [Fact]
        public void AddEpidemicsToDeck_EpidemicsSuccessfullyAdded()
        {
            StateManager State = new StateManager(
                Testing: true,
                NumberOfEpidemics: 2,
                PlayerDeck: new PlayerDeck(new List<Card>
                {
                    new CityCard("TestCity1", Colors.Yellow),
                    new CityCard("TestCity2", Colors.Yellow),
                    new CityCard("TestCity3", Colors.Red),
                    new CityCard("TestCity4", Colors.Blue),
                    new CityCard("TestCity5", Colors.Black),
                    new CityCard("TestCity6", Colors.Black),
                    new CityCard("TestCity7", Colors.Black),
                    new CityCard("TestCity8", Colors.Black),
                    new CityCard("TestCity9", Colors.Black)
                }));

            State.TestAddEpidemicsToDeck();

            int NumberOfEpidemics = 0;
            foreach(Card CurrentCard in State.PlayerDeck)
            {
                if (CurrentCard is EpidemicCard)
                {
                    NumberOfEpidemics++;
                }
            }

            Boolean CorrectCardsArePresentInEachShuffle = true;
            int Counter = 0;
            List<string> FirstPile = new List<string>
            {
                "TestCity1",
                "TestCity2",
                "TestCity3",
                "TestCity4",
                "TestCity5", 
                "Epidemic"
            };
            List<string> SecondPile = new List<string>
            {
                "TestCity6",
                "TestCity7",
                "TestCity8",
                "TestCity9",
                "Epidemic"
            };
            foreach (Card CurrentCard in State.PlayerDeck)
            {
                if(Counter < 6)
                {
                    if (!FirstPile.Remove(CurrentCard.Name)){
                        CorrectCardsArePresentInEachShuffle = false;
                        break;
                    }
                } else
                {
                    if (!SecondPile.Remove(CurrentCard.Name))
                    {
                        CorrectCardsArePresentInEachShuffle = false;
                        break;
                    }
                }
                Counter++;
            }

            Boolean CorrectNumberOfCards = (State.PlayerDeck.Count() == 11);
            Boolean CorrectNumberOfEpidemics = (NumberOfEpidemics == 2);

            Assert.True(CorrectNumberOfCards && CorrectNumberOfEpidemics && CorrectCardsArePresentInEachShuffle);
        }

        [Fact]
        public void AddEpidemicsToDeck_PlayerDeckNotCreated_ThrowsException()
        {
            StateManager State = new StateManager(
                Testing: true,
                NumberOfEpidemics: 2
                );

            Assert.Throws<UnexpectedBehaviourException>(() => State.TestAddEpidemicsToDeck());
        }

        [Fact]
        public void InitialInfection_CitiesAreInfected_Succeeds()
        {
            StateManager State = new StateManager(
                Testing: true,
                InfectionDeck: new InfectionDeck(new List<InfectionCard>
                {
                    new InfectionCard("TestCity1", Colors.Yellow),
                    new InfectionCard("TestCity2", Colors.Yellow),
                    new InfectionCard("TestCity3", Colors.Red),
                    new InfectionCard("TestCity4", Colors.Blue),
                    new InfectionCard("TestCity5", Colors.Black),
                    new InfectionCard("TestCity6", Colors.Black),
                    new InfectionCard("TestCity7", Colors.Black),
                    new InfectionCard("TestCity8", Colors.Black),
                    new InfectionCard("TestCity9", Colors.Black)
                }),
                Cities: new List<City>
                {
                    new City("TestCity1", Colors.Yellow),
                    new City("TestCity2", Colors.Yellow),
                    new City("TestCity3", Colors.Red),
                    new City("TestCity4", Colors.Blue),
                    new City("TestCity5", Colors.Black),
                    new City("TestCity6", Colors.Black),
                    new City("TestCity7", Colors.Black),
                    new City("TestCity8", Colors.Black),
                    new City("TestCity9", Colors.Black)
                });

            State.TestInitialInfection();

            Boolean CubesPlacedCorrectly = true;
            for (int i=1; i<4; i++)
            {
                for (int j=3*i-3; j<3*i; j++)
                {
                    City CurrentCity = State.Cities[j];
                    Colors CurrentColor = CurrentCity.Color;
                    if(CurrentCity.DiseaseCubes[CurrentColor] != i)
                    {
                        CubesPlacedCorrectly = false;
                        break;
                    }
                }
            }

            Assert.True(CubesPlacedCorrectly);
        }

        [Fact]
        public void InitialInfection_NoInfectionDeckCreated_ThrowsException()
        {
            StateManager State = new StateManager(
                Testing: true,
                Cities: new List<City>
                {
                    new City("TestCity1", Colors.Yellow),
                    new City("TestCity2", Colors.Yellow),
                    new City("TestCity3", Colors.Red),
                    new City("TestCity4", Colors.Blue),
                    new City("TestCity5", Colors.Black),
                    new City("TestCity6", Colors.Black),
                    new City("TestCity7", Colors.Black),
                    new City("TestCity8", Colors.Black),
                    new City("TestCity9", Colors.Black)
                });

            Assert.Throws<UnexpectedBehaviourException>(() => State.TestInitialInfection());
        }

        [Fact]
        public void InitialInfection_NoCitiesInState_ThrowsException()
        {
            StateManager State = new StateManager(
                Testing: true,
                InfectionDeck: new InfectionDeck(new List<InfectionCard>
                {
                    new InfectionCard("TestCity1", Colors.Yellow),
                    new InfectionCard("TestCity2", Colors.Yellow),
                    new InfectionCard("TestCity3", Colors.Red),
                    new InfectionCard("TestCity4", Colors.Blue),
                    new InfectionCard("TestCity5", Colors.Black),
                    new InfectionCard("TestCity6", Colors.Black),
                    new InfectionCard("TestCity7", Colors.Black),
                    new InfectionCard("TestCity8", Colors.Black),
                    new InfectionCard("TestCity9", Colors.Black)
                }));

            Assert.Throws<UnexpectedBehaviourException>(() => State.TestInitialInfection());
        }

        [Fact]
        public void GetCity_CityIsFound()
        {
            City City = new City("Atlanta", Colors.Blue);
            StateManager State = new StateManager(
                Testing: true,
                Cities: new List<City>
                {
                    City
                });

            CityCard CardToFind = new CityCard("Atlanta", Colors.Blue);

            City FoundCity = State.GetCity(CardToFind);

            Assert.True(City.Equals(FoundCity));
        }

        [Fact]
        public void GetCity_CityNotFound_ThrowsException()
        {
            StateManager State = new StateManager(Testing: true);

            CityCard CityToFind = new CityCard("Atlanta", Colors.Blue);

            Assert.Throws<UnexpectedBehaviourException>(() => State.GetCity(CityToFind));
        }

        [Fact]
        public void GetCitiesWithResearchStation_CitiesAreReturned_Succeeds()
        {
            List<City> Cities = new List<City>
                {
                    new City("TestCity1", Colors.Yellow),
                    new City("TestCity2", Colors.Yellow),
                    new City("TestCity3", Colors.Red),
                    new City("TestCity4", Colors.Blue)
                };
            StateManager State = new StateManager(
                Testing: true,
                Cities: Cities);

            State.Cities[1].BuildResearchStation();
            State.Cities[2].BuildResearchStation();

            List<City> CitiesWithStations = State.GetCitiesWithResearchStation();

            List<City> ActualCitiesWithStation = new List<City>();
            ActualCitiesWithStation.Add(Cities[1]);
            ActualCitiesWithStation.Add(Cities[2]);

            Assert.True(CitiesWithStations.SequenceEqual(ActualCitiesWithStation));
        }

        [Fact]
        public void GetCitiesWithResearchStation_NoCitiesWithStation_ThrowsException()
        {
            List<City> Cities = new List<City>
                {
                    new City("TestCity1", Colors.Yellow),
                    new City("TestCity2", Colors.Yellow),
                    new City("TestCity3", Colors.Red),
                    new City("TestCity4", Colors.Blue)
                };
            StateManager State = new StateManager(
                Testing: true,
                Cities: Cities);

            Assert.Throws<UnexpectedBehaviourException>(() => State.GetCitiesWithResearchStation());
        }

        [Fact]
        public void BuildResearchStation_Succeeds()
        {
            StateManager State = new StateManager(
                Testing: true,
                RemainingResearchStations: 2);

            State.BuildResearchStation();

            Assert.True(State.RemainingResearchStations == 1);
        }

        [Fact]
        public void BuildResearchStation_NoStationsLeft_ThrowsException()
        {
            StateManager State = new StateManager(
                Testing: true,
                RemainingResearchStations: 0);

            Assert.Throws<UnexpectedBehaviourException>(() => State.BuildResearchStation());
        }

        [Fact]
        public void IncreaseInfectionRate_Succeeds()
        {
            StateManager State = new StateManager(Testing: true, InfectionIndex: 0);

            State.IncreaseInfectionRate();
            Assert.True(State.InfectionIndex == 1);
        }

        [Fact]
        public void IncreaseInfectionRate_IndexIsTooLarge_ThrowsException()
        {
            StateManager State = new StateManager(Testing: true, InfectionIndex: 7);

            Assert.Throws<UnexpectedBehaviourException>(() => State.IncreaseInfectionRate());
        }
    }
}