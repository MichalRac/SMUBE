using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;

namespace SMUBE.Pathfinding
{
    public class PathfindingHandler
    {
        private PathfindingAlgorithm Pathfinding { get; } = new AStarPathfindingAlgorithm();
        private BattleScenePosition _activeUnitPosition;
        
        private List<PathfindingAlgorithm.PathfindingPathCache> _activeUnitReachablePositions = new List<PathfindingAlgorithm.PathfindingPathCache>();
        public IReadOnlyList<PathfindingAlgorithm.PathfindingPathCache> ActiveUnitReachablePositions => _activeUnitReachablePositions;
        

        private Dictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>> _allUnitPaths = new Dictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>>();
        public IReadOnlyDictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>> AllUnitPaths => _allUnitPaths;
        
        
        private Dictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>> _allUnitReachablePaths = new Dictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>>();
        public IReadOnlyDictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>> AllUnitReachablePaths => _allUnitReachablePaths;

        public void Initialize(BattleStateModel battleState)
        {
            EvaluateAllUnitReachablePositions(battleState);
            EvaluateAllPaths(battleState);
        }
        
        public void OnNewTurn(BattleStateModel battleState)
        {
            // todo some paths are recalculated, can be optimized
            EvaluateAllUnitReachablePositions(battleState);
            EvaluateAllPaths(battleState);
        }

        private void EvaluateAllUnitReachablePositions(BattleStateModel battleState)
        {
            _activeUnitReachablePositions.Clear();

            var unit = battleState.ActiveUnit;
            _activeUnitPosition = unit.UnitData.BattleScenePosition;
            
            var reachablePositions
                = Pathfinding.GetAllReachablePaths(battleState.BattleSceneState, unit.UnitData.BattleScenePosition, unit.UnitData.UnitStats.Speed);
            
            _activeUnitReachablePositions = reachablePositions;
            
            foreach (var currentUnit in battleState.Units)
            {
                var allPaths = Pathfinding.GetAllReachablePaths(battleState.BattleSceneState, currentUnit.UnitData.BattleScenePosition, unit.UnitData.UnitStats.Speed);
                _allUnitReachablePaths[currentUnit.UnitData.UnitIdentifier] = allPaths;
            }
        }

        private void EvaluateAllPaths(BattleStateModel battleState)
        {
            foreach (var unit in battleState.Units)
            {
                var allPaths = Pathfinding.GetAllReachablePaths(battleState.BattleSceneState, unit.UnitData.BattleScenePosition);
                _allUnitPaths[unit.UnitData.UnitIdentifier] = allPaths;
            }
        }
        
        public List<BattleScenePosition> GetSurroundingPositions(BattleStateModel battleState, BattleScenePosition target)
        {
            return Pathfinding.GetSurroundingPositions(battleState.BattleSceneState, target, true);
        }

        public List<BattleScenePosition> GetReachableSurroundingPositions(BattleStateModel battleState, BattleScenePosition target)
        {
            var result = new List<BattleScenePosition>();
            
            var surroundingPositions = GetSurroundingPositions(battleState, target);
            foreach (var surroundingPosition in surroundingPositions)
            {
                if (_activeUnitReachablePositions.Any(reachable => reachable.Position.Coordinates.Equals(surroundingPosition.Coordinates)))
                {
                    result.Add(surroundingPosition);
                }
            }

            return result;
        }

        public bool GetLastReachableOnPath(BattleStateModel battleState, BattleScenePosition target, out BattleScenePosition lastReachable, out int pathCost)
        {
            lastReachable = null;
            pathCost = int.MaxValue;
            
            var reachable = PathExists(battleState, _activeUnitPosition, target, out var path);
            if (!reachable)
            {
                return false;
            }
            else
            {
                pathCost = PathfindingAlgorithm.GetPathCost(path);
                for (var i = path.Count - 1; i >= 0; i--)
                {
                    if (_activeUnitReachablePositions.Any(activeReachablePos => activeReachablePos.Position.Coordinates.Equals(path[i].Coordinates)))
                    {
                        lastReachable = path[i];
                        return true;
                    }
                }
            }

            return false;
        }
        
        public bool PathExists(BattleStateModel battleState, BattleScenePosition start, BattleScenePosition target, out List<BattleScenePosition> path)
        {
            return Pathfinding.TryFindPathFromTo(battleState.BattleSceneState, start, target, out path, out _);
        }

        public bool IsNextTo(BattleStateModel battleState, BattleScenePosition start, BattleScenePosition target)
        {
            return Pathfinding.IsNextTo(battleState.BattleSceneState, start, target);
        }
    }
}