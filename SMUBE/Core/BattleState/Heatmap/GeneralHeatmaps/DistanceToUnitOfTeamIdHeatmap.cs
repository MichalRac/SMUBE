﻿using System;
using System.Linq;
using SMUBE.DataStructures.Units;

namespace SMUBE.BattleState.Heatmap.GeneralHeatmaps
{
    public class DistanceToUnitOfTeamIdHeatmap : BaseHeatmap
    {
        protected override int PrefillValue => int.MaxValue;
        
        private readonly int _teamId;
        private readonly bool _aggregated;
        private readonly UnitIdentifier[] _ignored;

        public DistanceToUnitOfTeamIdHeatmap(int teamId, bool aggregated, BattleStateModel battleStateModel, params UnitIdentifier[] ignored) : base(battleStateModel)
        {
            _teamId = teamId;
            _aggregated = aggregated;
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
                
                var unitPaths = battleStateModel.BattleSceneState
                    .PathfindingHandler.AllUnitPaths[unitOfTeamId.UnitData.UnitIdentifier];

                foreach (var pathCache in unitPaths)
                {
                    var distance = pathCache.ShortestDistance;

                    if (distance == 0)
                    {
                        continue;
                    }

                    var xIndex = pathCache.Position.Coordinates.x;
                    var yIndex = pathCache.Position.Coordinates.y;

                    if (_aggregated)
                    {
                        if (Heatmap[xIndex][yIndex] == int.MaxValue)
                        {
                            Heatmap[xIndex][yIndex] = 0;
                        }

                        Set(xIndex, yIndex, Heatmap[xIndex][yIndex] + distance);
                    }
                    else
                    {
                        if (distance < Heatmap[xIndex][yIndex])
                        {
                            Set(xIndex, yIndex, distance);
                        }
                    }
                }
            }
        }

        public override ConsoleColor GetDebugConsoleColor(int value)
        {
            var color = ConsoleColor.White;
            if (value > 14)
                color = ConsoleColor.DarkGray;
            if (value > 30)
                color = ConsoleColor.DarkYellow;
            if (value > 50)
                color = ConsoleColor.Blue;
            if (value > 75)
                color = ConsoleColor.DarkBlue;
            if (value == int.MaxValue)
                color = ConsoleColor.DarkRed;
            return color;
        }

        public override string GetDebugMessage(int value)
        {
            return value < int.MaxValue
                ? $"ally distance: {value}"
                : "unreachable!";
        }
    }
}