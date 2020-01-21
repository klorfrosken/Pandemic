using System;
using System.Collections.Generic;
using Xunit;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;

namespace Pandemic.UnitTests.GameElements
{
    public class CityTests
    {
        [Fact]
        void IsConnectedTo_CitiesAreConnected_True()
        {
            City actualCity = new City("Atlanta", Colors.Blue);
            City connectedCity = new City("Chicago", Colors.Blue);
            List<City> ConnectedCities = new List<City>
            {
                connectedCity,
                new City("Washington", Colors.Blue),
                new City("Miami", Colors.Yellow)
            };
            actualCity.ConnectedCities.AddRange(ConnectedCities);

            Boolean actual = actualCity.IsConnectedTo(connectedCity);

            Assert.True(actual);
        }

        [Fact]
        void IsConnectedTo_CitiesAreNotConnected_False()
        {
            City actualCity = new City("Atlanta", Colors.Blue);
            City connectedCity = new City("Chicago", Colors.Blue);
            List<City> ConnectedCities = new List<City>
            {
                new City("Washington", Colors.Blue),
                new City("Miami", Colors.Yellow)
            };
            actualCity.ConnectedCities.AddRange(ConnectedCities);

            Boolean actual = actualCity.IsConnectedTo(connectedCity);

            Assert.False(actual);
        }

        [Fact]
        void IsConnectedTo_ConnectedCityIsNull_ThrowsException()
        {
            City actualCity = new City("Atlanta", Colors.Blue);
            City connectedCity = null;
            List<City> ConnectedCities = new List<City>
            {
                new City("Washington", Colors.Blue),
                new City("Miami", Colors.Yellow)
            };
            actualCity.ConnectedCities.AddRange(ConnectedCities);

            Assert.Throws<UnexpectedBehaviourException>(() => actualCity.IsConnectedTo(connectedCity));
        }

        [Fact]
        void BuildResearchStation_ResearchStationIsBuilt_True()
        {
            City actualCity = new City("Paris", Colors.Blue);

            actualCity.BuildResearchStation();

            Assert.True(actualCity.HasResearchStation);
        }

        [Fact]
        void BuildResearchStation_CityAlreadyHasResearchStation_ThrowsException()
        {
            City actualCity = new City("Paris", Colors.Blue);
            actualCity.BuildResearchStation();

            Assert.Throws<IllegalMoveException>(() => actualCity.BuildResearchStation());
        }

        [Fact]
        void TreatDisease_NoDiseaseToCure_False()
        {
            City actualCity = new City("Atlanta", Colors.Blue);
            
            Assert.Throws<IllegalMoveException>(() => actualCity.TreatDisease(Colors.Blue));
        }

        [Fact]
        void TreatDisease_ColorIsNone_ThrowsException()
        {
            City actualCity = new City("Atlanta", Colors.Blue);
            Assert.Throws<UnexpectedBehaviourException>(() => actualCity.TreatDisease(Colors.None));
        }

        [Fact]
        void TreatDisease_YellowIsCured_True()
        {
            StateManager state = new StateManager(Testing: true, MaxCubesInCubePool: 12);
            City actualCity = new City("Miami", Colors.Yellow, state);
            Colors ColorCubeToRemove = Colors.Yellow;
            actualCity.DiseaseCubes[ColorCubeToRemove] = 2;

            int expectedCubesCity = 1;
            int expectedCubesState = 13;

            actualCity.TreatDisease(ColorCubeToRemove);

            Assert.Equal(expectedCubesCity, actualCity.DiseaseCubes[ColorCubeToRemove]);
            Assert.Equal(expectedCubesState, state.CubePools[ColorCubeToRemove]);
        }

        [Fact]
        void TreatDisease_RedIsCured_True()
        {
            StateManager state = new StateManager(Testing: true, MaxCubesInCubePool: 12);
            City actualCity = new City("Miami", Colors.Yellow, state);
            Colors ColorCubeToRemove = Colors.Red;
            actualCity.DiseaseCubes[ColorCubeToRemove] = 2;

            int expectedCubesCity = 1;
            int expectedCubesState = 13;

            actualCity.TreatDisease(ColorCubeToRemove);

            Assert.Equal(expectedCubesCity, actualCity.DiseaseCubes[ColorCubeToRemove]);
            Assert.Equal(expectedCubesState, state.CubePools[ColorCubeToRemove]);
        }

