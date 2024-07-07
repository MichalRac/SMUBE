using SMUBE.AI;
using SMUBE.AI.StateMachine;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using SMUBE.Core;

namespace SMUBE.Units
{
    public static class UnitHelper
    {
        private static Dictionary<int, int> teamCounts = new Dictionary<int, int>();

        public static Unit CreateRandomUnit(int teamId, AIModel aiModel, bool useSimpleBehavior = false)
        {
            switch (RngProvider.Next(3))
            {
                case 0:
                    return CreateUnit<Scholar>(teamId, aiModel, useSimpleBehavior);
                case 1:
                    return CreateUnit<Squire>(teamId, aiModel, useSimpleBehavior);
                case 2:
                    return CreateUnit<Hunter>(teamId, aiModel, useSimpleBehavior);
            }

            return null;
        }

        public static Unit CreateUnit<T>(int teamId, AIModel aiModel, bool useSimpleBehavior = false) where T : BaseCharacter, new()
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

            if(aiModel is StateMachineAIModel fsmAi)
            {
                if (fsmAi.IsCompetent)
                {
                    aiModel = StateMachineConfig.GetCompetentStateMachine();
                }
                else
                {
                    aiModel = StateMachineConfig.GetStateMachineForArchetype(newUnitStats.BaseCharacter, useSimpleBehavior);
                }
            }

            return new Unit(newUnitName, newUnitIdentifier, newUnitStats, aiModel, useSimpleBehavior ? null : baseUnit.AvailableCommands);
        }

        public static BaseCharacter CreateCharacter<T>() where T : BaseCharacter, new()
        {
            return new T();
        }
    }
}
