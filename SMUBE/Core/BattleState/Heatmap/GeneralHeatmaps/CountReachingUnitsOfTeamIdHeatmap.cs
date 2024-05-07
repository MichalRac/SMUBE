using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.BattleState.Heatmap.GeneralHeatmaps
{
    public class CountReachingUnitsOfTeamIdHeatmap : BaseHeatmap
    {
        private readonly int _teamId;
        private readonly bool _includeNeighbours;
        private readonly UnitIdentifier[] _ignored;

        public CountReachingUnitsOfTeamIdHeatmap(int teamId, BattleStateModel battleStateModel, bool includeNeighbours, params UnitIdentifier[] ignored) : base(battleStateModel)
        {
            _teamId = teamId;
            _includeNeighbours = includeNeighbours;
            _ignored = ignored;
        }
        
        public override void ProcessHeatmap(BattleStateModel battleStateModel)
        {
            base.ProcessHeatmap(battleStateModel);

            Dictionary<SMUBEVector2<int>, HashSet<UnitIdentifier>> reachingUnits = new Dictionary<SMUBEVector2<int>, HashSet<UnitIdentifier>>();
            foreach (var unitOfTeamId in battleStateModel.Units.Where(u => u.UnitData.UnitIdentifier.TeamId.Equals(_teamId)))
            {
                if (_ignored != null && _ignored.Contains(unitOfTeamId.UnitData.UnitIdentifier))
                {
                    continue;
                }
                
                var unitPaths = battleStateModel.BattleSceneState
                    .PathfindingHandler.ReachablePathCacheSets[unitOfTeamId.UnitData.UnitIdentifier];
                
                foreach (var pathCache in unitPaths.Data)
                {
                    if (pathCache == null || pathCache.ShortestDistance == 0 || pathCache.ShortestDistance == int.MaxValue)
                    {
                        continue;
                    }
                    
                    var coordinates = pathCache.TargetPosition.Coordinates;

                    if (!reachingUnits.ContainsKey(coordinates))
                    {
                        reachingUnits[coordinates] = new HashSet<UnitIdentifier>();
                    }

                    for (int xOffset = -1; xOffset <= 1; xOffset++)
                    {
                        for (int yOffset = -1; yOffset <= 1; yOffset++)
                        {
                            var reachingCoordinates = new SMUBEVector2<int>(coordinates.x + xOffset, coordinates.y + yOffset);

                            if (!battleStateModel.BattleSceneState.IsValid(reachingCoordinates))
                            {
                                continue;
                            }
                            
                            if (!reachingUnits.ContainsKey(reachingCoordinates))
                            {
                                reachingUnits[reachingCoordinates] = new HashSet<UnitIdentifier>();
                            }
                            
                            reachingUnits[reachingCoordinates].Add(unitOfTeamId.UnitData.UnitIdentifier);
                        }
                    }

                    Set(coordinates.x, coordinates.y, Heatmap[coordinates.x][coordinates.y] + 1);
                }
            }
            
            if(!_includeNeighbours) return;

            foreach (var posReachingUnits in reachingUnits)
            {
                var coord = posReachingUnits.Key;
                Heatmap[coord.x][coord.y] = posReachingUnits.Value.Count;
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