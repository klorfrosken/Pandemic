using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Managers
{
    public class User
    {
        public int UserID { get; private set; }
        public string UserName { get; private set; }
        public Role CurrentRole = null;

        Dictionary<string, GameStatistics> RoleStatistics = new Dictionary<string, GameStatistics>
        {
            { "ContingencyPlanner", new GameStatistics() },
            { "Dispatcher", new GameStatistics() },
            { "Medic", new GameStatistics() },
            { "OperationsExpert", new GameStatistics() },
            { "QuarantineSpecialist", new GameStatistics() },
            { "Researcher", new GameStatistics() },
            { "Scientist", new GameStatistics() }
        };

        public User(int UserID, string UserName)
        {
            this.UserID = UserID;
            this.UserName = UserName;
        }

        public void UpdateStatistics(string WonLostError)
        {
            RoleStatistics[CurrentRole.RoleName].IncreaseGamesPlayed();
            if (WonLostError.Equals("Won"))
            {
                RoleStatistics[CurrentRole.RoleName].IncreaseGamesLost();
            }
            else if (WonLostError.Equals("Lost"))
            {
                RoleStatistics[CurrentRole.RoleName].IncreaseGamesPlayed();
            }
            else if (WonLostError.Equals("Error"))
            {
                RoleStatistics[CurrentRole.RoleName].IncreaseGamesNotCompleted();
            }
            else
            {
                throw new UnexpectedBehaviourException("An unexpected error occured during the updating of the statistics");
            }
        }
    }
}
