using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Utils;
using System.Collections.Generic;

namespace SMUBE.Pathfinding
{
    public class AStarPathfindingAlgorithm : PathfindingAlgorithm
    {
        private class AStarPathCache : PathfindingPathCache
        {
            public int EstimatedPathDistance { get; set; } = int.MaxValue;

            public AStarPathCache(BattleScenePosition position) : base(position){ }
        }

        public override bool TryFindPathFromTo(GridBattleScene battleScene, BattleScenePosition start,
            BattleScenePosition target, out List<BattleScenePosition> path, out int visitedNodesCount)
        {
            path = null;
            visitedNodesCount = 0;
            var allNodes = new AStarPathCache[battleScene.Width, battleScene.Height];

            for (int i = 0; i < battleScene.Width; i++)
            {
                for (int j = 0; j < battleScene.Height; j++)
                {
                    allNodes[i, j] = new AStarPathCache(battleScene.Grid[i, j]);
                }
            }

            var currentNode = allNodes[start.Coordinates.x, start.Coordinates.y];
            currentNode.ShortestKnownPath = new List<BattleScenePosition>();
            var targetNode = allNodes[target.Coordinates.x, target.Coordinates.y];

            while (!targetNode.WasVisited)
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

                        // set distance from start
                        if (allNodes[moveTargetPos.x, moveTargetPos.y].ShortestDistance == int.MaxValue)
                        {
                            allNodes[moveTargetPos.x, moveTargetPos.y].ShortestKnownPath = pathSoFar;
                        }
                        else if (pathSoFar.Count <= allNodes[moveTargetPos.x, moveTargetPos.y].ShortestDistance)
                        {
                            allNodes[moveTargetPos.x, moveTargetPos.y].ShortestKnownPath
                                = GetShorterPath(pathSoFar, allNodes[moveTargetPos.x, moveTargetPos.y].ShortestKnownPath);
                        }

                        // set estimated remaining distance
                        if (allNodes[moveTargetPos.x, moveTargetPos.y].EstimatedPathDistance == int.MaxValue)
                        {
                            var estimatedCostRemaining = GetEstimatedCostRemaining(allNodes[moveTargetPos.x, moveTargetPos.y].Position, targetNode.Position);
                            allNodes[moveTargetPos.x, moveTargetPos.y].EstimatedPathDistance = estimatedCostRemaining;
                        }
                        else if (pathSoFar.Count <= allNodes[moveTargetPos.x, moveTargetPos.y].ShortestDistance)
                        {
                            var estimatedCostRemaining = GetEstimatedCostRemaining(allNodes[moveTargetPos.x, moveTargetPos.y].Position, targetNode.Position);

                            if(estimatedCostRemaining < allNodes[moveTargetPos.x, moveTargetPos.y].EstimatedPathDistance)
                            {
                                allNodes[moveTargetPos.x, moveTargetPos.y].EstimatedPathDistance = estimatedCostRemaining;
                            }
                        }
                    }
                }

                currentNode.WasVisited = true;
                visitedNodesCount++;

                AStarPathCache nextEvaluatedNode = null;
                foreach (var node in allNodes)
                {
                    if (node.WasVisited)
                    {
                        continue;
                    }

                    if (node.EstimatedPathDistance == int.MaxValue)
                    {
                        continue;
                    }

                    if (nextEvaluatedNode == null)
                    {
                        nextEvaluatedNode = node;
                        continue;
                    }

                    if (node.EstimatedPathDistance < nextEvaluatedNode.EstimatedPathDistance)
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

            if (targetNode.EstimatedPathDistance == int.MaxValue)
            {
                return false;
            }

            path = targetNode.ShortestKnownPath;
            return true;
        }

        public int GetEstimatedCostRemaining(BattleScenePosition start, BattleScenePosition target)
        {
            var steps = GetSimpleStepsCount(start, target);

            return steps.straight * STRAIGHT_COST_APPROXIMATION + steps.diagonal * DIAGONAL_COST_APPROXIMATION;
        }
    }
}
