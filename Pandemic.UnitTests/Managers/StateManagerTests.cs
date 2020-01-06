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
            Role testRole = new Medic(new City("Atlanta", Colors.Blue), 0);
            
            StateManager state = new StateManager(
                Testing: true, 
                Roles: new List<Role>
                {
                    testRole
                });

            Assert.Contains(testRole, state.Roles);
            Assert.Equal(1, state.NumberOfPlayers);
        }

        [Fact]
        public void StateManager_CitiesNotNull_Succeeds()
        {
            string testCityName = "Atlanta";

            StateManager State = new StateManager(
                Testing: true,
                Cities: new Dictionary<string, City> { { testCityName, new City(testCityName, Colors.Blue) } }
                );

            Assert.Contains<string, City>(testCityName, (IDictionary<string, City>)State.Cities);
        }

        [Fact]
        public void StateManager_CuresNotNull_Succeeds()
        {
            Dictionary<Colors, bool> initialCures = new Dictionary<Colors, bool>
                {
                    { Colors.Yellow, true },
                    { Colors.Red, false },
                    { Colors.Blue, true },
                    { Colors.Black, false }
                };

            StateManager state = new StateManager(
                Testing: true,
                Cures: initialCures
                );

            Assert.Equal(initialCures, state.Cures);
        }

        [Fact]
        public void StateManager_OutbreakThisChainNotNull_Succeeds()
        {
            City testCity = new City("Atlanta", Colors.Blue);

            StateManager state = new StateManager(
                Testing: true,
                OutbreakThisChain: new List<City> { testCity }
                );

            Assert.Contains(testCity, state.OutbreakThisChain);
        }

        [Fact]
        public void StateManager_InfectionDeckNotNull_Succeeds()
        {
            InfectionCard testCard = new InfectionCard("Atlanta", Colors.Blue);

            StateManager state = new StateManager(
                Testing: true,
                InfectionDeck: new InfectionDeck(new List<InfectionCard>
                {
                    testCard
                }));

            Assert.True(state.InfectionDeck.Contains(testCard));
        }

        [Fact]
        public void StateManager_InfectionDiscardNotNull_Succeeds()
        {
            InfectionCard testCard = new InfectionCard("Atlanta", Colors.Blue);

            StateManager state = new StateManager(
                Testing: true,
                InfectionDiscard: new InfectionDeck(new List<InfectionCard>
                {
                        testCard
                }));

            Assert.True(state.InfectionDiscard.Contains(testCard));
        }

        [Fact]
        public void StateManager_PlayerDeckNotNull_Succeeds()
        {
            PlayerCard testCard = new PlayerCard("Atlanta", Colors.Blue);

            StateManager state = new StateManager(
                Testing: true,
                PlayerDeck: new PlayerDeck(new List<PlayerCard>
                {
                        testCard
                }));

            Assert.True(state.PlayerDeck.Contains(testCard));
        }

        [Fact]
        public void StateManager_PlayerDiscardNotNull_Succeeds()
        {
            PlayerCard testCard = new PlayerCard("Atlanta", Colors.Blue);

            StateManager state = new StateManager(
                Testing: true,
                PlayerDiscard: new PlayerDeck(new List<PlayerCard>
                {
                        testCard
                }));

            Assert.True(state.PlayerDiscard.Contains(testCard));
        }

        [Fact]
        public void Statemanager_UsersNotNull_Succeds()
        {
            int actualNumberOfPlayers = 2;

            StateManager state = new StateManager(
                Testing: true,
                Users: new List<User>
                {
                        new User(0, "Testperson 1"),
                        new User(1, "Testperson 2")
                });

            Assert.Equal(actualNumberOfPlayers, state.NumberOfPlayers);
        }

        [Fact]
        public void GetInput_FileImportedCorrectly_Succeeds()
        {
            string citiesFilePath = "Pandemic.UnitTests.Managers.GameManager_TestFile.txt";
            Stream citiesResource = Assembly.GetExecutingAssembly().GetManifestResourceStream(citiesFilePath);
            StateManager state = new StateManager(Testing: true);

            List<string> expectedStrings = new List<string>
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

            List<string> actualStrings = state.TestGetInput(citiesResource);

            Assert.True(actualStrings.SequenceEqual(expectedStrings));

            Assert.Collection(actualStrings,
                item => { Assert.Equal(item, expectedStrings[0]); },
                item => { Assert.Equal(item, expectedStrings[1]); },
                item => { Assert.Equal(item, expectedStrings[2]); },
                item => { Assert.Equal(item, expectedStrings[3]); },
                item => { Assert.Equal(item, expectedStrings[4]); },
                item => { Assert.Equal(item, expectedStrings[5]); },
                item => { Assert.Equal(item, expectedStrings[6]); },
                item => { Assert.Equal(item, expectedStrings[7]); },
                item => { Assert.Equal(item, expectedStrings[8]); },
                item => { Assert.Equal(item, expectedStrings[9]); }
                );
        }

        [Fact]
        public void GetInput_InvalidFilePath_ThorwsException()
        {
            StateManager state = new StateManager(
                Testing: true,
                CitiesFilePath: @"C:\WrongFileName.txt");

            Assert.Throws<UnexpectedBehaviourException>(() => state.TestGetInput(null));
        }

        [Fact]
        public void GetInput_FileIsEmpty_ThorwsException()
        {
            string citiesFilePath = "Pandemic.UnitTests.Managers.GameManager_EmptyFile.txt";
            Stream citiesResource = Assembly.GetExecutingAssembly().GetManifestResourceStream(citiesFilePath);
            StateManager state = new StateManager(
                Testing: true,
                CitiesFilePath: "Pandemic.UnitTests.Managers.GameManager_EmptyFile.txt");

            Assert.Throws<UnexpectedBehaviourException>(() => state.TestGetInput(citiesResource));
        }

        [Fact]
        public void CleanInput_FileIsCleaned_Succeeds()
        {
            StateManager state = new StateManager(Testing: true);

            List<string> inputStrings = new List<string>
                {
                    "*TestComment",
                    "//TestNewColor",
                    "",
                    "         ",
                    "TestCity1 - TestConnectedCity1, TestConnectedCity2, TestConnectedCity3",
                    "TestCity2 - TestConnectedCity1, TestConnectedCity2",
                    "TestCity3 - TestConnectedCity1"
                };

            List<string> expectedRestult = new List<string>
                {
                    "//TestNewColor",
                    "TestCity1 - TestConnectedCity1, TestConnectedCity2, TestConnectedCity3",
                    "TestCity2 - TestConnectedCity1, TestConnectedCity2",
                    "TestCity3 - TestConnectedCity1"
                };

            state.TestCleanInput(inputStrings);

            Assert.Collection(inputStrings,
                item => { Assert.Equal(item, expectedRestult[0]); },
                item => { Assert.Equal(item, expectedRestult[1]); },
                item => { Assert.Equal(item, expectedRestult[2]); },
                item => { Assert.Equal(item, expectedRestult[3]); }              
                );
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
        public void CleanInput_FileIncorrectlyFormatted_ThrowsException(string misformattedString)
        {
            StateManager state = new StateManager(Testing: true);

            List<string> inputStrings = new List<string> { misformattedString };

            Assert.Throws<UnexpectedBehaviourException>(() => state.TestCleanInput(inputStrings));
        }

        [Fact]
        public void CreateCities_DifferentColoredCities_Succeeds()
        {
            StateManager state = new StateManager(Testing: true);

            List<string> inputStrings = new List<string>
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
            List<City> expectedCities = new List<City>
                {
                    new City("TestCity1", Colors.Yellow),
                    new City("TestCity2", Colors.Yellow),
                    new City("TestCity3", Colors.Red),
                    new City("TestCity4", Colors.Blue),
                    new City("TestCity5", Colors.Black)
                };

            Dictionary<string, City> generatedCities = state.TestCreateCities(inputStrings);

            Assert.Collection(generatedCities,
                item => { Assert.Equal(item.Value, expectedCities[0]); },
                item => { Assert.Equal(item.Value, expectedCities[1]); },
                item => { Assert.Equal(item.Value, expectedCities[2]); },
                item => { Assert.Equal(item.Value, expectedCities[3]); },
                item => { Assert.Equal(item.Value, expectedCities[4]); }
                );
        }

        [Fact]
        public void AddConnectedCities_CitiesAreAdded_Succeeds()
        {
            string cityName1 = "Atlanta";
            City testCity = new City(cityName1, Colors.Blue);

            string cityName2 = "Chicago";
            City connectedCity1 = new City(cityName2, Colors.Blue);

            string cityName3 = "Washington";
            City connectedCity2 = new City(cityName3, Colors.Blue);

            string cityName4 = "Miami";
            City connectedCity3 = new City(cityName4, Colors.Blue);

            Dictionary<string, City> cities = new Dictionary<string, City>
            {
                {cityName1, testCity },
                {cityName2, connectedCity1 },
                {cityName3, connectedCity2 },
                {cityName4, connectedCity3 }
            };
               
            StateManager state = new StateManager(
                Testing: true,
                Cities: cities
                );

            List<string> InputStrings = new List<string>
                {
                    "//Blue",
                    "Atlanta - Chicago, Washington, Miami"
                };

            state.TestAddConnectedCities(InputStrings);

            Assert.Collection((IEnumerable<City>)state.Cities[cityName1].ConnectedCities,
                item => { Assert.Equal(item, connectedCity1); },
                item => { Assert.Equal(item, connectedCity2); },
                item => { Assert.Equal(item, connectedCity3); }
                );
        }

        [Fact]
        public void AddConnectedCities_ConnectedCityDoesNotExist_ThrowsException()
        {
            StateManager state = new StateManager(
                Testing: true,
                Cities: new Dictionary<string, City>
                {
                        { "Atlanta", new City("Atlanta", Colors.Blue) },
                        { "Chicago", new City("Chicago", Colors.Blue) },
                        { "Washington", new City("Washington", Colors.Blue) }
                });

            List<string> inputStrings = new List<string>
                {
                    "//Blue",
                    "Atlanta - Chicago, Washington, Miami"
                };

            Assert.Throws<UnexpectedBehaviourException>(() => state.TestAddConnectedCities(inputStrings));
        }

        [Fact]
        public void AddCitiesToDecs_DecksAreCreated_Succeeds()
        {
            string cityName1 = "TestCity1";
            string cityName2 = "TestCity2";
            string cityName3 = "TestCity3";
            string cityName4 = "TestCity4";
            string cityName5 = "TestCity5";

            City testCity1 = new City(cityName1, Colors.Blue);
            City testCity2 = new City(cityName2, Colors.Blue);
            City testCity3 = new City(cityName3, Colors.Blue);
            City testCity4 = new City(cityName4, Colors.Blue);
            City testCity5 = new City(cityName5, Colors.Blue);

            StateManager state = new StateManager(
                Testing: true,
                Cities: new Dictionary<string, City>
                {
                        { cityName1, testCity1},
                        { cityName2, testCity2},
                        { cityName3, testCity3},
                        { cityName4, testCity4},
                        { cityName5, testCity5},
                });

            state.TestAddCitiesToDecks();

            Assert.Collection(state.PlayerDeck,
                item => { Assert.Equal(cityName1, item.Name); },
                item => { Assert.Equal(cityName2, item.Name); },
                item => { Assert.Equal(cityName3, item.Name); },
                item => { Assert.Equal(cityName4, item.Name); },
                item => { Assert.Equal(cityName5, item.Name); }
            );

            Assert.Collection(state.InfectionDeck,
                item => { Assert.Equal(cityName1, item.Name); },
                item => { Assert.Equal(cityName2, item.Name); },
                item => { Assert.Equal(cityName3, item.Name); },
                item => { Assert.Equal(cityName4, item.Name); },
                item => { Assert.Equal(cityName5, item.Name); }
            );
        }

        [Fact]
        public void AddEventsToPlayerDeck_Succeeds()
        {
            StateManager state = new StateManager(Testing: true);

            PlayerDeck expectedPlayerDeck = new PlayerDeck(new List<PlayerCard>
                {
                    new Airlift(),
                    new Forecast(),
                    new GovernmentGrant(),
                    new OneQuietNight(),
                    new ResilientPopulation()
                });

            state.TestAddEventsToPlayerDeck();

            Assert.Collection(state.PlayerDeck,
                item => { Assert.True(item is Airlift); },
                item => { Assert.True(item is Forecast); },
                item => { Assert.True(item is GovernmentGrant); },
                item => { Assert.True(item is OneQuietNight); },
                item => { Assert.True(item is ResilientPopulation); }
            );
        }

        [Fact]
        public void AssignPlayerRoles_FourPlayersAreAssignedRandomNonEqualRoles()
        {
            StateManager state = new StateManager(
                Testing: true,
                Cities: new Dictionary<string, City> { { "Atlanta", new City("Atlanta", Colors.Blue) } });

            List<User> testUsers = new List<User>
                {
                    new User(0, "Ragna Rekkverk"),
                    new User(1, "Frida Frosk"),
                    new User(2, "Sandra Salamander"),
                    new User(3, "Pelle Parafin")
                };

            state.TestAssignPlayerRoles(testUsers);

            Boolean duplicateRoles = false;
            int numberOfRoles = 0;
            for (int i = 0; i < testUsers.Count; i++)
            {
                User currentUser = testUsers[i];
                if (currentUser.CurrentRole != null)
                {
                    numberOfRoles++;
                }
                else { break; }

                for (int j = i + 1; j < testUsers.Count; j++)
                {
                    string currentUserRole = currentUser.CurrentRole.RoleName;
                    string otherUserRole = testUsers[j].CurrentRole.RoleName;
                    if (currentUserRole.Equals(otherUserRole))
                    {
                        duplicateRoles = true;
                        break;
                    }
                }
            }

            Assert.True(numberOfRoles == 4);
            Assert.False(duplicateRoles);
        }

        [Fact]
        public void DealPlayerCards_TwoPlayers_GetFourCardsEach()
        {
            int expectedNumberOfCards = 4;
            List<PlayerCard> expectedCards = new List<PlayerCard>
            {
                new CityCard("TestCity1", Colors.Yellow),
                new CityCard("TestCity2", Colors.Yellow),
                new CityCard("TestCity3", Colors.Red),
                new CityCard("TestCity4", Colors.Blue),
                new CityCard("TestCity5", Colors.Black),
                new CityCard("TestCity6", Colors.Black),
                new CityCard("TestCity7", Colors.Black),
                new CityCard("TestCity8", Colors.Black)
            };

            City startingCity = new City("Atlanta", Colors.Blue);
            StateManager state = new StateManager(
                Testing: true,
                Roles: new List<Role>
                {
                    new Medic(startingCity, 0),
                    new QuarantineSpecialist(startingCity, 1)
                },
                PlayerDeck: new PlayerDeck(expectedCards));

            expectedCards.Reverse();    //because draw pulls from end of deck. 

            state.TestDealPlayerCards();

            Assert.Collection(state.Roles,
                item => { 
                    Assert.Equal(expectedNumberOfCards, item.Hand.Count);
                    Assert.Collection(item.Hand,
                        item => { Assert.Equal(expectedCards[0], item); },
                        item => { Assert.Equal(expectedCards[1], item); },
                        item => { Assert.Equal(expectedCards[2], item); },
                        item => { Assert.Equal(expectedCards[3], item); }
                        );},
                item => {
                    Assert.Equal(expectedNumberOfCards, item.Hand.Count);
                    Assert.Collection(item.Hand,
                        item => { Assert.Equal(expectedCards[4], item); },
                        item => { Assert.Equal(expectedCards[5], item); },
                        item => { Assert.Equal(expectedCards[6], item); },
                        item => { Assert.Equal(expectedCards[7], item); }
                        );
                });
        }

        [Fact]
        public void DealPlayerCards_ThreePlayers_GetThreeCardsEach()
        {
            int expectedNumberOfCards = 3;
            List<PlayerCard> expectedCards = new List<PlayerCard>
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
            };

            City startingCity = new City("Atlanta", Colors.Blue);
            StateManager state = new StateManager(
                Testing: true,
                Roles: new List<Role>
                {
                    new Medic(startingCity, 0),
                    new QuarantineSpecialist(startingCity, 1),
                    new Scientist(startingCity, 2)
                },
                PlayerDeck: new PlayerDeck(expectedCards));

            expectedCards.Reverse();    //because draw pulls from end of deck. 

            state.TestDealPlayerCards();

            Assert.Collection(state.Roles,
                item => {
                    Assert.Equal(expectedNumberOfCards, item.Hand.Count);
                    Assert.Collection(item.Hand,
                        item => { Assert.Equal(expectedCards[0], item); },
                        item => { Assert.Equal(expectedCards[1], item); },
                        item => { Assert.Equal(expectedCards[2], item); }
                        );
                },
                item => {
                    Assert.Equal(expectedNumberOfCards, item.Hand.Count);
                    Assert.Collection(item.Hand,
                        item => { Assert.Equal(expectedCards[3], item); },
                        item => { Assert.Equal(expectedCards[4], item); },
                        item => { Assert.Equal(expectedCards[5], item); }
                        );
                },
                item => {
                    Assert.Equal(expectedNumberOfCards, item.Hand.Count);
                    Assert.Collection(item.Hand,
                        item => { Assert.Equal(expectedCards[6], item); },
                        item => { Assert.Equal(expectedCards[7], item); },
                        item => { Assert.Equal(expectedCards[8], item); }
                        );
                });
        }

        [Fact]
        public void DealPlayerCards_FourPlayers_GetTwoCardsEach()
        {
            int expectedNumberOfCards = 2;
            List<PlayerCard> expectedCards = new List<PlayerCard>
            {
                new CityCard("TestCity1", Colors.Yellow),
                new CityCard("TestCity2", Colors.Yellow),
                new CityCard("TestCity3", Colors.Red),
                new CityCard("TestCity4", Colors.Blue),
                new CityCard("TestCity5", Colors.Black),
                new CityCard("TestCity6", Colors.Black),
                new CityCard("TestCity7", Colors.Black),
                new CityCard("TestCity8", Colors.Black),
            };

            City startingCity = new City("Atlanta", Colors.Blue);
            StateManager state = new StateManager(
                Testing: true,
                Roles: new List<Role>
                {
                        new Medic(startingCity, 0),
                        new QuarantineSpecialist(startingCity, 1),
                        new Scientist(startingCity, 2),
                        new Researcher(startingCity, 3)
                },
                PlayerDeck: new PlayerDeck(expectedCards));

            expectedCards.Reverse();    //because draw pulls from end of deck. 

            state.TestDealPlayerCards();

            Assert.Collection(state.Roles,
                item => {
                    Assert.Equal(expectedNumberOfCards, item.Hand.Count);
                    Assert.Collection(item.Hand,
                        item => { Assert.Equal(expectedCards[0], item); },
                        item => { Assert.Equal(expectedCards[1], item); }
                        );
                },
                item => {
                    Assert.Equal(expectedNumberOfCards, item.Hand.Count);
                    Assert.Collection(item.Hand,
                        item => { Assert.Equal(expectedCards[2], item); },
                        item => { Assert.Equal(expectedCards[3], item); }
                        );
                },
                item =>
                {
                    Assert.Equal(expectedNumberOfCards, item.Hand.Count);
                    Assert.Collection(item.Hand,
                        item => { Assert.Equal(expectedCards[4], item); },
                        item => { Assert.Equal(expectedCards[5], item); }
                        );
                },
                item => {
                    Assert.Equal(expectedNumberOfCards, item.Hand.Count);
                    Assert.Collection(item.Hand,
                        item => { Assert.Equal(expectedCards[6], item); },
                        item => { Assert.Equal(expectedCards[7], item); }
                        );
                });
        }

        [Fact]
        public void DealPlayerCards_NoPlayerDeck_ThrowsException()
        {
            City startingCity = new City("Atlanta", Colors.Blue);
            StateManager state = new StateManager(
                Testing: true);

            Assert.Throws<UnexpectedBehaviourException>(() => state.TestDealPlayerCards());
        }

        [Fact]
        public void DealPlayerCards_InvalidNumberOfPlayers_ThrowsException()
        {
            City startingCity = new City("Atlanta", Colors.Blue);
            StateManager state = new StateManager(
                Testing: true,
                Roles: new List<Role>
                {
                        new Medic(startingCity, 0),
                        new QuarantineSpecialist(startingCity, 1),
                        new Scientist(startingCity, 2),
                        new QuarantineSpecialist(startingCity, 3),
                        new Researcher(startingCity, 4)
                },
                PlayerDeck: new PlayerDeck(new List<PlayerCard>
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

            Assert.Throws<UnexpectedBehaviourException>(() => state.TestDealPlayerCards());
        }

        [Fact]
        public void DealPlayerCards_InvalidNumberOfCardsInDeck_ThrowsException()
        {
            City startingCity = new City("Atlanta", Colors.Blue);
            StateManager state = new StateManager(
                Testing: true,
                Roles: new List<Role>
                {
                        new Medic(startingCity, 0),
                        new QuarantineSpecialist(startingCity, 1),
                },
                PlayerDeck: new PlayerDeck(new List<PlayerCard>
                {
                        new CityCard("TestCity1", Colors.Yellow),
                        new CityCard("TestCity2", Colors.Yellow),

                }));

            Assert.Throws<UnexpectedBehaviourException>(() => state.TestDealPlayerCards());
        }

        [Fact]
        public void AddEpidemicsToDeck_EpidemicsSuccessfullyAdded()
        {
            List<PlayerCard> initialDeck = new List<PlayerCard> 
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
            };

            List<PlayerCard> firstExpectedPile = new List<PlayerCard>
            {
                initialDeck[8],
                initialDeck[7],
                initialDeck[6],
                initialDeck[5],
                initialDeck[4]
            };
            List<PlayerCard> secondExpectedPile = new List<PlayerCard>
            {
                initialDeck[3],
                initialDeck[2],
                initialDeck[1],
                initialDeck[0]
            };

            StateManager state = new StateManager(
                Testing: true,
                NumberOfEpidemics: 2,
                PlayerDeck: new PlayerDeck(initialDeck));

            state.TestAddEpidemicsToDeck();

            Assert.Collection(state.PlayerDeck,
                item => { Assert.True((item is EpidemicCard) || (firstExpectedPile.Contains(item))); },
                item => { Assert.True((item is EpidemicCard) || (firstExpectedPile.Contains(item))); },
                item => { Assert.True((item is EpidemicCard) || (firstExpectedPile.Contains(item))); },
                item => { Assert.True((item is EpidemicCard) || (firstExpectedPile.Contains(item))); },
                item => { Assert.True((item is EpidemicCard) || (firstExpectedPile.Contains(item))); },
                item => { Assert.True((item is EpidemicCard) || (firstExpectedPile.Contains(item))); },
                item => { Assert.True((item is EpidemicCard) || (secondExpectedPile.Contains(item))); },
                item => { Assert.True((item is EpidemicCard) || (secondExpectedPile.Contains(item))); },
                item => { Assert.True((item is EpidemicCard) || (secondExpectedPile.Contains(item))); },
                item => { Assert.True((item is EpidemicCard) || (secondExpectedPile.Contains(item))); },
                item => { Assert.True((item is EpidemicCard) || (secondExpectedPile.Contains(item))); }
                );





            //int NumberOfEpidemics = 0;
            //foreach (Card CurrentCard in State.PlayerDeck)
            //{
            //    if (CurrentCard is EpidemicCard)
            //    {
            //        NumberOfEpidemics++;
            //    }
            //}

            //Boolean CorrectCardsArePresentInEachShuffle = true;
            //int Counter = 0;
            //List<string> FirstPile = new List<string>
            //{
            //    "TestCity1",
            //    "TestCity2",
            //    "TestCity3",
            //    "TestCity4",
            //    "TestCity5",
            //    "Epidemic"
            //};
            //List<string> SecondPile = new List<string>
            //{
            //    "TestCity6",
            //    "TestCity7",
            //    "TestCity8",
            //    "TestCity9",
            //    "Epidemic"
            //};
            //foreach (Card CurrentCard in State.PlayerDeck)
            //{
            //    if (Counter < 6)
            //    {
            //        if (!FirstPile.Remove(CurrentCard.Name))
            //        {
            //            CorrectCardsArePresentInEachShuffle = false;
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        if (!SecondPile.Remove(CurrentCard.Name))
            //        {
            //            CorrectCardsArePresentInEachShuffle = false;
            //            break;
            //        }
            //    }
            //    Counter++;
            //}

            //Boolean CorrectNumberOfCards = (State.PlayerDeck.Count() == 11);
            //Boolean CorrectNumberOfEpidemics = (NumberOfEpidemics == 2);

            //Assert.True(CorrectNumberOfCards && CorrectNumberOfEpidemics && CorrectCardsArePresentInEachShuffle);

        }

        [Fact]
        public void AddEpidemicsToDeck_PlayerDeckNotCreated_ThrowsException()
        {
            StateManager state = new StateManager(
                Testing: true,
                NumberOfEpidemics: 2
                );

            Assert.Throws<UnexpectedBehaviourException>(() => state.TestAddEpidemicsToDeck());
        }

        [Fact]
        public void InitialInfection_CitiesAreInfected_Succeeds()
        {
            Colors testColor = Colors.Blue;

            string cityName1 = "TestCity1";
            string cityName2 = "TestCity2";
            string cityName3 = "TestCity3";
            string cityName4 = "TestCity4";
            string cityName5 = "TestCity5";
            string cityName6 = "TestCity6";
            string cityName7 = "TestCity7";
            string cityName8 = "TestCity8";
            string cityName9 = "TestCity9";

            StateManager state = new StateManager(
                Testing: true,
                InfectionDeck: new InfectionDeck(new List<InfectionCard>
                {
                    new InfectionCard(cityName1 , testColor),
                    new InfectionCard(cityName2 , testColor),
                    new InfectionCard(cityName3 , testColor),
                    new InfectionCard(cityName4 , testColor),
                    new InfectionCard(cityName5 , testColor),
                    new InfectionCard(cityName6 , testColor),
                    new InfectionCard(cityName7 , testColor),
                    new InfectionCard(cityName8 , testColor),
                    new InfectionCard(cityName9 , testColor)
                }),
                Cities: new Dictionary<string, City>
                {
                    {cityName1 , new City(cityName1, testColor) },
                    {cityName2 , new City(cityName2, testColor) },
                    {cityName3 , new City(cityName3, testColor) },
                    {cityName4 , new City(cityName4, testColor) },
                    {cityName5 , new City(cityName5, testColor) },
                    {cityName6 , new City(cityName6, testColor) },
                    {cityName7 , new City(cityName7, testColor) },
                    {cityName8 , new City(cityName8, testColor) },
                    {cityName9 , new City(cityName9, testColor) }
                });

            state.TestInitialInfection();

            Assert.Collection(state.Cities.Values,
                item => { Assert.Equal(3, item.DiseaseCubes[testColor]); },
                item => { Assert.Equal(3, item.DiseaseCubes[testColor]); },
                item => { Assert.Equal(3, item.DiseaseCubes[testColor]); },
                item => { Assert.Equal(2, item.DiseaseCubes[testColor]); },
                item => { Assert.Equal(2, item.DiseaseCubes[testColor]); },
                item => { Assert.Equal(2, item.DiseaseCubes[testColor]); },
                item => { Assert.Equal(1, item.DiseaseCubes[testColor]); },
                item => { Assert.Equal(1, item.DiseaseCubes[testColor]); },
                item => { Assert.Equal(1, item.DiseaseCubes[testColor]); }
                );
        }

        [Fact]
        public void InitialInfection_NoInfectionDeckCreated_ThrowsException()
        {
            Colors testColor = Colors.Blue;

            string cityName1 = "TestCity1";
            string cityName2 = "TestCity2";
            string cityName3 = "TestCity3";
            string cityName4 = "TestCity4";
            string cityName5 = "TestCity5";
            string cityName6 = "TestCity6";
            string cityName7 = "TestCity7";
            string cityName8 = "TestCity8";
            string cityName9 = "TestCity9";

            StateManager state = new StateManager(
                Testing: true,
                Cities: new Dictionary<string, City>
                {
                    {cityName1 , new City(cityName1, testColor) },
                    {cityName2 , new City(cityName2, testColor) },
                    {cityName3 , new City(cityName3, testColor) },
                    {cityName4 , new City(cityName4, testColor) },
                    {cityName5 , new City(cityName5, testColor) },
                    {cityName6 , new City(cityName6, testColor) },
                    {cityName7 , new City(cityName7, testColor) },
                    {cityName8 , new City(cityName8, testColor) },
                    {cityName9 , new City(cityName9, testColor) }
                });

            Assert.Throws<UnexpectedBehaviourException>(() => state.TestInitialInfection());
        }

        [Fact]
        public void InitialInfection_NoCitiesInState_ThrowsException()
        {
            StateManager state = new StateManager(
                Testing: true,
                InfectionDeck: new InfectionDeck(new List<InfectionCard>
                {
                        new InfectionCard("TestCity1", Colors.Yellow),
                }));

            Assert.Throws<UnexpectedBehaviourException>(() => state.TestInitialInfection());
        }

        [Fact]
        public void GetCity_CardAsInput_CityIsFound()
        {
            string testName = "Atlanta";
            City testCity = new City(testName, Colors.Blue);
            StateManager state = new StateManager(
                Testing: true,
                Cities: new Dictionary<string, City>{
                    {testName, testCity } });

            CityCard cardToFind = new CityCard(testName, Colors.Blue);

            City foundCity = state.GetCity(cardToFind);

            Assert.Equal(testCity, foundCity);
        }

        [Fact]
        public void GetCity_CityNotFound_ThrowsException()
        {
            StateManager state = new StateManager(Testing: true);

            CityCard cityToFind = new CityCard("Atlanta", Colors.Blue);

            Assert.Throws<UnexpectedBehaviourException>(() => state.GetCity(cityToFind));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetCity_IntAsInput_CityIsFound(int index)
        {
            Colors testColor = Colors.Blue;

            string cityName1 = "TestCity1";
            string cityName2 = "TestCity2";
            string cityName3 = "TestCity3";

            City testCity1 = new City(cityName1, testColor);
            City testCity2 = new City(cityName2, testColor);
            City testCity3 = new City(cityName3, testColor);

            List<City> expectedCity = new List<City>
            {
                testCity1,
                testCity2,
                testCity3
            };

            StateManager state = new StateManager(
                Testing: true,
                Cities: new Dictionary<string, City>
                {
                    {cityName1 , new City(cityName1, testColor) },
                    {cityName2 , new City(cityName2, testColor) },
                    {cityName3 , new City(cityName3, testColor) }
                });

            City foundCity = state.GetCity(index);

            Assert.Equal(expectedCity[index], foundCity);
        }

        [Fact]
        public void GetCitiesWithResearchStation_CitiesAreReturned_Succeeds()
        {
            Colors testColor = Colors.Blue;

            string cityName1 = "TestCity1";
            string cityName2 = "TestCity2";
            string cityName3 = "TestCity3";
            string cityName4 = "TestCity4";

            City testCity1 = new City(cityName1, testColor);
            City testCity2 = new City(cityName2, testColor);
            City testCity3 = new City(cityName3, testColor);
            City testCity4 = new City(cityName4, testColor);

            StateManager State = new StateManager(
                Testing: true,
                Cities: new Dictionary<string, City>{
                    {cityName1, testCity1 },
                    {cityName2, testCity2 },
                    {cityName3, testCity3 },
                    {cityName4, testCity4 } 
                });

            testCity2.BuildResearchStation();
            testCity3.BuildResearchStation();

            List<City> CitiesWithStations = State.GetCitiesWithResearchStation();

            Assert.Collection(CitiesWithStations,
                item =>
                {
                    Assert.Equal(testCity2, item);
                    Assert.True(item.ResearchStation);
                },
                item => {
                    Assert.Equal(testCity3, item);
                    Assert.True(item.ResearchStation);
                });
        }

        [Fact]
        public void GetCitiesWithResearchStation_NoCitiesWithStation_ThrowsException()
        {
            string cityName1 = "TestCity1";
            City testCity1 = new City(cityName1, Colors.Blue);

            StateManager State = new StateManager(
                Testing: true,
                Cities: new Dictionary<string, City>{
                    {cityName1, testCity1 },
                });

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