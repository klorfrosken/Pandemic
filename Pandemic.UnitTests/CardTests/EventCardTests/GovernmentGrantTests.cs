//using Xunit;
//using Pandemic.Cards.EventCards;
//using Pandemic.Game;
//using Pandemic.Managers;


//namespace Pandemic.UnitTests.Cards
//{
//    public class GovernmentGrantTests
//    {
//        [Fact]
//        public void Play_ThereIsAlreadyResearchStationInCity_Fails()
//        {
//            //Arrange
//            GovernmentGrant EventCard = new GovernmentGrant();
//            City CityToBuildIn = new City("Atlanta", Colors.Blue);
//            CityToBuildIn.ResearchStation = true;

//            //Act
//            bool Actual = EventCard.PlayHandler(CityToBuildIn);

//            //Assert
//            Assert.False(Actual);
//        }

//        [Fact]
//        public void Play_BuildIsAchieved_Succeeds()
//        {
//            //Arrange
//            GovernmentGrant EventCard = new GovernmentGrant();
//            City CityToBuildIn = new City("Atlanta", Colors.Blue);

//            //Act
//            bool Actual = EventCard.PlayHandler(CityToBuildIn);

//            //Assert
//            Assert.True(Actual);
//        }
//    }
//}