using System;
using System.Collections.Generic;
using System.Text;

namespace Pandemic.Managers
{
    class GameStatistics
    {
        public int GamesPlayed { get; private set; }
        public int GamesNotCompleted { get; private set; }
        public int GamesWon { get; private set; }
        public int GamesLost { get; private set; }

        public GameStatistics() 
        {
            GamesPlayed = 0;
            GamesNotCompleted = 0;
            GamesWon = 0;
        }

        public void IncreaseGamesPlayed()
        {
            GamesPlayed++;
        }

        public void IncreaseGamesNotCompleted()
        {
            GamesNotCompleted++;
        }

        public void IncreaseGamesWon()
        {
            GamesWon++;
        }

        public void IncreaseGamesLost()
        {
            GamesLost++;
        }
    }
}
