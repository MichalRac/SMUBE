using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using SMUBE.Units;

namespace SMUBE.AI.QLearning.States
{
    public class QLearningTeamPositionStateComponent : BaseQLearningStateComponent
    {
        // Far / Independent Battles / Under Assault / Assaulting / Full Battle
 
        public QLearningTeamPositionStateComponent(int id) 
            : base(id) { }

        protected override long GetNonUniqueStateValue(BattleStateModel stateModel, Unit actor)
        {
            var teamId = actor.UnitData.UnitIdentifier.TeamId;
            
            // build groups of units
            var groups = new List<HashSet<UnitIdentifier>>();
            foreach (var unit in stateModel.Units)
            {
                var unitId = unit.UnitData.UnitIdentifier;
                if (groups.Count > 0 && groups.Any(group => group.Any(u => u.Equals(unitId))))
                {
                    // already evaluated
                    continue;
                }

                var newGroup = new HashSet<UnitIdentifier>();
                newGroup.Add(unitId);
                EvaluateGroup(stateModel, unit, newGroup);
                groups.Add(newGroup);
            }
            
            if (groups.Count == 1)
            {
                return 4; // Full Battle
            }

            bool fightFound = false;
            var advantageDifference = 0;
            foreach (var group in groups)
            {
                var groupTeamCount = 0;
                var opponentTeamCount = 0;
                foreach (var groupMember in group)
                {
                    if (groupMember.TeamId == teamId)
                        groupTeamCount++;
                    else
                        opponentTeamCount++;
                }
                    
                // only same team members
                if (groupTeamCount == group.Count || opponentTeamCount == group.Count)
                {
                    continue;
                }

                fightFound = true;
                    
                // update advantage
                if (groupTeamCount > opponentTeamCount)
                    advantageDifference++;
                else if (groupTeamCount < opponentTeamCount)
                    advantageDifference--;
            }

            if (fightFound)
            {
                if (advantageDifference < 0)
                {
                    return 2; // under assault
                }

                if (advantageDifference == 0)
                {
                    return 1; // independent battles
                }

                if (advantageDifference > 0)
                {
                    return 3; // assault
                }
            }
            else
            {
                return 0; // far
            }

            return 0;
        }

        private void EvaluateGroup(BattleStateModel stateModel, Unit unit, HashSet<UnitIdentifier> group)
        {
            // find all neighbours
            var reachableUnits = GetAllUnitsInReach(stateModel, unit);
            // remove already evaluated
            reachableUnits = reachableUnits.Where(u => !group.Contains(u.UnitData.UnitIdentifier)).ToList();
            // add and recursive search remaining neighbours
            foreach (var reachableUnit in reachableUnits)
            {
                group.Add(reachableUnit.UnitData.UnitIdentifier);
                EvaluateGroup(stateModel, reachableUnit, group);
            }
        }

        private List<Unit> GetAllUnitsInReach(BattleStateModel stateModel, Unit startUnit)
        {
            var reachable = new List<Unit>();

            foreach (var targetUnit in stateModel.Units)
            {
                var targetUnitId = targetUnit.UnitData.UnitIdentifier;
                if (startUnit.UnitData.UnitIdentifier.Equals(targetUnitId))
                {
                    continue;
                }

                var surroundingPositions = 
                    stateModel.BattleSceneState.PathfindingHandler
                        .GetSurroundingPositions(stateModel, targetUnit.UnitData.BattleScenePosition);
                var reachablePathCache = stateModel.BattleSceneState.PathfindingHandler
                    .GetAllReachablePathCacheSetsForUnit(stateModel, startUnit.UnitData.UnitIdentifier);
                
                foreach (var surroundingPosition in surroundingPositions)
                {
                    var pathCache = reachablePathCache.Data[surroundingPosition.Coordinates.x, surroundingPosition.Coordinates.y];
                    if (pathCache != null)
                    {
                        reachable.Add(targetUnit);
                        break;
                    }
                }
            }

            return reachable;
        }

        internal override string GetValueWithDescriptions(BattleStateModel stateModel, Unit actor)
        {
            var value = GetNonUniqueStateValue(stateModel, actor);
            return ValueToDescription(value);
        }

        internal override string ValueToDescription(long value)
        {
            switch (value)
            {
                case 0:
                    return $"TeamsPosition - {value}: Far";
                case 1:
                    return $"TeamsPosition - {value}: IndependentBattles";
                case 2:
                    return $"TeamsPosition - {value}: UnderAssault";
                case 3:
                    return $"TeamsPosition - {value}: Assaulting";
                case 4:
                    return $"TeamsPosition - {value}: FullBattle";
                default:
                    throw new ArgumentException();
            }
        }
    }
}