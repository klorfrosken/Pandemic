using Xunit;
using Pandemic.Game;
using Pandemic.Managers;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using Pandemic.UnitTests.TestClasses;
using Pandemic.Cards;
using System.Collections.Generic;

namespace Pandemic.UnitTests.GameElements.Roles
{
    public class ScientistTest
    {
        [Fact]
        public void DiscoverCure_Succeeds()
        {
            StateManager state = new StateManager(
                Testing: true,
                Cures: new Dictionary<Colors, bool>{
                    {Colors.Yellow, false },
                    {Colors.Red, false },
                    {Colors.Blue, false },
                    {Colors.Black, false },});
            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.BuildResearchStation();

            Role role = new Scientist(currentCity, 0, state);
            role.Hand = new List<PlayerCard>
            {
                new CityCard("card", Colors.Blue),
                new CityCard("card", Colors.Blue),
                new CityCard("card", Colors.Blue),
                new CityCard("card", Colors.Blue),
            };

            role.DiscoverCure();

            Assert.True(state.Cures[Colors.Blue]);
        }
    }
}
