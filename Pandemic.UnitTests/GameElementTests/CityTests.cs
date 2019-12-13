//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xunit;
//using Pandemic.Game;
//using Pandemic.Cards;
//using Pandemic.Managers;
//using Pandemic.Game_Elements.Roles;

//namespace Pandemic.UnitTests.GameElements
//{
//    public class CityTests
//    {
//        [Fact]
//        void IsConnectedTo_CitiesAreConnected_True()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            City ConnectedCity = new City("Chicago", Colors.Blue);
//            List<City> ConnectedCities = new List<City>
//            {
//                ConnectedCity,
//                new City("Washington", Colors.Blue),
//                new City("Miami", Colors.Yellow)
//            };
//            ActualCity.ConnectedCities.AddRange(ConnectedCities);

//            Assert.True(ActualCity.IsConnectedTo(ConnectedCity));
//        }

//        [Fact]
//        void IsConnectedTo_CitiesAreNotConnected_False()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            City ConnectedCity = new City("Chicago", Colors.Blue);
//            List<City> ConnectedCities = new List<City>
//            {
//                new City("Washington", Colors.Blue),
//                new City("Miami", Colors.Yellow)
//            };
//            ActualCity.ConnectedCities.AddRange(ConnectedCities);

//            Assert.False(ActualCity.IsConnectedTo(ConnectedCity));
//        }

//        [Fact]
//        void BuildResearchStation_ResearchStationIsBuilt_True()
//        {
//            City ActualCity = new City("Paris", Colors.Blue);
//            Assert.True(ActualCity.BuildResearchStation());
//        }

//        [Fact]
//        void BuildResearchStation_CityAlreadyHasResearchStation_True()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            ActualCity.ResearchStation = true;
//            Assert.False(ActualCity.BuildResearchStation());
//        }
    
//        [Fact]
//        void TreatDisease_NoDiseaseToCure_False()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Assert.False(ActualCity.TreatDisease(Colors.Blue));
//        }

//        //Dette er ikke god praksis, men hvordan skal man ellers gjøre det når metoden gjør 3 ting. En må jo sjekke at alle tre er gjennomført...
//        [Fact]
//        void TreatDisease_YellowIsCured_True()
//        {
//            new StateManager(Testing: true, MaxCubesInCubePool: 12);
//            City ActualCity = new City("Miami", Colors.Yellow);
//            Colors ColorCubeToRemove = Colors.Yellow;
//            ActualCity.DiseaseCubes[ColorCubeToRemove] = 2;

//            bool MethodReturnsTrue = ActualCity.TreatDisease(ColorCubeToRemove);

//            bool CubeIsRemovedFromCity = (ActualCity.DiseaseCubes[ColorCubeToRemove] == 1);
//            bool CubeIsAddedToCubePool = (StateManager.CubePools[ColorCubeToRemove] == 13);

//            Assert.True(CubeIsRemovedFromCity && MethodReturnsTrue && CubeIsAddedToCubePool);
//        }

//        [Fact]
//        void TreatDisease_RedIsCured_True()
//        {
//            new StateManager(Testing: true, MaxCubesInCubePool: 12);
//            City ActualCity = new City("Miami", Colors.Yellow);
//            Colors ColorCubeToRemove = Colors.Red;
//            ActualCity.DiseaseCubes[ColorCubeToRemove] = 2;

//            bool MethodReturnsTrue = ActualCity.TreatDisease(ColorCubeToRemove);

//            bool CubeIsRemovedFromCity = (ActualCity.DiseaseCubes[ColorCubeToRemove] == 1);
//            bool CubeIsAddedToCubePool = (StateManager.CubePools[ColorCubeToRemove] == 13);

//            Assert.True(CubeIsRemovedFromCity && MethodReturnsTrue && CubeIsAddedToCubePool);
//        }

//        [Fact]
//        void TreatDisease_BlueIsCured_True()
//        {
//            new StateManager(Testing: true, MaxCubesInCubePool: 12);
//            City ActualCity = new City("Miami", Colors.Yellow);
//            Colors ColorCubeToRemove = Colors.Blue;
//            ActualCity.DiseaseCubes[ColorCubeToRemove] = 2;