        [Fact]
        void TreatDisease_BlueIsCured_True()
        {
            StateManager state = new StateManager(Testing: true, MaxCubesInCubePool: 12);
            City actualCity = new City("Miami", Colors.Yellow, state);
            Colors ColorCubeToRemove = Colors.Blue;
            actualCity.DiseaseCubes[ColorCubeToRemove] = 2;

            int expectedCubesCity = 1;
            int expectedCubesState = 13;

            actualCity.TreatDisease(ColorCubeToRemove);

            Assert.Equal(expectedCubesCity, actualCity.DiseaseCubes[ColorCubeToRemove]);
            Assert.Equal(expectedCubesState, state.CubePools[ColorCubeToRemove]);
        }

        [Fact]
        void TreatDisease_BlackIsCured_True()
        {
            StateManager state = new StateManager(Testing: true, MaxCubesInCubePool: 12);
            City actualCity = new City("Miami", Colors.Yellow, state);
            Colors ColorCubeToRemove = Colors.Black;
            actualCity.DiseaseCubes[ColorCubeToRemove] = 2;

            int expectedCubesCity = 1;
            int expectedCubesState = 13;

            actualCity.TreatDisease(ColorCubeToRemove);

            Assert.Equal(expectedCubesCity, actualCity.DiseaseCubes[ColorCubeToRemove]);
            Assert.Equal(expectedCubesState, state.CubePools[ColorCubeToRemove]);
        }

        [Fact]
        void DiseaseIsEradicated_CubePoolIsNotFullAndCureIsFound_False()
        {
            Colors currentColor = Colors.Blue;

            StateManager state = new StateManager(
                Testing: true,
                MaxCubesInCubePool: 12,
                Cures: new Dictionary<Colors, bool> { { currentColor, true } },
                CubePools: new Dictionary<Colors, int> { { currentColor, 5 } }
                );
            City actualCity = new City("Atlanta", currentColor, state);

            bool diseaseEradicated = actualCity.TestDiseaseIsEradicated(currentColor);

            Assert.False(diseaseEradicated);
        }

        [Fact]
        void DiseaseIsEradicated_CubePoolIsFullAndCureIsFound_True()
        {
            Colors currentColor = Colors.Blue;

            StateManager state = new StateManager(
                Testing: true,
                MaxCubesInCubePool: 12,
                Cures: new Dictionary<Colors, bool> { { currentColor, true } },
                CubePools: new Dictionary<Colors, int> { { currentColor, 12 } }
                );
            City actualCity = new City("Atlanta", currentColor, state);

            bool diseaseEradicated = actualCity.TestDiseaseIsEradicated(currentColor);

            Assert.True(diseaseEradicated);
        }

        [Fact]
        void DiseaseIsEradicated_CubePoolIsNotFullAndCureIsNotFound_False()
        {
            Colors currentColor = Colors.Blue;

            StateManager state = new StateManager(
                Testing: true,
                MaxCubesInCubePool: 12,
                Cures: new Dictionary<Colors, bool> { { currentColor, false } },
                CubePools: new Dictionary<Colors, int> { { currentColor, 5 } }
                );
            City actualCity = new City("Atlanta", currentColor, state);

            bool diseaseEradicated = actualCity.TestDiseaseIsEradicated(currentColor);

            Assert.False(diseaseEradicated);
        }

        [Fact]
        void DiseaseIsEradicated_CubePoolIsFullCureIsNotFound_False()
        {
            Colors currentColor = Colors.Blue;

            StateManager state = new StateManager(
                Testing: true,
                MaxCubesInCubePool: 12,
                Cures: new Dictionary<Colors, bool> { { currentColor, false } },
                CubePools: new Dictionary<Colors, int> { { currentColor, 12 } }
                );
            City actualCity = new City("Atlanta", currentColor, state);

            bool diseaseEradicated = actualCity.TestDiseaseIsEradicated(currentColor);

            Assert.False(diseaseEradicated);
        }

        [Fact]
        void InfectionPreventedByQuarantineSpecialist_QuarantineSpecialistIsNotInGame_False()
        {
            StateManager state = new StateManager(
                Testing: true,
                QuarantineSpecialistInGame: false);
            City actualCity = new City("Atlanta", Colors.Blue, state);

            bool InfectionPrevented = actualCity.TestInfectionPreventedByQuarantineSpecialist();

            Assert.False(InfectionPrevented);
        }

