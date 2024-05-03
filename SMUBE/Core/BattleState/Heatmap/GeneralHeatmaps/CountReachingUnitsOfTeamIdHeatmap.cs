using System;
using System.Linq;
using SMUBE.DataStructures.Units;

namespace SMUBE.BattleState.Heatmap.GeneralHeatmaps
{
    public class CountReachingUnitsOfTeamIdHeatmap : BaseHeatmap
    {
        private readonly int _teamId;
        private readonly UnitIdentifier[] _ignored;

        public CountReachingUnitsOfTeamIdHeatmap(int teamId, BattleStateModel battleStateModel, params UnitIdentifier[] ignored) : base(battleStateModel)
        {
            _teamId = teamId;
            _ignored = ignored;
        }
        
        public override void ProcessHeatmap(BattleStateModel battleStateModel)
        {
            base.ProcessHeatmap(battleStateModel);

            foreach (var unitOfTeamId in battleStateModel.Units.Where(u => u.UnitData.UnitIdentifier.TeamId.Equals(_teamId)))
            {
                if (_ignored != null && _ignored.Contains(unitOfTeamId.UnitData.UnitIdentifier))
                {
                    continue;
                }
                
                var unitReachablePaths = battleStateModel.BattleSceneState
                    .PathfindingHandler.AllUnitReachablePaths[unitOfTeamId.UnitData.UnitIdentifier];
                
                foreach (var pathCache in unitReachablePaths)
                {
                    if (pathCache.ShortestDistance == 0)
                    {
                        continue;
                    }

                    var coordinates = pathCache.TargetPosition.Coordinates;
                    Set(coordinates.x, coordinates.y, Heatmap[coordinates.x][coordinates.y] + 1);
                }
            }
        }

        public override ConsoleColor GetDebugConsoleColor(int value)
        {
            var color = ConsoleColor.White;
            if (value == PrefillValue)
                color = ConsoleColor.Red;
            if (value > 1)
                color = ConsoleColor.Yellow;
            if (value > 2)
                color = ConsoleColor.Green;
            if (value > 3)
                color = ConsoleColor.DarkGreen;
            return color;
        }

        public override string GetDebugMessage(int value)
        {
            return $"ReachedBy:{value}";
        }
    }
}