//            bool MethodReturnsTrue = ActualCity.TreatDisease(ColorCubeToRemove);

//            bool CubeIsRemovedFromCity = (ActualCity.DiseaseCubes[ColorCubeToRemove] == 1);
//            bool CubeIsAddedToCubePool = (StateManager.CubePools[ColorCubeToRemove] == 13);

//            Assert.True(CubeIsRemovedFromCity && MethodReturnsTrue && CubeIsAddedToCubePool);
//        }

//        [Fact]
//        void TreatDisease_BlackIsCured_True()
//        {
//            new StateManager(Testing: true, MaxCubesInCubePool: 12);
//            City ActualCity = new City("Miami", Colors.Yellow);
//            Colors ColorCubeToRemove = Colors.Black;
//            ActualCity.DiseaseCubes[ColorCubeToRemove] = 2;
           
//            bool MethodReturnsTrue = ActualCity.TreatDisease(ColorCubeToRemove);

//            bool CubeIsRemovedFromCity = (ActualCity.DiseaseCubes[ColorCubeToRemove] == 1);
//            bool CubeIsAddedToCubePool = (StateManager.CubePools[ColorCubeToRemove] == 13);

//            Assert.True(CubeIsRemovedFromCity && MethodReturnsTrue && CubeIsAddedToCubePool);
//        }

//        [Fact]
//        void TreatDisease_ColorIsNone_ThrowsException()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Assert.Throws<ArgumentException>(() => ActualCity.TreatDisease(Colors.None));
//        }

//        [Fact]
//        void DiseaseIsEradicated_CubePoolIsNotFullAndCureIsFound_False()
//        {
//            new StateManager(Testing: true);
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Colors CurrentColor = Colors.Blue;
//            StateManager.CubePools[CurrentColor]--;
//            StateManager.Cures[CurrentColor] = true;

//            Assert.False(ActualCity.DiseaseIsEradicated(CurrentColor));
//        }

//        [Fact]
//        void DiseaseIsEradicated_CubePoolIsFullAndCureIsFound_True()
//        {
//            new StateManager(Testing: true);
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Colors CurrentColor = Colors.Blue;
//            StateManager.Cures[CurrentColor] = true;

//            Assert.True(ActualCity.DiseaseIsEradicated(CurrentColor));
//        }

//        [Fact]
//        void DiseaseIsEradicated_CubePoolIsNotFullAndCureIsNotFound_False()
//        {
//            new StateManager(Testing: true);
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Colors CurrentColor = Colors.Blue;
//            StateManager.CubePools[CurrentColor]--;

//            Assert.False(ActualCity.DiseaseIsEradicated(CurrentColor));
//        }

//        [Fact]
//        void DiseaseIsEradicated_CubePoolIsFullCureIsNotFound_False()
//        {
//            new StateManager(Testing: true);
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Colors CurrentColor = Colors.Blue;

//            Assert.False(ActualCity.DiseaseIsEradicated(CurrentColor));
//        }

//        [Fact]
//        void InfectionPreventedByQuarantineSpecialist_QuarantineSpecialistIsNotInGame_False()
//        {
//            new StateManager(Testing: true);
//            City ActualCity = new City("Atlanta", Colors.Blue);

//            Assert.False(ActualCity.InfectionPreventedByQuarantineSpecialist());
//        }

//        [Fact]
//        void InfectionPreventedByQuarantineSpecialist_QuarantineSpecialistIsInCity_True()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Role TestPlayer = new QuarantineSpecialist(ActualCity);
//            new StateManager(Testing: true, QuarantineSpecialistInGame: true ,TestPlayer: TestPlayer);

//            Assert.True(ActualCity.InfectionPreventedByQuarantineSpecialist());
//        }

//        [Fact]
//        void InfectionPreventedByQuarantineSpecialist_QuarantineSpecialistIsInConnectedCity_True()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            City ConnectedCity = new City("Miami", Colors.Yellow);
//            ActualCity.ConnectedCities.Add(ConnectedCity);
//            Role TestPlayer = new QuarantineSpecialist(ConnectedCity);
//            new StateManager(Testing: true, QuarantineSpecialistInGame: true, TestPlayer: TestPlayer);

