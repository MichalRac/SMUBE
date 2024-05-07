using System;
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

        public Dictionary<UnitIdentifier, PathfindingAlgorithm.PathfindingPathCacheSet> PathCacheSets =
            new Dictionary<UnitIdentifier, PathfindingAlgorithm.PathfindingPathCacheSet>();
        
        public Dictionary<UnitIdentifier, PathfindingAlgorithm.PathfindingPathCacheSet> ReachablePathCacheSets =
            new Dictionary<UnitIdentifier, PathfindingAlgorithm.PathfindingPathCacheSet>();
        
        private bool initialized = false;
        
        private void Initialize(BattleStateModel battleState)
        {
            EvaluateAllPaths(battleState);
            EvaluateAllUnitReachablePositions(battleState);

            _activeUnitPosition = battleState.ActiveUnit.UnitData.BattleScenePosition;
            _activeUnitReachablePositions = _allUnitReachablePaths[battleState.ActiveUnit.UnitData.UnitIdentifier];
        }
        
        public void OnNewTurn(BattleStateModel battleState)
        {
            if (!initialized)
            {
                initialized = true;
                Initialize(battleState);
                return;
            }
            
            _activeUnitPosition = battleState.ActiveUnit.UnitData.BattleScenePosition;
            _activeUnitReachablePositions = _allUnitReachablePaths[battleState.ActiveUnit.UnitData.UnitIdentifier];

            if(!PathfindingAlgorithm.DirtyPositionCache.Any())
                return;
            
            UpdateAllPaths(battleState);
            EvaluateAllUnitReachablePositions(battleState);

            PathfindingAlgorithm.DirtyPositionCache.Clear();
        }

        private void EvaluateAllUnitReachablePositions(BattleStateModel battleState)
        {
            foreach (var currentUnit in battleState.Units)
            {
                var reachablePaths = Pathfinding.TrimByMaxSteps(
                    _allUnitPaths[currentUnit.UnitData.UnitIdentifier], 
                    currentUnit.UnitData.UnitStats.Speed);
                
                _allUnitReachablePaths[currentUnit.UnitData.UnitIdentifier] = reachablePaths;
                
                var width = battleState.BattleSceneState.Width;
                var height = battleState.BattleSceneState.Height;
                var allPathsSet = new PathfindingAlgorithm.PathfindingPathCacheSet(width, height);

                foreach (var path in reachablePaths)
                {
                    var coordinates = path.TargetPosition.Coordinates;
                    allPathsSet.Data[coordinates.x, coordinates.y] = path;
                }
                ReachablePathCacheSets[currentUnit.UnitData.UnitIdentifier] = allPathsSet;
            }
        }
        
        private void EvaluateAllPaths(BattleStateModel battleState)
        {
            foreach (var currentUnit in battleState.Units)
            {
                var allPaths = Pathfinding.ProcessAllPaths(battleState.BattleSceneState, currentUnit.UnitData.BattleScenePosition);
                _allUnitPaths[currentUnit.UnitData.UnitIdentifier] = allPaths;
                
                var width = battleState.BattleSceneState.Width;
                var height = battleState.BattleSceneState.Height;
                var allPathsSet = new PathfindingAlgorithm.PathfindingPathCacheSet(width, height);

                foreach (var path in allPaths)
                {
                    var coordinates = path.TargetPosition.Coordinates;
                    allPathsSet.Data[coordinates.x, coordinates.y] = path;
                }
                PathCacheSets[currentUnit.UnitData.UnitIdentifier] = allPathsSet;
            }
        }

        private void UpdateAllPaths(BattleStateModel battleStateModel)
        {
            foreach (var unit in battleStateModel.Units)
            {
                var knownPaths = _allUnitPaths[unit.UnitData.UnitIdentifier];
                _allUnitPaths[unit.UnitData.UnitIdentifier] = Pathfinding.UpdatePaths(knownPaths, battleStateModel.BattleSceneState, unit.UnitData.BattleScenePosition);
                
                var width = battleStateModel.BattleSceneState.Width;
                var height = battleStateModel.BattleSceneState.Height;
                var allPathsSet = new PathfindingAlgorithm.PathfindingPathCacheSet(width, height);

                foreach (var path in _allUnitPaths[unit.UnitData.UnitIdentifier])
                {
                    var coordinates = path.TargetPosition.Coordinates;
                    allPathsSet.Data[coordinates.x, coordinates.y] = path;
                }
                PathCacheSets[unit.UnitData.UnitIdentifier] = allPathsSet;
            }
        }
        
        public List<BattleScenePosition> GetSurroundingPositions(BattleStateModel battleState, BattleScenePosition target, bool onlyEmpty = true)
        {
            return Pathfinding.GetSurroundingPositions(battleState.BattleSceneState, target, onlyEmpty);
        }

        public List<BattleScenePosition> GetReachableSurroundingPositions(BattleStateModel battleState, BattleScenePosition target)
        {
            var result = new List<BattleScenePosition>();
            
            var surroundingPositions = GetSurroundingPositions(battleState, target);
            foreach (var surroundingPosition in surroundingPositions)
            {
                if (_activeUnitReachablePositions.Any(reachable => reachable.TargetPosition.Coordinates.Equals(surroundingPosition.Coordinates)))
                {
                    result.Add(surroundingPosition);
                }
            }

            return result;
        }
        
        public List<PathfindingAlgorithm.PathfindingPathCache> GetSurroundingPathCache(BattleStateModel battleState, BattleScenePosition target, bool includeOutOfReach = false)
        {
            var result = new List<PathfindingAlgorithm.PathfindingPathCache>();
            
            var surroundingPositions = GetSurroundingPositions(battleState, target);
            foreach (var surroundingPosition in surroundingPositions)
            {
                var pathCacheToCheck = includeOutOfReach 
                    ? _allUnitPaths[battleState.ActiveUnit.UnitData.UnitIdentifier] 
                    : _activeUnitReachablePositions;
                
                foreach (var reachablePosition in pathCacheToCheck)
                {
                    if (reachablePosition.TargetPosition.Coordinates.Equals(surroundingPosition.Coordinates))
                    {
                        result.Add(reachablePosition);
                    }
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
                    if (_activeUnitReachablePositions.Any(activeReachablePos => activeReachablePos.TargetPosition.Coordinates.Equals(path[i].Coordinates)))
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
        
        public int GetRequiredMovementTurns(PathfindingAlgorithm.PathfindingPathCache target, int speed)
        {
            return Pathfinding.GetRequiredMovementTurns(target, speed);
        }
    }
}