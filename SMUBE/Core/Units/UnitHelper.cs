using SMUBE.AI;
using SMUBE.AI.StateMachine;
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

        public static Unit CreateUnit<T>(int teamId, AIModel aiModel) where T : BaseCharacter, new()
        {
            if (!teamCounts.ContainsKey(teamId))
            {
                teamCounts.Add(teamId, 0);
            }

            var newUnitIdentifier = new UnitIdentifier(teamCounts[teamId], teamId);
            var baseUnit = CreateCharacter<T>();
            var newUnitStats = baseUnit.DefaultStats;
            var newUnitName = nameof(T) + $"_team{newUnitIdentifier.TeamId}_id{newUnitIdentifier.PersonalId}";

            teamCounts[teamId] = teamCounts[teamId] + 1;

            if(aiModel is StateMachineAIModel)
            {
                aiModel = StateMachineConfig.GetStateMachineForArchetype(newUnitStats.BaseCharacter);
            }

            return new Unit(newUnitName, newUnitIdentifier, newUnitStats, aiModel, baseUnit.AvailableCommands);
        }

        public static BaseCharacter CreateCharacter<T>() where T : BaseCharacter, new()
        {
            return new T();
        }
    }
}