//            Assert.True(ActualCity.InfectionPreventedByQuarantineSpecialist());
//        }

//        [Fact]
//        void InfectionPreventedByMedic_MedicNotInGame_False()
//        {
//            Colors CurrentColor = Colors.Blue;
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            new StateManager(Testing: true);

//            Assert.False(ActualCity.InfectionPreventedByMedic(CurrentColor));
//        }

//        [Fact]
//        void InfectionPreventedByMedic_MedicNotInCityAndNoCure_False()
//        {
//            Colors CurrentColor = Colors.Blue;
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            City StartingCity = new City("Miami", Colors.Yellow);
//            Role TestPlayer = new Medic(StartingCity);
//            new StateManager(Testing: true, TestPlayer: TestPlayer);

//            Assert.False(ActualCity.InfectionPreventedByMedic(CurrentColor));
//        }

//        [Fact]
//        void InfectionPreventedByMedic_MedicInCityAndNoCure_False()
//        {
//            Colors CurrentColor = Colors.Blue;
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Role TestPlayer = new Medic(ActualCity);
//            new StateManager(Testing: true, TestPlayer: TestPlayer);

//            Assert.False(ActualCity.InfectionPreventedByMedic(CurrentColor));
//        }

//        [Fact]
//        void InfectionPreventedByMedic_MedicNotInCityAndCure_False()
//        {
//            Colors CurrentColor = Colors.Blue;
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            City StartingCity = new City("Miami", Colors.Yellow);
//            Role TestPlayer = new Medic(StartingCity);
//            new StateManager(Testing: true, TestPlayer: TestPlayer);
//            StateManager.Cures[CurrentColor] = true;

//            Assert.False(ActualCity.InfectionPreventedByMedic(CurrentColor));
//        }

//        [Fact]
//        void InfectionPreventedByMedic_MedicInCityAndCure_True()
//        {
//            Colors CurrentColor = Colors.Blue;
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Role TestPlayer = new Medic(ActualCity);
//            new StateManager(Testing: true, TestPlayer: TestPlayer, MedicInGame: true);
//            StateManager.Cures[CurrentColor] = true;

//            Assert.True(ActualCity.InfectionPreventedByMedic(CurrentColor));
//        }

//        [Fact]
//        void InfectCity_DiseaseEradicated_Fails()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            new StateManager(Testing: true);
//            Colors CurrentColor = Colors.Blue;
//            StateManager.Cures[CurrentColor] = true;

//            ActualCity.InfectCity(CurrentColor);

//            Assert.True(ActualCity.DiseaseCubes[CurrentColor] == 0);
//        }

//        [Fact]
//        void InfectCity_DiseasePreventedByQuarantineSpecialist_Fails()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Role TestPlayer = new QuarantineSpecialist(ActualCity);
//            new StateManager(Testing: true, TestPlayer: TestPlayer);
//            Colors CurrentColor = Colors.Blue;

//            ActualCity.InfectCity(CurrentColor);

//            Assert.True(ActualCity.DiseaseCubes[CurrentColor] == 0);
//        }

//        [Fact]
//        void InfectCity_DiseasePreventedByMedic_Fails()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            Role TestPlayer = new Medic(ActualCity);
//            new StateManager(Testing: true, TestPlayer: TestPlayer);
//            Colors CurrentColor = Colors.Blue;
//            StateManager.Cures[CurrentColor] = true;

//            ActualCity.InfectCity(CurrentColor);

//            Assert.True(ActualCity.DiseaseCubes[CurrentColor] == 0);
//        }

//        [Fact]
//        void Outbreak_Succeeds()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            City ConnectedCity1 = new City("Washington", Colors.Blue);
//            City ConnectedCity2 = new City("Chicago", Colors.Blue);
//            City ConnectedCity3 = new City("Miami", Colors.Yellow);

//            ActualCity.ConnectedCities.Add(ConnectedCity1);
//            ActualCity.ConnectedCities.Add(ConnectedCity2);
//            ActualCity.ConnectedCities.Add(ConnectedCity3);

