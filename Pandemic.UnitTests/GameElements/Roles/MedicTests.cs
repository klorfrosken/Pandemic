using Xunit;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Game_Elements.Roles;
using Pandemic.Exceptions;
using System;
using System.Collections.Generic;

namespace Pandemic.UnitTests.GameElements.Roles
{
    public class MedicTests
    {
        [Fact]
        public void TreatDisease_NoCubesToRemove_ThrowsException()
        {
            Colors currentColor = Colors.Blue;
            City currentCity = new City("Atlanta", Colors.Blue);
            Medic player = new Medic(currentCity, 0);

            Exception ex = Assert.Throws<IllegalMoveException>(() => player.TreatDisease(currentColor));
            Assert.Equal($"There are no {currentColor} cubes in {currentCity} to remove.", ex.Message);
        }

        [Fact]
        public void TreatDisease_CubesPresentInCity_CureNotFound_Succeeds()
        {
            Colors currentColor = Colors.Blue;
            StateManager state = new StateManager(testing: true,
                cures: new Dictionary<Colors, bool>
                {
                    { currentColor, false }
                });
            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.DiseaseCubes[currentColor] = 3;
            Medic player = new Medic(currentCity, 0, state);

            player.TreatDisease(currentColor);

            Assert.Equal(0, currentCity.DiseaseCubes[currentColor]);
            Assert.Equal(3, player.RemainingActions);
        }

        [Fact]
        public void TreatDisease_CubesPresentInCity_CureFound_Succeeds()
        {
            Colors currentColor = Colors.Blue;
            StateManager state = new StateManager(testing: true,
                cures: new Dictionary<Colors, bool>
                {
                    { currentColor, true }
                });
            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.DiseaseCubes[currentColor] = 3;
            Medic player = new Medic(currentCity, 0, state);

            player.TreatDisease(currentColor);

            Assert.Equal(0, currentCity.DiseaseCubes[currentColor]);
            Assert.Equal(4, player.RemainingActions);
        }

        [Fact]
        public void AutoCure_Succeeds()
        {
            Colors currentColor = Colors.Blue;
            StateManager state = new StateManager(testing: true,
                cures: new Dictionary<Colors, bool>
                {
                    { currentColor, true },
                    { Colors.Yellow, false },
                    { Colors.Red, false },
                    { Colors.Black, false }
                });
            City currentCity = new City("Atlanta", Colors.Blue, state);
            currentCity.DiseaseCubes = new Dictionary<Colors, int>
            {
                {Colors.Yellow, 3 },
                {Colors.Red, 3 },
                {Colors.Blue, 3 },
                {Colors.Black, 3 }
            };
            Medic player = new Medic(currentCity, 0, state);

            player.TreatDisease(currentColor);

            Assert.Equal(0, currentCity.DiseaseCubes[currentColor]);
            Assert.Equal(3, currentCity.DiseaseCubes[Colors.Yellow]);
            Assert.Equal(3, currentCity.DiseaseCubes[Colors.Red]);
            Assert.Equal(3, currentCity.DiseaseCubes[Colors.Black]);
            Assert.Equal(4, player.RemainingActions);
        }
    }
}
