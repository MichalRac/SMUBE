using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Pathfinding
{
    public class PathfindingHandler
    {
        public List<(SMUBEVector2<int> pos, bool emptyAfter)> AggregatedDirtyPositionCache = new List<(SMUBEVector2<int>, bool)>();
        public Dictionary<UnitIdentifier, List<(SMUBEVector2<int> pos, bool emptyAfter)>> PersonalDirtyPositionCache 
            = new Dictionary<UnitIdentifier, List<(SMUBEVector2<int> pos, bool emptyAfter)>>();
        
        private List<UnitIdentifier> _initializedPaths = new List<UnitIdentifier>();
        
        public PathfindingAlgorithm Pathfinding { get; } = new AStarPathfindingAlgorithm();
        private BattleScenePosition _activeUnitPosition;
        
        private Dictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>> _allUnitPaths 
            = new Dictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>>();
        
        public List<PathfindingAlgorithm.PathfindingPathCache> GetAllPathsForUnit(BattleStateModel battleStateModel, UnitIdentifier unitIdentifier)
        {
            EnsureUnitPathsUpdated(battleStateModel, unitIdentifier);
            return _allUnitPaths[unitIdentifier];
        }

        private Dictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>> _allUnitReachablePaths 
            = new Dictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>>();

        public List<PathfindingAlgorithm.PathfindingPathCache> GetAllReachablePathsForUnit(BattleStateModel battleStateModel, UnitIdentifier unitIdentifier)
        {
            EnsureUnitPathsUpdated(battleStateModel, unitIdentifier);
            return _allUnitReachablePaths[unitIdentifier];
        }
        
        public List<PathfindingAlgorithm.PathfindingPathCache> GetAllReachablePathsForActiveUnit(BattleStateModel battleStateModel)
        {
            return GetAllReachablePathsForUnit(battleStateModel, battleStateModel.ActiveUnit.UnitData.UnitIdentifier);
        }

        private Dictionary<UnitIdentifier, PathfindingAlgorithm.PathfindingPathCacheSet> PathCacheSets =
            new Dictionary<UnitIdentifier, PathfindingAlgorithm.PathfindingPathCacheSet>();
        
        public PathfindingAlgorithm.PathfindingPathCacheSet GetAllPathCacheSetsForUnit(BattleStateModel battleStateModel, UnitIdentifier unitIdentifier)
        {
            EnsureUnitPathsUpdated(battleStateModel, unitIdentifier);
            return PathCacheSets[unitIdentifier];
        }
        
        private Dictionary<UnitIdentifier, PathfindingAlgorithm.PathfindingPathCacheSet> ReachablePathCacheSets =
            new Dictionary<UnitIdentifier, PathfindingAlgorithm.PathfindingPathCacheSet>();
        public PathfindingAlgorithm.PathfindingPathCacheSet GetAllReachablePathCacheSetsForUnit(BattleStateModel battleStateModel, UnitIdentifier unitIdentifier)
        {
            EnsureUnitPathsUpdated(battleStateModel, unitIdentifier);
            return ReachablePathCacheSets[unitIdentifier];
        }

        private bool initialized = false;

        private void Initialize(BattleStateModel battleState)
        {
            EvaluateAllPaths(battleState);
            EvaluateAllUnitReachablePositions(battleState);

            foreach (var unit in battleState.Units)
            {
                _initializedPaths.Add(unit.UnitData.UnitIdentifier);
            }
            
            _activeUnitPosition = battleState.ActiveUnit.UnitData.BattleScenePosition;
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

            if(!AggregatedDirtyPositionCache.Any())
                return;
            
            _initializedPaths.Clear();

            foreach (var units in battleState.Units)
            {
                var unitId = units.UnitData.UnitIdentifier;
                var didMove = false;

                if (!_allUnitPaths[unitId].Any())
                {
                    didMove = true;
                }
                else
                {
                    SMUBEVector2<int> previousCoordinates = _allUnitPaths[unitId].First().StartPosition.Coordinates;
                    SMUBEVector2<int> currentCoordinates = units.UnitData.BattleScenePosition.Coordinates;
                    didMove = !currentCoordinates.Equals(previousCoordinates);
                }

                if (didMove)
                {
                    _allUnitPaths[unitId].Clear();
                    _allUnitReachablePaths[unitId].Clear();
    
                    if(PersonalDirtyPositionCache.ContainsKey(unitId))
                        PersonalDirtyPositionCache[unitId] = null;
                }
                else
                {
                    if (!PersonalDirtyPositionCache.ContainsKey(unitId) || PersonalDirtyPositionCache[unitId] == null)
                    {
                        PersonalDirtyPositionCache[unitId] = new List<(SMUBEVector2<int> pos, bool emptyAfter)>();
                    }
                    
                    foreach (var newDirtyPositionCache in AggregatedDirtyPositionCache)
                    {
                        PersonalDirtyPositionCache[unitId].Add(newDirtyPositionCache);
                    }
                }
            }
            
            AggregatedDirtyPositionCache.Clear();
            
            EnsureUnitPathsUpdated(battleState, battleState.ActiveUnit.UnitData.UnitIdentifier);

            /*
            UpdateAllPaths(battleState);
            EvaluateAllUnitReachablePositions(battleState);
            */

        }
        
        private void EnsureUnitPathsUpdated(BattleStateModel battleStateModel, UnitIdentifier unitIdentifier)
        {
            if (!_initializedPaths.Any(ip => ip.Equals(unitIdentifier)))
            {
                UpdatePathsForUnit(battleStateModel, unitIdentifier);
                _initializedPaths.Add(unitIdentifier);
                if(PersonalDirtyPositionCache.TryGetValue(unitIdentifier, out var cacheList) && cacheList != null)
                    cacheList.Clear();
            }
        }

        private void UpdatePathsForUnit(BattleStateModel battleStateModel, UnitIdentifier unitIdentifier)
        {
            var unit = battleStateModel.Units.Find(u => u.UnitData.UnitIdentifier.Equals(unitIdentifier));
            
            // all paths
            {
                var dirtyPositionCache = PersonalDirtyPositionCache.TryGetValue(unitIdentifier, out var value) 
                    ? value
                    : null;
                var knownPaths = _allUnitPaths[unitIdentifier];
                _allUnitPaths[unitIdentifier] = Pathfinding.UpdatePaths(knownPaths, battleStateModel.BattleSceneState, 
                    unit.UnitData.BattleScenePosition, dirtyPositionCache);
            
                var width = battleStateModel.BattleSceneState.Width;
                var height = battleStateModel.BattleSceneState.Height;
                var allPathsSet = new PathfindingAlgorithm.PathfindingPathCacheSet(width, height);
                foreach (var path in _allUnitPaths[unitIdentifier])
                {
                    var coordinates = path.TargetPosition.Coordinates;
                    allPathsSet.Data[coordinates.x, coordinates.y] = path;
                }
                PathCacheSets[unitIdentifier] = allPathsSet;
            }
            
            // reachable paths
            {
                var reachablePaths = Pathfinding.TrimByMaxSteps(
                    _allUnitPaths[unitIdentifier], 
                    unit.UnitData.UnitStats.Speed);
                
                _allUnitReachablePaths[unitIdentifier] = reachablePaths;
                
                var width = battleStateModel.BattleSceneState.Width;
                var height = battleStateModel.BattleSceneState.Height;
                var allPathsSet = new PathfindingAlgorithm.PathfindingPathCacheSet(width, height);

                foreach (var path in reachablePaths)
                {
                    var coordinates = path.TargetPosition.Coordinates;
                    allPathsSet.Data[coordinates.x, coordinates.y] = path;
                }
                ReachablePathCacheSets[unit.UnitData.UnitIdentifier] = allPathsSet;
            }
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

        /*
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
        */
        
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
                if (GetAllReachablePathsForActiveUnit(battleState).Any(reachable => reachable.TargetPosition.Coordinates.Equals(surroundingPosition.Coordinates)))
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
                    ? GetAllPathsForUnit(battleState, battleState.ActiveUnit.UnitData.UnitIdentifier) 
                    : GetAllReachablePathsForActiveUnit(battleState);
                
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
                    if (GetAllReachablePathsForActiveUnit(battleState).Any(activeReachablePos => activeReachablePos.TargetPosition.Coordinates.Equals(path[i].Coordinates)))
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