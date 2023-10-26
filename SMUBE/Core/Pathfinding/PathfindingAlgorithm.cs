using SMUBE.BattleState;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Pathfinding
{
    public abstract class PathfindingAlgorithm
    {
        protected class PathfindingPositionCache
        {
            public BattleScenePosition Position { get; }
            public bool WasVisited { get; set; } = false;
            public int ShortestDistance { get; private set; } = int.MaxValue;
            private List<BattleScenePosition> ShortestKnownPathBackingField { get; set; } = new List<BattleScenePosition>();
            public List<BattleScenePosition> ShortestKnownPath
            {
                get => ShortestKnownPathBackingField;
                set
                {
                    ShortestKnownPathBackingField = value;
                    ShortestDistance = ShortestKnownPathBackingField.Count;
                }
            }

            public PathfindingPositionCache(BattleScenePosition position)
            {
                Position = position;
            }
        }


        protected readonly static int STRAIGHT_COST_APPROXIMATION = 10;
        protected readonly static int DIAGONAL_COST_APPROXIMATION = 14;

        public abstract bool TryFindPathFromTo(GridBattleScene battleScene, BattleScenePosition start,
            BattleScenePosition target, out List<BattleScenePosition> path, out int visitedNodesCount);

        public List<BattleScenePosition> GetAllReachablePositions(GridBattleScene battleScene, BattleScenePosition position, int maxSteps = int.MaxValue)
        {
            var allNodes = new PathfindingPositionCache[battleScene.Width, battleScene.Height];
            var reachableNodes = new List<PathfindingPositionCache>();

            for (int i = 0; i < battleScene.Width; i++)
            {
                for (int j = 0; j < battleScene.Height; j++)
                {
                    allNodes[i, j] = new PathfindingPositionCache(battleScene.Grid[i, j]);
                }
            }

            var currentNode = allNodes[position.Coordinates.x, position.Coordinates.y];
            currentNode.ShortestKnownPath = new List<BattleScenePosition>();

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

                        var moveTargetPos = new SMUBEVector2<int>(currentNode.Position.Coordinates.x + xDelta, currentNode.Position.Coordinates.y + yDelta);
                        if (!battleScene.IsValid(moveTargetPos) || !battleScene.IsEmpty(moveTargetPos))
                        {
                            continue;
                        }
                        if (allNodes[moveTargetPos.x, moveTargetPos.y] == null)
                        {
                            continue;
                        }

                        var pathSoFar = new List<BattleScenePosition>(currentNode.ShortestKnownPath) { currentNode.Position };

                        if (allNodes[moveTargetPos.x, moveTargetPos.y].ShortestDistance == int.MaxValue)
                        {
                            allNodes[moveTargetPos.x, moveTargetPos.y].ShortestKnownPath = pathSoFar;
                        }
                        else if (pathSoFar.Count <= allNodes[moveTargetPos.x, moveTargetPos.y].ShortestDistance)
                        {
                            allNodes[moveTargetPos.x, moveTargetPos.y].ShortestKnownPath
                                = GetShorterPath(pathSoFar, allNodes[moveTargetPos.x, moveTargetPos.y].ShortestKnownPath);
                        }
                    }
                }

                currentNode.WasVisited = true;

                reachableNodes.Add(currentNode);

                PathfindingPositionCache nextEvaluatedNode = null;
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

            var reachablePositions = reachableNodes.Where(n => n.ShortestDistance <= maxSteps).ToList();

            return reachablePositions.Select(n => n.Position).ToList();
        }

        protected List<BattleScenePosition> GetShorterPath(List<BattleScenePosition> pathA, List<BattleScenePosition> pathB)
        {
            var pathADistance = GetPathCost(pathA);
            var pathBDistance = GetPathCost(pathB);

            return pathADistance < pathBDistance ? pathA : pathB;
        }

        protected int GetPathCost(List<BattleScenePosition> path)
        {
            int cost = 0;

            var previousEntry = path[0];
            for (int i = 0; i < path.Count; i++)
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

    }
}