        [Fact]
        void InfectionPreventedByQuarantineSpecialist_QuarantineSpecialistNotInVicinity_False()
        {
            City otherCity = new City("Lima", Colors.Yellow);
            Role quarantineSpecialist = new QuarantineSpecialist(otherCity, 0);
            
            StateManager state = new StateManager(
                Testing: true,
                QuarantineSpecialistInGame: true,
                Roles: new List<Role> { quarantineSpecialist });
            City actualCity = new City("Atlanta", Colors.Blue, state);

            bool InfectionPrevented = actualCity.TestInfectionPreventedByQuarantineSpecialist();

            Assert.False(InfectionPrevented);
        }

        [Fact]
        void InfectionPreventedByQuarantineSpecialist_QuarantineSpecialistIsInCity_True()
        {
            List<Role> roles = new List<Role>();
            StateManager state = new StateManager(
                Testing: true,
                QuarantineSpecialistInGame: true,
                Roles: roles);

            City actualCity = new City("Atlanta", Colors.Blue, state);
            roles.Add(new QuarantineSpecialist(actualCity, 0));

            bool InfectionPrevented = actualCity.TestInfectionPreventedByQuarantineSpecialist();

            Assert.True(InfectionPrevented);
        }

        [Fact]
        void InfectionPreventedByQuarantineSpecialist_QuarantineSpecialistIsInConnectedCity_True()
        {
            City connectedCity = new City("Miami", Colors.Yellow);
            Role quarantineSpecialist = new QuarantineSpecialist(connectedCity, 0);

            StateManager state = new StateManager(
                Testing: true,
                QuarantineSpecialistInGame: true,
                Roles: new List<Role> { quarantineSpecialist });
            City actualCity = new City("Atlanta", Colors.Blue, state);
            actualCity.ConnectedCities.Add(connectedCity);

            bool InfectionPrevented = actualCity.TestInfectionPreventedByQuarantineSpecialist();

            Assert.True(InfectionPrevented);
        }

        [Fact]
        void InfectionPreventedByMedic_MedicNotInGame_False()
        {
            Colors currentColor = Colors.Blue;
            StateManager state = new StateManager(
                Testing: true,
                MedicInGame: false);
            City actualCity = new City("Atlanta", currentColor, state);

            Boolean InfectionPrevented = actualCity.TestInfectionPreventedByMedic(currentColor);

            Assert.False(InfectionPrevented);
        }

        [Fact]
        void InfectionPreventedByMedic_MedicNotInCityAndNoCure_False()
        {
            Colors currentColor = Colors.Blue;
            City otherCity = new City("Lima", Colors.Yellow);
            Role medic = new Medic(otherCity, 0);
            StateManager state = new StateManager(
                Testing: true,
                MedicInGame: true,
                Roles: new List<Role> { medic },
                Cures: new Dictionary<Colors, bool> { { currentColor, false } });
            City actualCity = new City("Atlanta", currentColor, state);

            Boolean InfectionPrevented = actualCity.TestInfectionPreventedByMedic(currentColor);

            Assert.False(InfectionPrevented);
        }

        [Fact]
        void InfectionPreventedByMedic_MedicInCityAndNoCure_False()
        {
            Colors currentColor = Colors.Blue;
            List<Role> roles = new List<Role>();
            StateManager state = new StateManager(
                Testing: true,
                MedicInGame: true,
                Roles: roles,
                Cures: new Dictionary<Colors, bool> { { currentColor, false } });
            City actualCity = new City("Atlanta", currentColor, state);
            roles.Add(new Medic(actualCity, 0));
            
            Boolean InfectionPrevented = actualCity.TestInfectionPreventedByMedic(currentColor);

            Assert.False(InfectionPrevented);
        }

        [Fact]
        void InfectionPreventedByMedic_MedicNotInCityAndCure_False()
        {
            Colors currentColor = Colors.Blue;
            City otherCity = new City("Lima", Colors.Yellow);
            Role medic = new Medic(otherCity, 0);
            StateManager state = new StateManager(
                Testing: true,
                MedicInGame: true,
                Roles: new List<Role> { medic },
                Cures: new Dictionary<Colors, bool> { { currentColor, true } });
            City actualCity = new City("Atlanta", currentColor, state);

            Boolean InfectionPrevented = actualCity.TestInfectionPreventedByMedic(currentColor);

            Assert.False(InfectionPrevented);
        }

        [Fact]
        void InfectionPreventedByMedic_MedicInCityAndCure_True()
        {
            Colors currentColor = Colors.Blue;
            List<Role> roles = new List<Role>();
            StateManager state = new StateManager(
                Testing: true,
                MedicInGame: true,
                Roles: roles,
                Cures: new Dictionary<Colors, bool> { { currentColor, true } });
            City actualCity = new City("Atlanta", currentColor, state);
            roles.Add(new Medic(actualCity, 0));

            Boolean InfectionPrevented = actualCity.TestInfectionPreventedByMedic(currentColor);

            Assert.True(InfectionPrevented);
        }

