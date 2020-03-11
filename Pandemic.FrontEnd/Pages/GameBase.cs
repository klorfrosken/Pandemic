using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pandemic.Managers;
using Pandemic.Game;

namespace Pandemic.FrontEnd.Pages
{
    public class GameBase : ComponentBase
    {
        private static (string png, string svg)[] alfaCureBottles =
        {
            ("/css/icons/Alfa NoCure.png",      "/css/icons/Alfa NoCure.svg"),
            ("/css/icons/Alfa Cure.png",        "/css/icons/Alfa Cure.svg"),
            ("/css/icons/Alfa Eradicated.png",  "/css/icons/Alfa Eradicated.svg"),
        };

        private static (string png, string svg)[] betaCureBottles =
        {
            ("/css/icons/Beta NoCure.png",      "/css/icons/Beta NoCure.svg"),
            ("/css/icons/Beta Cure.png",        "/css/icons/Beta Cure.svg"),
            ("/css/icons/Beta Eradicated.png",  "/css/icons/Beta Eradicated.svg"),
        };

        private static (string png, string svg)[] gammaCureBottles =
        {
            ("/css/icons/Gamma NoCure.png",      "/css/icons/Gamma NoCure.svg"),
            ("/css/icons/Gamma Cure.png",        "/css/icons/Gamma Cure.svg"),
            ("/css/icons/Gamma Eradicated.png",  "/css/icons/Gamma Eradicated.svg"),
        };

        private static (string png, string svg)[] deltaCureBottles =
        {
            ("/css/icons/Delta NoCure.png",      "/css/icons/Delta NoCure.svg"),
            ("/css/icons/Delta Cure.png",        "/css/icons/Delta Cure.svg"),
            ("/css/icons/Delta Eradicated.png",  "/css/icons/Delta Eradicated.svg"),
        };

        public List<(string png, string svg)[]> cureBottles = new List<(string png, string svg)[]>
        {
            alfaCureBottles,
            betaCureBottles,
            gammaCureBottles,
            deltaCureBottles
        };

        public (string png, string svg)[] cubePools =
        {
            ("/css/icons/Alfa Pool.png",      "/css/icons/Alfa Pool.svg"),
            ("/css/icons/Beta Pool.png",      "/css/icons/Beta Pool.svg"),
            ("/css/icons/Gamma Pool.png",      "/css/icons/Gamma Pool.svg"),
            ("/css/icons/Delta Pool.png",      "/css/icons/Delta Pool.svg"),
        };




        //tempCode
        static Dictionary<Colors, bool> SMcures = new Dictionary<Colors, bool>
        {
            {Colors.Yellow, false },
            {Colors.Red, true },
            {Colors.Blue, true },
            {Colors.Black, false }
        };

        static Dictionary<Colors, int> SMcubePools = new Dictionary<Colors, int>
        {
            {Colors.Yellow, 24 },
            {Colors.Red, 12 },
            {Colors.Blue, 24 },
            {Colors.Black, 24 }
        };

        static Dictionary<string, City> SMcities = new Dictionary<string, City>
            {
                {"Atlanta", new City("Atlanta", Colors.Blue) },
                {"Chicago", new City("Chicago", Colors.Blue) },
                {"Washington", new City("Washington", Colors.Blue) },
                {"Miami", new City("Miami", Colors.Blue) }
            };
        public StateManager currentState = new StateManager(testing: true, cures: SMcures, cubePools: SMcubePools, cities: SMcities);


        //public GameManager Game = new GameManager();

        public int getCureState(int i)
        {
            int cureState = 0;
            bool cureFound = currentState.Cures[(Colors)i];
            bool isEradicated = currentState.CubePools[(Colors)i] == currentState.MaxCubesInCubePool;
            if (cureFound && isEradicated)
            {
                cureState = 2;
            }
            else if (cureFound)
            {
                cureState = 1;
            }
            return cureState;
        }

        public string setCityID(City city)
        {
            string cityID = city.Name.ToLower();
            if (city.Name.Contains(' '))
            {
                cityID.Replace(' ', '-');
            }
            return cityID;
        }
    }
}
