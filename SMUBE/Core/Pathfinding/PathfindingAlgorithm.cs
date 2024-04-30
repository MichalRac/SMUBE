using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMUBE.Pathfinding
{
    public abstract class PathfindingAlgorithm
    {
        public static List<SMUBEVector2<int>> DirtyPositionCache = new List<SMUBEVector2<int>>();
        
        public class PathfindingPathCache
        {
            public BattleScenePosition Position { get; }
            public bool WasVisited { get; set; } = false;
            public int ShortestDistance { get; private set; } = int.MaxValue;
            public bool IsReachable => ShortestDistance < int.MaxValue;
            private List<BattleScenePosition> ShortestKnownPathBackingField { get; set; } = new List<BattleScenePosition>();
            public List<BattleScenePosition> ShortestKnownPath
            {
                get => ShortestKnownPathBackingField;
                set
                {
                    ShortestKnownPathBackingField = value;
                    ShortestDistance = GetPathCost(ShortestKnownPathBackingField);
                }
            }

            public PathfindingPathCache(BattleScenePosition position)
            {
                Position = position;
            }

            public bool IsDirty => ShortestKnownPath.Any(pathNode => DirtyPositionCache.Contains(pathNode.Coordinates));
        }

        protected readonly static float SINGLE_STEP_RANGE = 1f;
        protected readonly static int STRAIGHT_COST_APPROXIMATION = 10;
        protected readonly static int DIAGONAL_COST_APPROXIMATION = 14;

        public abstract bool TryFindPathFromTo(GridBattleScene battleScene, BattleScenePosition start,
            BattleScenePosition target, out List<BattleScenePosition> path, out int visitedNodesCount);
        
        public bool IsNextTo(GridBattleScene battleScene, BattleScenePosition start, BattleScenePosition target)
        {
            var potentialTargets = GetSurroundingPositions(battleScene, target, true);
            foreach (var potentialTarget in potentialTargets)
            {
                if (potentialTarget.Coordinates == start.Coordinates)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanGetNextTo(GridBattleScene battleScene, BattleScenePosition start, BattleScenePosition target, out List<BattleScenePosition> validPositions, int maxSteps = int.MaxValue)
        {
            var reachablePositions = GetAllReachablePositions(battleScene, start, maxSteps);
            var potentialTargets = GetSurroundingPositions(battleScene, target, true);

            validPositions = new List<BattleScenePosition>();

            foreach (var potentialTarget in potentialTargets)
            {
                foreach (var reachablePosition in reachablePositions)
                {
                    if(potentialTarget.Coordinates == reachablePosition.Coordinates)
                    {
                        validPositions.Add(potentialTarget); 
                        break;
                    }
                }
            }

            return validPositions.Count > 0;
        }

        public List<BattleScenePosition> GetSurroundingPositions(GridBattleScene battleScene, BattleScenePosition position, bool onlyEmpty)
        {
            var result = new List<BattleScenePosition>();

            for (int xDelta = -1; xDelta <= 1; xDelta++)
            {
                for (int yDelta = -1; yDelta <= 1; yDelta++)
                {
                    if (xDelta == 0 && yDelta == 0)
                    {
                        continue;
                    }

                    var checkPos = new SMUBEVector2<int>(position.Coordinates.x + xDelta, position.Coordinates.y + yDelta);

                    if (!battleScene.IsValid(checkPos))
                    {
                        continue;
                    }

                    if(onlyEmpty && !battleScene.IsEmpty(checkPos))
                    {
                        continue;
                    }

                    result.Add(battleScene.Grid[checkPos.x, checkPos.y]);
                }
            }

            return result;
        }

        public List<BattleScenePosition> GetAllReachablePositions(GridBattleScene battleScene, BattleScenePosition position, int maxSteps = int.MaxValue)
        {
            return GetAllReachablePaths(battleScene, position, maxSteps).Select(p => p.Position).ToList();
        }
        
        public List<PathfindingPathCache> GetAllReachablePaths(GridBattleScene battleScene, BattleScenePosition position, int maxSteps = int.MaxValue)
        {
            var maxDistance = (maxSteps * SINGLE_STEP_RANGE);
            var allNodes = new PathfindingPathCache[battleScene.Width, battleScene.Height];
            var reachableNodes = new List<PathfindingPathCache>();

            for (int i = 0; i < battleScene.Width; i++)
            {
                for (int j = 0; j < battleScene.Height; j++)
                {
                    allNodes[i, j] = new PathfindingPathCache(battleScene.Grid[i, j]);
                }
            }

            GreedyPathfindingLoop(battleScene, position, allNodes, reachableNodes, maxDistance);

            return reachableNodes.Where(n => n.ShortestDistance <= maxDistance).ToList();
        }

        private void GreedyPathfindingLoop(GridBattleScene battleScene, BattleScenePosition position, PathfindingPathCache[,] allNodes, List<PathfindingPathCache> reachableNodes, float maxDistance)
        {
            var currentNode = allNodes[position.Coordinates.x, position.Coordinates.y];
            currentNode.ShortestKnownPath = new List<BattleScenePosition>() { currentNode.Position };

            while (currentNode != null)
            {
                for (int xDelta = -1; xDelta <= 1; xDelta++)
                {
                    if (currentNode.WasVisited)
                    {
                        break;
                    }

                    for (int yDelta = -1; yDelta <= 1; yDelta++)
                    {
                        
                        if (xDelta == 0 && yDelta == 0)
                        {
                            continue;
                        }

                        var moveTargetPos = new SMUBEVector2<int>(currentNode.Position.Coordinates.x + xDelta, currentNode.Position.Coordinates.y + yDelta);
                        
                        if (!battleScene.IsValid(moveTargetPos) || !battleScene.IsEmpty(moveTargetPos))
                        {
                            continue;
                        }

                        var moveTarget = allNodes[moveTargetPos.x, moveTargetPos.y];

                        if (moveTarget == null)
                        {
                            continue;
                        }

                        var pathSoFar = new List<BattleScenePosition>(currentNode.ShortestKnownPath) { moveTarget.Position };

                        if (moveTarget.ShortestDistance == int.MaxValue)
                        {
                            moveTarget.ShortestKnownPath = pathSoFar;
                        }
                        else if (pathSoFar.Count <= moveTarget.ShortestDistance)
                        {
                            moveTarget.ShortestKnownPath = GetShorterPath(pathSoFar, moveTarget.ShortestKnownPath);
                        }
                    }
                }

                currentNode.WasVisited = true;

                reachableNodes.Add(currentNode);

                PathfindingPathCache nextEvaluatedNode = null;
                foreach (var node in allNodes)
                {
                    if (node.WasVisited)
                    {
                        continue;
                    }

                    if (node.ShortestDistance == int.MaxValue)
                    {
                        continue;
                    }

                    if(node.ShortestDistance > maxDistance)
                    {
                        continue;
                    }

                    if (nextEvaluatedNode == null)
                    {
                        nextEvaluatedNode = node;
                        continue;
                    }

                    if (node.ShortestDistance < nextEvaluatedNode.ShortestDistance)
                    {
                        nextEvaluatedNode = node;
                    }
                }

                if (nextEvaluatedNode == null)
                {
                    break;
                }
                else
                {
                    currentNode = nextEvaluatedNode;
                }
            }
        }

        public List<PathfindingPathCache> UpdateAllReachablePaths(List<PathfindingPathCache> knownPaths, GridBattleScene battleScene, BattleScenePosition position, int maxSteps = int.MaxValue)
        {
            var maxDistance = (maxSteps * SINGLE_STEP_RANGE);
            var allNodes = new PathfindingPathCache[battleScene.Width, battleScene.Height];
            var reachableNodes = new List<PathfindingPathCache>();

            for (int i = 0; i < battleScene.Width; i++)
            {
                for (int j = 0; j < battleScene.Height; j++)
                {
                    allNodes[i, j] = new PathfindingPathCache(battleScene.Grid[i, j]);
                }
            }
            
            foreach (var knownPathNode in knownPaths)
            {
                // if a node or it's any neighbour has changed, recalculate any paths going through them
                var surrounding = GetSurroundingPositions(battleScene, knownPathNode.Position, true);
                if (knownPathNode.IsDirty || surrounding.Any(node => DirtyPositionCache.Contains(node.Coordinates)))
                {
                    continue;
                }

                allNodes[knownPathNode.Position.Coordinates.x, knownPathNode.Position.Coordinates.y] = knownPathNode;
            }
            
            GreedyPathfindingLoop(battleScene, position, allNodes, reachableNodes, maxDistance);
            
            return reachableNodes.Where(n => n.ShortestDistance <= maxDistance).ToList();
        }

        protected List<BattleScenePosition> GetShorterPath(List<BattleScenePosition> pathA, List<BattleScenePosition> pathB)
        {
            var pathADistance = GetPathCost(pathA);
            var pathBDistance = GetPathCost(pathB);

            return pathADistance < pathBDistance ? pathA : pathB;
        }

        public static int GetPathCost(List<BattleScenePosition> path)
        {
            int cost = 0;

            if(path == null || path.Count == 0) 
            {
                return cost;
            }

            var previousEntry = path[0];
            for (int i = 1; i < path.Count; i++)
            {
                var horizontalStep = Math.Abs(previousEntry.Coordinates.x - path[i].Coordinates.x) != 0;
                var verticallStep = Math.Abs(previousEntry.Coordinates.y - path[i].Coordinates.y) != 0;

                if (horizontalStep && verticallStep)
                {
                    cost += DIAGONAL_COST_APPROXIMATION;
                }
                else
                {
                    cost += STRAIGHT_COST_APPROXIMATION;
                }

                previousEntry = path[i];
            }

            return cost;
        }

        protected (int straight, int diagonal) GetSimpleStepsCount(BattleScenePosition start, BattleScenePosition target)
        {
            var currentX = start.Coordinates.x;
            var currentY = start.Coordinates.y;

            var targetX = target.Coordinates.x;
            var targetY = target.Coordinates.y;
            
            var stepsStraight = 0;
            var stepsDiagonal = 0;

            while(currentX != targetX || currentY != targetY)
            {
                bool horizontal = false;
                bool vertical = false;

                if (currentX < targetX)
                {
                    currentX++;
                    horizontal = true;
                }
                else if (currentX > targetX)
                {
                    currentX--;
                    horizontal = true;
                }

                if (currentY < targetY)
                {
                    currentY++;
                    vertical = true;
                }
                else if (currentY > targetY)
                {
                    currentY--;
                    vertical = true;
                }

                if(horizontal && vertical)
                {
                    stepsDiagonal++;
                }
                else
                {
                    stepsStraight++;
                }
            }

            return (stepsStraight, stepsDiagonal);
        }

        /*
        protected void CacheLearnedPaths(BattleScenePosition start, List<PathfindingPositionCache> learnedPositionCaches)
        {
            if (PathfindingCache.TryGetValue(start, out var knownPathsForPos))
            {
                if (knownPathsForPos.Count >= learnedPositionCaches.Count)
                {
                    return;
                }
                
                knownPathsForPos.Clear();
                knownPathsForPos.AddRange(learnedPositionCaches);
            }
            else
            {
                var pathfindingPositionCaches = new List<PathfindingPositionCache>();
                pathfindingPositionCaches.AddRange(learnedPositionCaches);

                PathfindingCache.Add(start, pathfindingPositionCaches);
            }
        }

        public void ClearCache()
        {
            PathfindingCache.Clear();
        }
    */
    }
}