        [Fact]
        void InfectCity_DiseaseEradicated_NoInfection()
        {
            Colors currentColor = Colors.Blue;

            StateManager state = new StateManager(
                Testing: true,
                MaxCubesInCubePool: 12,
                Cures: new Dictionary<Colors, bool> { { currentColor, true } },
                CubePools: new Dictionary<Colors, int> { { currentColor, 12 } }
                );
            City actualCity = new City("Atlanta", currentColor, state);

            actualCity.InfectCity(currentColor);

            Assert.Equal(0, actualCity.DiseaseCubes[currentColor]);
        }

        [Fact]
        void InfectCity_DiseasePreventedByQuarantineSpecialist_NoInfection()
        {
            List<Role> roles = new List<Role>();
            StateManager state = new StateManager(
                Testing: true,
                QuarantineSpecialistInGame: true,
                Roles: roles);

            Colors currentColor = Colors.Blue;
            City actualCity = new City("Atlanta", currentColor, state);
            roles.Add(new QuarantineSpecialist(actualCity, 0));

            actualCity.InfectCity(currentColor);

            Assert.Equal(0, actualCity.DiseaseCubes[currentColor]);
        }

        [Fact]
        void InfectCity_DiseasePreventedByMedic_NoInfection()
        {
            Colors currentColor = Colors.Blue;
            List<Role> roles = new List<Role>();
            StateManager state = new StateManager(
                Testing: true,
                MedicInGame: true,
                Roles: roles,
                Cures: new Dictionary<Colors, bool> { { currentColor, true } });
            City actualCity = new City("Atlanta", currentColor, state);
            roles.Add(new Medic(actualCity, 0));

            actualCity.InfectCity(currentColor);

            Assert.Equal(0, actualCity.DiseaseCubes[currentColor]);
        }

        [Fact]
        void InfectCity_NoMoreCubesInCubePool_ThrowsException()
        {
            Colors currentColor = Colors.Blue;
            StateManager state = new StateManager(
                Testing: true,
                CubePools: new Dictionary<Colors, int> { { currentColor, 0 } });
            City actualCity = new City("Atlanta", currentColor, state);

            Assert.Throws<TheWorldIsDeadException>(() => actualCity.InfectCity(currentColor));
        }

        [Fact]
        void InfectCity_CubeIsCityColor_Succeeds()
        {
            StateManager state = new StateManager(
                Testing: true,
                MaxCubesInCubePool: 12);
            ITextManager textMgr = new TestTextManager();

            Colors currentColor = Colors.Blue;
            City actualCity = new City("Atlanta", currentColor, state, textMgr);

            actualCity.InfectCity(currentColor);

            Assert.Equal(1, actualCity.DiseaseCubes[currentColor]);
            Assert.Equal(11, state.CubePools[currentColor]);
            Assert.False(actualCity.MultipleDiseases);
        }

        [Fact]
        void InfectCity_CubeNotCityColor_Succeeds()
        {
            StateManager state = new StateManager(
                Testing: true,
                MaxCubesInCubePool: 12);
            ITextManager textMgr = new TestTextManager();

            Colors currentColor = Colors.Blue;
            City actualCity = new City("Atlanta", currentColor, state, textMgr);

            Colors infectionColor = Colors.Yellow;
            actualCity.InfectCity(infectionColor);

            Assert.Equal(1, actualCity.DiseaseCubes[infectionColor]);
            Assert.Equal(11, state.CubePools[infectionColor]);
            Assert.True(actualCity.MultipleDiseases);
        }

        [Fact]
        void Outbreak_MaxOutbreaksReached_ThrowsException()
        {
            StateManager state = new StateManager(
                Testing: true,
                MaxOutbreaks: 8,
                Outbreaks: 7);
            Colors currentColor = Colors.Blue;
            City actualCity = new City("Atlanta", currentColor, state);

            Assert.Throws<TheWorldIsDeadException>(() => actualCity.TestOutbreak(currentColor));
        }