//            new StateManager(Testing: true);
//            Colors CurrentColor = Colors.Blue;

//            ActualCity.Outbreak(CurrentColor);
            
//            bool OutbreaksIncreased = (StateManager.Outbreaks == 1);
//            bool CityInOutbreaksThisChain = StateManager.OutbreakThisChain.Exists(City => City == ActualCity);
//            bool CubeInConnectedCity1 = (ConnectedCity1.DiseaseCubes[CurrentColor] == 1);
//            bool CubeInConnectedCity2 = (ConnectedCity2.DiseaseCubes[CurrentColor] == 1);
//            bool CubeInConnectedCity3 = (ConnectedCity3.DiseaseCubes[CurrentColor] == 1);
//            bool CubeInAllConnectedCities = (CubeInConnectedCity1 && CubeInConnectedCity2 && CubeInConnectedCity3);

//            Assert.True(OutbreaksIncreased && CityInOutbreaksThisChain && CubeInAllConnectedCities);
//        }

//        [Fact]
//        void InfectCity_OutbreakAndNotOutbreakThisChain_Succeeds()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            City ConnectedCity1 = new City("Washington", Colors.Blue);
//            City ConnectedCity2 = new City("Chicago", Colors.Blue);
//            City ConnectedCity3 = new City("Miami", Colors.Yellow);

//            ActualCity.ConnectedCities.Add(ConnectedCity1);
//            ActualCity.ConnectedCities.Add(ConnectedCity2);
//            ActualCity.ConnectedCities.Add(ConnectedCity3);

//            new StateManager(Testing: true);
//            Colors CurrentColor = Colors.Blue;
//            ActualCity.DiseaseCubes[CurrentColor] = 3;

//            ActualCity.InfectCity(CurrentColor);

//            bool OutbreaksIncreased = (StateManager.Outbreaks == 1);
//            bool CityInOutbreaksThisChain = StateManager.OutbreakThisChain.Exists(City => City == ActualCity);
//            bool CubeInConnectedCity1 = (ConnectedCity1.DiseaseCubes[CurrentColor] == 1);
//            bool CubeInConnectedCity2 = (ConnectedCity2.DiseaseCubes[CurrentColor] == 1);
//            bool CubeInConnectedCity3 = (ConnectedCity3.DiseaseCubes[CurrentColor] == 1);
//            bool CubeInAllConnectedCities = (CubeInConnectedCity1 && CubeInConnectedCity2 && CubeInConnectedCity3);

//            Assert.True(OutbreaksIncreased && CityInOutbreaksThisChain && CubeInAllConnectedCities);
//        }

//        [Fact]
//        void InfectCity_OutbreakAndAlreadyOutbreakThisChain_Fails()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            City ConnectedCity1 = new City("Washington", Colors.Blue);
//            City ConnectedCity2 = new City("Chicago", Colors.Blue);
//            City ConnectedCity3 = new City("Miami", Colors.Yellow);

//            ActualCity.ConnectedCities.Add(ConnectedCity1);
//            ActualCity.ConnectedCities.Add(ConnectedCity2);
//            ActualCity.ConnectedCities.Add(ConnectedCity3);

//            new StateManager(Testing: true);
//            Colors CurrentColor = Colors.Blue;
//            ActualCity.DiseaseCubes[CurrentColor] = 3;
//            StateManager.OutbreakThisChain.Add(ActualCity);

//            ActualCity.InfectCity(CurrentColor);

//            bool OutbreaksIncreased = (StateManager.Outbreaks == 1);
//            bool CubeInConnectedCity1 = (ConnectedCity1.DiseaseCubes[CurrentColor] == 0);
//            bool CubeInConnectedCity2 = (ConnectedCity2.DiseaseCubes[CurrentColor] == 0);
//            bool CubeInConnectedCity3 = (ConnectedCity3.DiseaseCubes[CurrentColor] == 0);
//            bool NoCubeInAllConnectedCities = (CubeInConnectedCity1 && CubeInConnectedCity2 && CubeInConnectedCity3);

