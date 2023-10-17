using SMUBE.AI;
using SMUBE.AI.StateMachine;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Units
{
    public static class UnitHelper
    {
        public static long HunterCount = 0;
        public static long SquireCount = 0;
        public static long ScholarCount = 0;
        private static Random _randomCache;
        private static Random Random => _randomCache ?? (_randomCache = new Random());


        private static Dictionary<int, int> teamCounts = new Dictionary<int, int>();

        public static Unit CreateRandomUnit(int teamId, AIModel aiModel, bool useSimpleBehavior = false)
        {
            var rng = Random;

            switch (rng.Next(3))
            {
                case 0:
                    ScholarCount++;
                    return CreateUnit<Scholar>(teamId, aiModel, useSimpleBehavior);
                case 1:
                    SquireCount++;
                    return CreateUnit<Squire>(teamId, aiModel, useSimpleBehavior);
                case 2:
                    HunterCount++;
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

            if(aiModel is StateMachineAIModel)
            {
                aiModel = StateMachineConfig.GetStateMachineForArchetype(newUnitStats.BaseCharacter, useSimpleBehavior);
            }

            return new Unit(newUnitName, newUnitIdentifier, newUnitStats, aiModel, useSimpleBehavior ? null : baseUnit.AvailableCommands);
        }

        public static BaseCharacter CreateCharacter<T>() where T : BaseCharacter, new()
        {
            return new T();
        }
    }
}
