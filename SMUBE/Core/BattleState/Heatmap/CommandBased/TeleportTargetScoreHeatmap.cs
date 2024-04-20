using System;
using SMUBE.BattleState.Heatmap.GeneralHeatmaps;

namespace SMUBE.BattleState.Heatmap.CommandBased
{
    public class TeleportTargetScoreHeatmap : BaseHeatmap
    {
        protected override int PrefillValue => 0;

        public TeleportTargetScoreHeatmap(BattleStateModel battleStateModel) : base(battleStateModel)
        {
        }

        public override void ProcessHeatmap(BattleStateModel battleStateModel)
        {
            base.ProcessHeatmap(battleStateModel);

            var activeUnitId = battleStateModel.ActiveUnit.UnitData.UnitIdentifier;
            var opponentTeamId = activeUnitId.TeamId == 0 ? 1 : 0;

            var reachablePositionsHeatmap = 
                new GetReachablePositionsHeatmap(activeUnitId, battleStateModel);
            var distanceToUnitOfTeamIdHeatmap = 
                new DistanceToUnitOfTeamIdHeatmap(opponentTeamId, false, battleStateModel);
            var countReachingUnitsOfTeamIdHeatmap =
                new CountReachingUnitsOfTeamIdHeatmap(opponentTeamId, battleStateModel);

            reachablePositionsHeatmap.ProcessHeatmap(battleStateModel);
            distanceToUnitOfTeamIdHeatmap.ProcessHeatmap(battleStateModel);
            countReachingUnitsOfTeamIdHeatmap.ProcessHeatmap(battleStateModel);
            
            Heatmap = HeatmapHelper.ReprocessHeatmaps(distanceToUnitOfTeamIdHeatmap, 
                countReachingUnitsOfTeamIdHeatmap, 
                reachablePositionsHeatmap, 
                TeleportScore).Heatmap;

            return;

            int TeleportScore(int distanceToAnyEnemyUnit, int countOfEnemyUnitsReachingPosition, int reachablePosition)
            {
                if (distanceToAnyEnemyUnit == int.MaxValue)
                {
                    return -1;
                }

                var score = 1_000;
                    
                if (countOfEnemyUnitsReachingPosition == 0)
                {
                    score /= (distanceToAnyEnemyUnit * 100);
                }
                else
                {
                    score /= (distanceToAnyEnemyUnit * (countOfEnemyUnitsReachingPosition * 4));
                }
                
                if (reachablePosition == 1)
                {
                    score /= 4;
                }

                return score;
            }
        }

        public override ConsoleColor GetDebugConsoleColor(int value)
        {
            var color = ConsoleColor.White;
            if (value == 0)
                color = ConsoleColor.DarkRed;
            if (value > 0)
                color = ConsoleColor.Red;
            if (value > 100)
                color = ConsoleColor.DarkYellow;
            if (value > 250)
                color = ConsoleColor.Yellow;
            if (value > 500)
                color = ConsoleColor.Blue;
            if (value > 750)
                color = ConsoleColor.Green;
            return color;
        }

        public override string GetDebugMessage(int value)
        {
            return $"score:{value}";
        }
    }
}