//            Assert.True(!OutbreaksIncreased && NoCubeInAllConnectedCities);
//        }

//        [Fact]
//        void InfectCity_CubeNotCityColor_Succeeds()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            new StateManager(Testing: true, MaxCubesInCubePool: 12);
//            Colors CurrentColor = Colors.Yellow;

//            ActualCity.InfectCity(CurrentColor);

//            bool MultipleDiseasesInCity = ActualCity.MultipleDiseases;
//            bool DiseaseCubeInCity = (ActualCity.DiseaseCubes[CurrentColor] == 1);
//            bool CubePoolDecreased = (StateManager.CubePools[CurrentColor] == 11);

//            Assert.True(MultipleDiseasesInCity && DiseaseCubeInCity && CubePoolDecreased);
//        }

//        [Fact]
//        void InfectCity_CubeIsCityColor_Succeeds()
//        {
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            new StateManager(Testing: true, MaxCubesInCubePool: 12);
//            Colors CurrentColor = Colors.Blue;

//            ActualCity.InfectCity(CurrentColor);

//            bool MultipleDiseasesInCity = ActualCity.MultipleDiseases;
//            bool DiseaseCubeInCity = (ActualCity.DiseaseCubes[CurrentColor] == 1);
//            bool CubePoolDecreased = (StateManager.CubePools[CurrentColor] == 11);

//            Assert.True(!MultipleDiseasesInCity && DiseaseCubeInCity && CubePoolDecreased);
//        }

//        [Fact]
//        void InfectCity_NoMoreCubesInCubePool_Fails()
//        {
//            //This method is currently written to fail as there is no method for loosing conditions yet
//            City ActualCity = new City("Atlanta", Colors.Blue);
//            new StateManager(Testing: true, MaxCubesInCubePool: 0);
//            Colors CurrentColor = Colors.Blue;

//            ActualCity.InfectCity(CurrentColor);

//            bool LooseCondition = false;

//            Assert.True(LooseCondition);
//        }

//        [Fact]
//        void Equals_ObjectIsMatchingCity_Succeeds()
//        {
//            City FirstCity = new City("Atlanta", Colors.Blue);
//            City SecondCity = new City("Atlanta", Colors.Blue);

//            Assert.Equal(FirstCity, SecondCity);
//        }

//        [Fact]
//        void Equals_ObjectIsNotMatchingCity_Fails()
//        {
//            City FirstCity = new City("Atlanta", Colors.Blue);
//            City SecondCity = new City("Paris", Colors.Blue);

//            Assert.NotEqual(FirstCity, SecondCity);
//        }

//        [Fact]
//        void Equals_ObjectIsMatchingCityCard_Succeeds()
//        {
//            City FirstCity = new City("Atlanta", Colors.Blue);
//            CityCard CityCard = new CityCard("Atlanta", Colors.Blue);

//            Assert.True(FirstCity.Equals(CityCard));
//        }

//        [Fact]
//        void Equals_ObjectIsNotMatchingCityCard_Fails()
//        {
//            City FirstCity = new City("Atlanta", Colors.Blue);
//            CityCard CityCard = new CityCard("Paris", Colors.Blue);

//            Assert.False(FirstCity.Equals(CityCard));
//        }

//        [Fact]
//        void Equals_ObjectIsMatchingInfectionCard_Succeeds()
//        {
//            City FirstCity = new City("Atlanta", Colors.Blue);
//            InfectionCard CityCard = new InfectionCard("Atlanta", Colors.Blue);

//            Assert.True(FirstCity.Equals(CityCard));
//        }

//        [Fact]
//        void Equals_ObjectIsNotMatchingInfectionCard_Fails()
//        {
//            City FirstCity = new City("Atlanta", Colors.Blue);
//            InfectionCard CityCard = new InfectionCard("Paris", Colors.Blue);

//            Assert.False(FirstCity.Equals(CityCard));
//        }

//        [Fact]
//        void Equals_ObjectIsNotValid_Fails()
//        {
//            City FirstCity = new City("Atlanta", Colors.Blue);
//            var i = 2;

//            Assert.False(FirstCity.Equals(i));
//        }
//    }
//}
