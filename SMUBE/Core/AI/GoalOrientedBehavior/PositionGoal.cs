using SMUBE.BattleState;
using SMUBE.BattleState.Heatmap.GeneralHeatmaps;
using SMUBE.DataStructures.Units;

namespace SMUBE.AI.GoalOrientedBehavior
{
    public class PositionGoal : Goal
    {
        private readonly float _healthRiskThreshold;
        protected override float Importance => 70;

        public PositionGoal(float healthRiskThreshold)
        {
            _healthRiskThreshold = healthRiskThreshold;
        }
        
        public override float GetDiscontentment(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            var activeUnitStats = battleStateModel.ActiveUnit.UnitData.UnitStats;
            var healthPercentage = (float)activeUnitStats.CurrentHealth / activeUnitStats.MaxHealth;

            if (healthPercentage > _healthRiskThreshold)
            {
                return GetScoreByDistanceToEnemy(battleStateModel);
            }
            else
            {
                return GetScoreBySafety(battleStateModel);
            }
        }

        public int GetScoreBySafety(BattleStateModel battleStateModel)
        {
            var enemyTeamId = battleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var distanceToEnemyHeatmap = new DistanceToUnitOfTeamIdHeatmap(enemyTeamId, false, battleStateModel);
            distanceToEnemyHeatmap.ProcessHeatmap(battleStateModel);

            const int minValue = 10;

            var activeUnitCoordinates = battleStateModel.ActiveUnit.UnitData.BattleScenePosition.Coordinates;
            var distanceToEnemy = distanceToEnemyHeatmap.Heatmap[activeUnitCoordinates.x][activeUnitCoordinates.y] - minValue;

            if (distanceToEnemy > 50)
                return (int)(1 * Importance);
            else
                return (int) (((float)distanceToEnemy / 50) * Importance);
        }

        public int GetScoreByDistanceToEnemy(BattleStateModel battleStateModel)
        {
            var enemyTeamId = battleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var distanceToEnemyHeatmap = new DistanceToUnitOfTeamIdHeatmap(enemyTeamId, false, battleStateModel, battleStateModel.ActiveUnit.UnitData.UnitIdentifier);
            distanceToEnemyHeatmap.ProcessHeatmap(battleStateModel);

            const int minValue = 10;

            var activeUnitCoordinates = battleStateModel.ActiveUnit.UnitData.BattleScenePosition.Coordinates;
            var distanceToEnemy = distanceToEnemyHeatmap.Heatmap[activeUnitCoordinates.x][activeUnitCoordinates.y] - minValue;
            
            return (int) ((1 / (float)distanceToEnemy) * Importance);
        }
    }
}