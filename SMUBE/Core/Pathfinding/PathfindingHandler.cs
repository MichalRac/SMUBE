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
        private List<UnitIdentifier> _initializedPaths = new List<UnitIdentifier>();
        private BattleScenePosition _activeUnitPosition;
        public PathfindingAlgorithm Pathfinding { get; } = new AStarPathfindingAlgorithm();
        
        public List<(SMUBEVector2<int> pos, bool emptyAfter)> AggregatedDirtyPositionCache = new List<(SMUBEVector2<int>, bool)>();
        public Dictionary<UnitIdentifier, List<(SMUBEVector2<int> pos, bool emptyAfter)>> PersonalDirtyPositionCache 
            = new Dictionary<UnitIdentifier, List<(SMUBEVector2<int> pos, bool emptyAfter)>>();
        
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

        public PathfindingHandler(){}

        private PathfindingHandler(PathfindingHandler sourcePathfindingHandler)
        {
            
            initialized = sourcePathfindingHandler.initialized;
            Pathfinding = sourcePathfindingHandler.Pathfinding;
            _activeUnitPosition = sourcePathfindingHandler._activeUnitPosition.DeepCopy();
            
            _initializedPaths = new List<UnitIdentifier>();
            foreach (var initializedPath in sourcePathfindingHandler._initializedPaths)
            {
                _initializedPaths.Add(initializedPath);
            }

            AggregatedDirtyPositionCache = new List<(SMUBEVector2<int> pos, bool emptyAfter)>();
            foreach (var item in sourcePathfindingHandler.AggregatedDirtyPositionCache)
            {
                AggregatedDirtyPositionCache.Add((new SMUBEVector2<int>(item.pos.x, item.pos.y), item.emptyAfter));
            }

            PersonalDirtyPositionCache = new Dictionary<UnitIdentifier, List<(SMUBEVector2<int> pos, bool emptyAfter)>>();
            foreach (var item in sourcePathfindingHandler.PersonalDirtyPositionCache)
            {
                if (item.Value != null)
                {
                    var unitCache = new List<(SMUBEVector2<int> pos, bool emptyAfter)>();
                    foreach (var cache in item.Value)
                    {
                        unitCache.Add((new SMUBEVector2<int>(cache.pos.x, cache.pos.y), cache.emptyAfter));
                    }
                    PersonalDirtyPositionCache.Add(item.Key, unitCache);
                }
            }

            _allUnitPaths = new Dictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>>();
            foreach (var item in sourcePathfindingHandler._allUnitPaths)
            {
                var unitPathCache = new List<PathfindingAlgorithm.PathfindingPathCache>();
                foreach (var cache in item.Value)
                {
                    unitPathCache.Add(cache.DeepCopy());
                }
                _allUnitPaths.Add(item.Key, unitPathCache);
            }
            
            _allUnitReachablePaths = new Dictionary<UnitIdentifier, List<PathfindingAlgorithm.PathfindingPathCache>>();
            foreach (var item in sourcePathfindingHandler._allUnitReachablePaths)
            {
                var unitPathCache = new List<PathfindingAlgorithm.PathfindingPathCache>();
                foreach (var cache in item.Value)
                {
                    unitPathCache.Add(cache.DeepCopy());
                }
                _allUnitReachablePaths.Add(item.Key, unitPathCache);
            }

            foreach (var pathCacheSet in sourcePathfindingHandler.PathCacheSets)
            {
                PathCacheSets.Add(pathCacheSet.Key, pathCacheSet.Value.DeepCopy());
            }
            foreach (var reachablePathCacheSet in sourcePathfindingHandler.ReachablePathCacheSets)
            {
                ReachablePathCacheSets.Add(reachablePathCacheSet.Key, reachablePathCacheSet.Value.DeepCopy());
            }
        }
        
        public PathfindingHandler DeepCopy()
        {
            return new PathfindingHandler(this);
        }
        
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

        public List<BattleScenePosition> GetReachableSurroundingPositions(BattleStateModel battleState, BattleScenePosition target, UnitIdentifier ignoredOccupant = null)
        {
            var result = new List<BattleScenePosition>();

            var allReachablePaths = GetAllReachablePathCacheSetsForUnit(battleState, battleState.ActiveUnit.UnitData.UnitIdentifier);
            var surroundingPositions = GetSurroundingPositions(battleState, target, false);

            foreach (var surroundingPosition in surroundingPositions)
            {
                var coordinate = surroundingPosition.Coordinates;
                var position = allReachablePaths.Data[coordinate.x, coordinate.y];
                
                if(position == null || position.TargetPosition == null)
                    continue;
                if (position.TargetPosition.UnitIdentifier != null)
                {
                    if (ignoredOccupant != null && !position.TargetPosition.UnitIdentifier.Equals(ignoredOccupant))
                    {
                        continue;
                    }
                    
                    /*
                    if(ignoredOccupant != null)
                    {
                        continue;
                    }
                    
                    if (position.StartPosition.Coordinates.Equals(position.TargetPosition.Coordinates))
                    {
                        result.Add(position.TargetPosition);
                        continue;
                    }
                */
                }
                if(!position.TargetPosition.IsWalkable())
                    continue;
                
                result.Add(position.TargetPosition);
            }

            return result;
            
            surroundingPositions = surroundingPositions
                .Where(pos => pos.IsWalkable() && (pos.UnitIdentifier == null 
                                                   || (ignoredOccupant != null && pos.UnitIdentifier.Equals(ignoredOccupant)))).ToList();
            foreach (var surroundingPosition in surroundingPositions)
            {
                if (GetAllReachablePathsForActiveUnit(battleState).Any(reachable => reachable.TargetPosition.Coordinates.Equals(surroundingPosition.Coordinates)))
                {
                    result.Add(surroundingPosition);
                }
            }

            return result;
        }
        
        public List<PathfindingAlgorithm.PathfindingPathCache> GetSurroundingPathCache(BattleStateModel battleState, BattleScenePosition target, bool includeOutOfReach = false, UnitIdentifier ignoredOccupant = null)
        {
            var result = new List<PathfindingAlgorithm.PathfindingPathCache>();
            
            var pathCacheToCheck = includeOutOfReach 
                ? GetAllPathCacheSetsForUnit(battleState, battleState.ActiveUnit.UnitData.UnitIdentifier) 
                : GetAllReachablePathCacheSetsForUnit(battleState, battleState.ActiveUnit.UnitData.UnitIdentifier);
            var surroundingPositions = GetSurroundingPositions(battleState, target, false);

            foreach (var surroundingPosition in surroundingPositions)
            {
                var coordinate = surroundingPosition.Coordinates;
                var position = pathCacheToCheck.Data[coordinate.x, coordinate.y];
                
                if(position == null || position.TargetPosition == null)
                    continue;
                if (position.TargetPosition.UnitIdentifier != null)
                {
                    if (ignoredOccupant != null && !position.TargetPosition.UnitIdentifier.Equals(ignoredOccupant))
                    {
                        continue;
                    }
                    
                    if(ignoredOccupant != null)
                    {
                        continue;
                    }
                }
                if(!position.TargetPosition.IsWalkable())
                    continue;
                
                result.Add(position);
            }

            return result;

            
            /*
            var surroundingPositions = GetSurroundingPositions(battleState, target, false);
            surroundingPositions = surroundingPositions
                .Where(pos => battleState.BattleSceneState.IsValid(pos.Coordinates) 
                              && pos.IsWalkable() 
                              && (pos.UnitIdentifier == null 
                                  || (ignoredOccupant != null && pos.UnitIdentifier.Equals(ignoredOccupant)))).ToList();
            foreach (var surroundingPosition in surroundingPositions)
            {
                foreach (var reachablePosition in pathCacheToCheck)
                {
                    if (reachablePosition.TargetPosition.Coordinates.Equals(surroundingPosition.Coordinates))
                    {
                        result.Add(reachablePosition);
                    }
                }
            }

            return result;
        */
        }
        
        public bool GetLastReachableOnPath(BattleStateModel battleState, BattleScenePosition target, out BattleScenePosition lastReachable, out int pathCost)
        {
            lastReachable = null;
            pathCost = int.MaxValue;

            var isReachable = false;
            var activeUnitPathCache = GetAllPathCacheSetsForUnit(battleState, battleState.ActiveUnit.UnitData.UnitIdentifier);
            var path = activeUnitPathCache.Data[target.Coordinates.x, target.Coordinates.y];
            
            if (path == null)
            {
                return false;
            }
            else
            {
                pathCost = PathfindingAlgorithm.GetPathCost(path.ShortestKnownPath);
                for (var i = path.ShortestKnownPath.Count - 1; i >= 0; i--)
                {
                    if (GetAllReachablePathsForActiveUnit(battleState).Any(activeReachablePos => activeReachablePos.TargetPosition.Coordinates.Equals(path.ShortestKnownPath[i].Coordinates)))
                    {
                        lastReachable = path.ShortestKnownPath[i];
                        return true;
                    }
                }
            }
            
            /*
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
            */

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