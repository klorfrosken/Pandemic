//using Xunit;
//using Pandemic.Managers;
//using Pandemic.Cards.EventCards;
//using Pandemic.Game;
//using Pandemic.Game_Elements.Roles;

//namespace Pandemic.UnitTests.Cards
//{
//Husk å legge inn at kortet må discardes!
//    public class AirliftTests
//    {
//        [Fact]
//        public void Play_PlayerToMoveAlreadyInCity_Fails()
//        {
//            //Arrange
//            Airlift EventCard = new Airlift();
//            City CurrentCity = new City("Atlanta", Colors.Blue);
//            Role PlayerToMove = new Scientist(CurrentCity);
//            City NewCity = CurrentCity;

//            //Act
//            bool Actual = EventCard.PlayHandler(PlayerToMove, NewCity);

//            //Assert
//            Assert.False(Actual);
//        }

//        [Fact]
//        public void Play_PlayerToMoveIsMoved_Succeeds()
//        {
//            //Arrange
//            Airlift EventCard = new Airlift();
//            City CurrentCity = new City("Atlanta", Colors.Blue);
//            City CityToMoveTo = new City("Paris", Colors.Blue);
//            Role PlayerToMove = new Scientist(CurrentCity);

//            //Act
//            bool PlayHandlerWorks = EventCard.PlayHandler(PlayerToMove, CityToMoveTo);
//            bool PlayerIsMoved = PlayerToMove.CurrentCity == CityToMoveTo;
//            bool Actual = PlayHandlerWorks && PlayerIsMoved;

//            //Assert
//            Assert.True(Actual);

//        }
//    }
//}