        [Fact]
        void Outbreak_Succeeds()
        {
            StateManager state = new StateManager(
                Testing: true,
                Outbreaks: 2);
            ITextManager textMgr = new TestTextManager();

            Colors currentColor = Colors.Blue;
            City actualCity = new City("Atlanta", currentColor, state, textMgr);
            actualCity.ConnectedCities = new List<City>
            {
                new City("Washington", currentColor, state, textMgr),
                new City("Chicago", currentColor, state, textMgr),
                new City("Miami", currentColor, state, textMgr)
            };

            actualCity.TestOutbreak(currentColor);

            Assert.Equal(3, state.Outbreaks);
            Assert.True(state.OutbreakThisChain.Exists(City => City == actualCity));
            Assert.Collection(actualCity.ConnectedCities,
                item => { Assert.Equal(1, item.DiseaseCubes[currentColor]); },
                item => { Assert.Equal(1, item.DiseaseCubes[currentColor]); },
                item => { Assert.Equal(1, item.DiseaseCubes[currentColor]); });
        }

        [Fact]
        void InfectCity_OutbreakAndNotOutbreakThisChain_Succeeds()
        {
            StateManager state = new StateManager(
                Testing: true,
                Outbreaks: 2);
            ITextManager textMgr = new TestTextManager();

            Colors currentColor = Colors.Blue;
            City actualCity = new City("Atlanta", currentColor, state, textMgr);
            actualCity.ConnectedCities = new List<City>
            {
                new City("Washington", currentColor, state, textMgr),
                new City("Chicago", currentColor, state, textMgr),
                new City("Miami", currentColor, state, textMgr)
            };
            actualCity.DiseaseCubes[currentColor] = 3;

            actualCity.InfectCity(currentColor);

            Assert.Equal(3, state.Outbreaks);
            Assert.Contains(actualCity, state.OutbreakThisChain);
            Assert.Collection(actualCity.ConnectedCities,
                item => { Assert.Equal(1, item.DiseaseCubes[currentColor]); },
                item => { Assert.Equal(1, item.DiseaseCubes[currentColor]); },
                item => { Assert.Equal(1, item.DiseaseCubes[currentColor]); });
        }

        [Fact]
        void InfectCity_DoubleOutbreak_Succeeds()
        {
            StateManager state = new StateManager(
                Testing: true,
                Outbreaks: 2);
            ITextManager textMgr = new TestTextManager();

            Colors currentColor = Colors.Blue;
            City firstCity = new City("Atlanta", currentColor, state, textMgr);
            City secondCity = new City("Washington", currentColor, state, textMgr);
            City chicago = new City("Chicago", currentColor, state, textMgr);
            City miami = new City("Miami", Colors.Yellow, state, textMgr);
            City montreal = new City("Montreal", currentColor, state, textMgr);
            City newYork = new City("New York", currentColor, state, textMgr);
            firstCity.ConnectedCities = new List<City>
            {
                secondCity,
                chicago,
                miami
            };
            secondCity.ConnectedCities = new List<City>
            {
                firstCity,
                miami,
                montreal,
                newYork
            };
            firstCity.DiseaseCubes[currentColor] = 3;
            secondCity.DiseaseCubes[currentColor] = 3;

            firstCity.InfectCity(currentColor);

            Assert.Equal(4, state.Outbreaks);
            Assert.Contains(firstCity, state.OutbreakThisChain);
            Assert.Contains(secondCity, state.OutbreakThisChain);
            Assert.Collection(firstCity.ConnectedCities,
                item => { Assert.Equal(3, item.DiseaseCubes[currentColor]); },
                item => { Assert.Equal(1, item.DiseaseCubes[currentColor]); },
                item => { Assert.Equal(2, item.DiseaseCubes[currentColor]); 
                          Assert.True(item.MultipleDiseases); });
            Assert.Collection(secondCity.ConnectedCities,
                item => { Assert.Equal(3, item.DiseaseCubes[currentColor]); },
                item => { Assert.Equal(2, item.DiseaseCubes[currentColor]); 
                          Assert.True(item.MultipleDiseases);},
                item => { Assert.Equal(1, item.DiseaseCubes[currentColor]); },
                item => { Assert.Equal(1, item.DiseaseCubes[currentColor]); });
        }

        [Fact]
        void Equals_ObjectIsMatchingCity_True()
        {
            City FirstCity = new City("Atlanta", Colors.Blue);
            City SecondCity = new City("Atlanta", Colors.Blue);

            Assert.Equal(FirstCity, SecondCity);
        }

        [Fact]
        void Equals_ObjectIsNotMatchingCity_False()
        {
            City FirstCity = new City("Atlanta", Colors.Blue);
            City SecondCity = new City("Paris", Colors.Blue);

            Assert.NotEqual(FirstCity, SecondCity);
        }
    }
}
