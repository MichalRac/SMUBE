using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMUBE.Pathfinding
{
    public abstract class PathfindingAlgorithm
    {
        public static List<(SMUBEVector2<int> pos, bool emptyAfter)> DirtyPositionCache = new List<(SMUBEVector2<int>, bool)>();
        
        public class PathfindingPathCache
        {
            public BattleScenePosition StartPosition { get; }
            public BattleScenePosition TargetPosition { get; }
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

            public PathfindingPathCache(BattleScenePosition startPosition, BattleScenePosition targetPosition)
            {
                StartPosition = startPosition;
                TargetPosition = targetPosition;
            }

            public bool IsDirty => ShortestKnownPath.Any(pathNode => DirtyPositionCache.Any(pos => pos.pos.Equals(pathNode.Coordinates)));
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
            var allPaths = ProcessAllPaths(battleScene, start);
            var reachablePositions = TrimByMaxSteps(allPaths, maxSteps)
                .Select(reachableNode => reachableNode.TargetPosition).ToList();

            
            //var reachablePositions = GetAllReachablePositions(battleScene, start, maxSteps);
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

        public List<BattleScenePosition> GetSurroundingPositions(GridBattleScene battleScene, BattleScenePosition position, bool onlyEmpty, bool onlyWalkable = false)
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

                    if (onlyWalkable && !battleScene.IsWalkable(checkPos))
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

        /*
        public List<BattleScenePosition> GetAllReachablePositions(GridBattleScene battleScene, BattleScenePosition position, int maxSteps = int.MaxValue)
        {
            return GetAllReachablePaths(battleScene, position, maxSteps).Select(p => p.TargetPosition).ToList();
        }
        */
        
        /*
        public List<PathfindingPathCache> GetAllReachablePaths(GridBattleScene battleScene, BattleScenePosition position, int maxSteps = int.MaxValue)
        {
            var maxDistance = (maxSteps * SINGLE_STEP_RANGE);
            var allPaths = ProcessAllPaths(battleScene, position);
            return allPaths.Where(n => n.ShortestDistance <= maxDistance).ToList();
            
            /*
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

            return allPaths.Where(n => n.ShortestDistance <= maxDistance).ToList();
        #1#
        }
        */

        public List<PathfindingPathCache> TrimByMaxSteps(List<PathfindingPathCache> allPaths, int maxSteps = int.MaxValue)
        {
            var maxDistance = (maxSteps * SINGLE_STEP_RANGE);
            return allPaths.Where(n => n.ShortestDistance <= maxDistance).ToList();
        }
        
        public List<PathfindingPathCache> ProcessAllPaths(GridBattleScene battleScene, BattleScenePosition startPosition)
        {
            var allNodes = new PathfindingPathCache[battleScene.Width, battleScene.Height];
            var reachableNodes = new List<PathfindingPathCache>();

            for (int i = 0; i < battleScene.Width; i++)
            {
                for (int j = 0; j < battleScene.Height; j++)
                {
                    allNodes[i, j] = new PathfindingPathCache(startPosition, battleScene.Grid[i, j]);
                }
            }
            
            var currentNode = allNodes[startPosition.Coordinates.x, startPosition.Coordinates.y];
            currentNode.ShortestKnownPath = new List<BattleScenePosition>() { currentNode.TargetPosition };

            while (true)
            {
                EvaluateCurrentNode(battleScene, currentNode, allNodes);

                currentNode.WasVisited = true;
                reachableNodes.Add(currentNode);

                currentNode = TryFindNextNode(allNodes);

                // if no more nodes to evaluate
                if (currentNode == null)
                    break;
            }

            return reachableNodes;
        }
        
        public List<PathfindingPathCache> UpdatePaths(List<PathfindingPathCache> knownPaths, GridBattleScene battleScene, BattleScenePosition startPosition)
        {
            if (knownPaths == null || knownPaths.Count == 0 || !knownPaths.First().StartPosition.Coordinates.Equals(startPosition.Coordinates))
            {
                return ProcessAllPaths(battleScene, startPosition);
            }
            
            var allNodes = new PathfindingPathCache[battleScene.Width, battleScene.Height];
            var reachableNodes = new List<PathfindingPathCache>();
            var reopenedNodes = new HashSet<SMUBEVector2<int>>();
            
            foreach (var knownPath in knownPaths)
            {
                var isValid = true;
                foreach (var dirtyCacheElement in DirtyPositionCache)
                {
                    // if any node on path was closed, ignore path
                    if (!dirtyCacheElement.emptyAfter 
                        && knownPath.ShortestKnownPath.Any(pathElement => pathElement.Coordinates.Equals(dirtyCacheElement.pos)))
                    {
                        isValid = false;
                        break;
                    }

                    // for opened nodes, check their neighbours, if part of known path, tag neighbour node for revisiting
                    if (dirtyCacheElement.emptyAfter)
                    {
                        var pos = battleScene.Grid[dirtyCacheElement.pos.x, dirtyCacheElement.pos.y];
                        var neighbours = GetSurroundingPositions(battleScene, pos, false, onlyWalkable: true);

                        foreach (var pathElement in knownPath.ShortestKnownPath)
                        {
                            foreach (var dirtyElementNeighbour in neighbours)
                            {
                                if (pathElement.Coordinates.Equals(dirtyElementNeighbour.Coordinates))
                                {
                                    reopenedNodes.Add(pathElement.Coordinates);
                                }
                            }
                        }
                    }
                }

                // assign node path cache to known paths
                if (isValid)
                {
                    allNodes[knownPath.TargetPosition.Coordinates.x, knownPath.TargetPosition.Coordinates.y] = knownPath;
                }
            }

            foreach (var reopenedNode in reopenedNodes)
            {
                if (allNodes[reopenedNode.x, reopenedNode.y] != null)
                {
                    allNodes[reopenedNode.x, reopenedNode.y].WasVisited = false;
                }
            }

            for (int i = 0; i < battleScene.Width; i++)
            {
                for (int j = 0; j < battleScene.Height; j++)
                {
                    if(allNodes[i, j] == null)
                        allNodes[i, j] = new PathfindingPathCache(startPosition, battleScene.Grid[i, j]);
                }
            }

            var currentNode = TryFindNextNode(allNodes);
            while (currentNode != null)
            {
                EvaluateCurrentNode(battleScene, currentNode, allNodes);

                currentNode.WasVisited = true;
                //reachableNodes.Add(currentNode);

                currentNode = TryFindNextNode(allNodes);

                // if no more nodes to evaluate
                if (currentNode == null)
                    break;
            }

            foreach (var node in allNodes)
            {
                if (node.IsReachable)
                {
                    reachableNodes.Add(node);
                }
            }
            return reachableNodes;
        }

        private static void EvaluateCurrentNode(GridBattleScene battleScene, PathfindingPathCache currentNode, PathfindingPathCache[,] allNodes)
        {
            for (int xDelta = -1; xDelta <= 1; xDelta++)
            {
                for (int yDelta = -1; yDelta <= 1; yDelta++)
                {
                    if (xDelta == 0 && yDelta == 0)
                        continue;

                    var moveTargetPos = new SMUBEVector2<int>(
                        currentNode.TargetPosition.Coordinates.x + xDelta, 
                        currentNode.TargetPosition.Coordinates.y + yDelta);
                        
                    if (!battleScene.IsValid(moveTargetPos) || !battleScene.IsEmpty(moveTargetPos))
                        continue;

                    var moveTarget = allNodes[moveTargetPos.x, moveTargetPos.y];
                    var pathSoFar = new List<BattleScenePosition>(currentNode.ShortestKnownPath) { moveTarget.TargetPosition };
                    var newDistance = GetPathCost(pathSoFar);

                    if (moveTarget.ShortestDistance == int.MaxValue)
                    {
                        moveTarget.ShortestKnownPath = pathSoFar;
                    }
                    else if (newDistance < moveTarget.ShortestDistance)
                    {
                        //moveTarget.ShortestKnownPath = GetShorterPath(pathSoFar, moveTarget.ShortestKnownPath);
                        moveTarget.ShortestKnownPath = pathSoFar;
                        // if already evaluated, it needs to be reevaluated due to shorter path being found
                        moveTarget.WasVisited = false;
                    }
                }
            }
        }

        private static PathfindingPathCache TryFindNextNode(PathfindingPathCache[,] allNodes)
        {
            PathfindingPathCache nextEvaluatedNode = null;
            foreach (var node in allNodes)
            {
                // already evaluated (and no shorter path to it found)
                if (node.WasVisited)
                    continue;
                    
                // no known path
                if (node.ShortestDistance == int.MaxValue)
                    continue;
                    
                // if no next node found yet, set it as potential next node
                if (nextEvaluatedNode == null)
                {
                    nextEvaluatedNode = node;
                    continue;
                }

                // find node with shortest current distance
                if (node.ShortestDistance < nextEvaluatedNode.ShortestDistance)
                {
                    nextEvaluatedNode = node;
                }
            }

            // return node with shortest distance as the next one to be evaluated / null if not found
            return nextEvaluatedNode;
        }

        private void GreedyPathfindingLoop(GridBattleScene battleScene, BattleScenePosition position, PathfindingPathCache[,] allNodes, List<PathfindingPathCache> reachableNodes, float maxDistance)
        {
            var currentNode = allNodes[position.Coordinates.x, position.Coordinates.y];
            currentNode.ShortestKnownPath = new List<BattleScenePosition>() { currentNode.TargetPosition };

            while (currentNode != null)
            {
                for (int xDelta = -1; xDelta <= 1; xDelta++)
                {
                    for (int yDelta = -1; yDelta <= 1; yDelta++)
                    {
                        if (xDelta == 0 && yDelta == 0)
                        {
                            continue;
                        }

                        var moveTargetPos = new SMUBEVector2<int>(currentNode.TargetPosition.Coordinates.x + xDelta, currentNode.TargetPosition.Coordinates.y + yDelta);
                        
                        if (!battleScene.IsValid(moveTargetPos) || !battleScene.IsEmpty(moveTargetPos))
                        {
                            continue;
                        }

                        var moveTarget = allNodes[moveTargetPos.x, moveTargetPos.y];

                        if (moveTarget == null)
                        {
                            continue;
                        }

                        var pathSoFar = new List<BattleScenePosition>(currentNode.ShortestKnownPath) { moveTarget.TargetPosition };

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
                        if (node.IsReachable && node.ShortestDistance < maxDistance && !reachableNodes.Any(n => n.TargetPosition.Coordinates.Equals(node.TargetPosition.Coordinates)))
                        {
                            reachableNodes.Add(node);
                        }
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

        public List<PathfindingPathCache> UpdateAllReachablePaths(List<PathfindingPathCache> knownPaths, GridBattleScene battleScene, BattleScenePosition startPosition, int maxSteps = int.MaxValue)
        {
            var maxDistance = (maxSteps * SINGLE_STEP_RANGE);
            var allNodes = new PathfindingPathCache[battleScene.Width, battleScene.Height];
            var reachableNodes = new List<PathfindingPathCache>();

            for (int i = 0; i < battleScene.Width; i++)
            {
                for (int j = 0; j < battleScene.Height; j++)
                {
                    allNodes[i, j] = new PathfindingPathCache(startPosition, battleScene.Grid[i, j]);
                }
            }
            
            foreach (var knownPathNode in knownPaths)
            {
                // if a node or it's any neighbour has changed, recalculate any paths going through them
                if (knownPathNode.IsDirty)
                {
                    continue;
                }                
                
                var surrounding = GetSurroundingPositions(battleScene, knownPathNode.TargetPosition, true);
                if (surrounding.Any(node => DirtyPositionCache.Any(cache => cache.pos.Equals(node.Coordinates))))
                {
                    knownPathNode.WasVisited = false;
                }
                
                allNodes[knownPathNode.TargetPosition.Coordinates.x, knownPathNode.TargetPosition.Coordinates.y] = knownPathNode;
            }
            
            GreedyPathfindingLoop(battleScene, startPosition, allNodes, reachableNodes, maxDistance);
            
            return reachableNodes.Where(n => n.ShortestDistance <= maxDistance).ToList();
        }

        protected List<BattleScenePosition> GetShorterPath(List<BattleScenePosition> pathA, List<BattleScenePosition> pathB)
        {
            var pathADistance = GetPathCost(pathA);
            var pathBDistance = GetPathCost(pathB);

            return pathADistance <= pathBDistance ? pathA : pathB;
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
