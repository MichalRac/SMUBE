using System;
using SMUBE.BattleState;
using SMUBE.BattleState.Heatmap.GeneralHeatmaps;
using SMUBE.Units;

namespace SMUBE.AI.QLearning.States
{
    public class QLearningPositionStateComponent : BaseQLearningStateComponent
    {
        // Far from enemy / Barely Outside enemy range / In Enemy Range / Among Enemies

        public QLearningPositionStateComponent(int id) 
            : base(id) { }

        protected override long GetNonUniqueStateValue(BattleStateModel stateModel, Unit actor)
        {
            var opponentTeamId = actor.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;

            var reachableByEnemiesHeatmap = new CountReachingUnitsOfTeamIdHeatmap(opponentTeamId, stateModel, true);
            reachableByEnemiesHeatmap.ProcessHeatmap(stateModel);

            var unitPosition = actor.UnitData.BattleScenePosition;
            var countOfEnemiesReachingUnit = reachableByEnemiesHeatmap.Heatmap[unitPosition.Coordinates.x][unitPosition.Coordinates.y];
            
            if (countOfEnemiesReachingUnit == 0)
            {
                var surroundingPositions = stateModel.BattleSceneState.PathfindingHandler.GetSurroundingPositions(stateModel, unitPosition);
                foreach (var surroundingPosition in surroundingPositions)
                {
                    if (reachableByEnemiesHeatmap.Heatmap[surroundingPosition.Coordinates.x][surroundingPosition.Coordinates.y] > 0)
                    {
                        return 1; // Barely Outside Enemy Range
                    }
                }
                return 0; // Far from enemy
            }
            else if(countOfEnemiesReachingUnit == 1)
            {
                return 2; // In Enemy range
            }
            else if(countOfEnemiesReachingUnit > 1)
            {
                return 3; // Among Enemies
            }
            else
                throw new ArgumentException();
        }
        
        internal override string GetValueWithDescriptions(BattleStateModel stateModel, Unit actor)
        {
            var value = GetNonUniqueStateValue(stateModel, actor);
            switch (value)
            {
                case 0:
                    return $"PersonalPosition - {value}: FarFromEnemy";
                case 1:
                    return $"PersonalPosition - {value}: BarelyOutsideEnemyRange";
                case 2:
                    return $"PersonalPosition - {value}: InEnemyRange";
                case 3:
                    return $"PersonalPosition - {value}: AmongEnemies";
                default:
                    throw new ArgumentException();
            }
        }
    }
}