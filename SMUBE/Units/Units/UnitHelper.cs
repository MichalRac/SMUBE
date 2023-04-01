using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Units
{
    public static class UnitHelper
    {
        private static Dictionary<int, int> teamCounts = new Dictionary<int, int>();

        public static Unit CreateUnit<T>(int teamId) where T : BaseCharacter, new()
        {
            if (!teamCounts.ContainsKey(teamId))
            {
                teamCounts.Add(teamId, 0);
            }

            var newUnitIdentifier = new UnitIdentifier(teamCounts[teamId], teamId); 
            var newUnitStats = CreateCharacter<T>().UnitStats;
            var newUnitName = nameof(T) + $"_team{newUnitIdentifier.TeamId}_id{newUnitIdentifier.PersonalId}";

            teamCounts[teamId] = teamCounts[teamId] + 1;

            return new Unit(newUnitName, newUnitIdentifier, newUnitStats);
        }

        public static BaseCharacter CreateCharacter<T>() where T : BaseCharacter, new()
        {
            return new T();
        }
    }
}
