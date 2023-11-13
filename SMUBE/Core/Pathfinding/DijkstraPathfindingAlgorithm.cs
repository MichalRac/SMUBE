using SMUBE.BattleState;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Pathfinding
{
    internal class DijkstraPathfindingAlgorithm : PathfindingAlgorithm
    {
        public override bool TryFindPathFromTo(GridBattleScene battleScene, BattleScenePosition start,
            BattleScenePosition target, out List<BattleScenePosition> path, out int visitedNodesCount)
        {
            path = null;
            visitedNodesCount = 0;
            var allNodes = new PathfindingPositionCache[battleScene.Width, battleScene.Height];

            for (int i = 0; i < battleScene.Width; i++)
            {
                for (int j = 0; j < battleScene.Height; j++)
                {
                    allNodes[i, j] = new PathfindingPositionCache(battleScene.Grid[i,j]);
                }
            }

            var currentNode = allNodes[start.Coordinates.x, start.Coordinates.y];
            currentNode.ShortestKnownPath = new List<BattleScenePosition>();
            var targetNode = allNodes[target.Coordinates.x, target.Coordinates.y];

            while (!targetNode.WasVisited)
            {
                for (int xDelta = -1; xDelta <= 1; xDelta++)
                {
                    for (int yDelta = -1; yDelta <=1; yDelta++)
                    {
                        if (xDelta==0 && yDelta ==0)
                        {
                            continue;
                        }

                        var targetPos = new SMUBEVector2<int>(currentNode.Position.Coordinates.x + xDelta, currentNode.Position.Coordinates.y + yDelta);
                        if (!battleScene.IsValid(targetPos) || !battleScene.IsEmpty(targetPos))
                        {
                            continue;
                        }
                        if (allNodes[targetPos.x, targetPos.y] == null)
                        {
                            continue;
                        }

                        var pathSoFar = new List<BattleScenePosition>(currentNode.ShortestKnownPath){ currentNode.Position };

                        if(allNodes[targetPos.x, targetPos.y].ShortestDistance == int.MaxValue)
                        {
                            allNodes[targetPos.x, targetPos.y].ShortestKnownPath = pathSoFar;
                        }
                        else if (pathSoFar.Count <= allNodes[targetPos.x, targetPos.y].ShortestDistance)
                        {
                            allNodes[targetPos.x, targetPos.y].ShortestKnownPath 
                                = GetShorterPath(pathSoFar, allNodes[targetPos.x, targetPos.y].ShortestKnownPath);
                        }
                    }
                }

                currentNode.WasVisited = true;
                visitedNodesCount++;

                PathfindingPositionCache nextEvaluatedNode = null;
                foreach (var node in allNodes)
                {
                    if(node.WasVisited)
                    {
                        continue;
                    }

                    if(node.ShortestDistance == int.MaxValue)
                    {
                        continue;
                    }

                    if(nextEvaluatedNode == null)
                    {
                        nextEvaluatedNode = node;
                        continue;
                    }

                    if(node.ShortestDistance < nextEvaluatedNode.ShortestDistance)
                    {
                        nextEvaluatedNode = node;
                    }
                }

                if(nextEvaluatedNode == null)
                {
                    break;
                }
                else
                {
                    currentNode = nextEvaluatedNode;
                }
            }

            if(targetNode.ShortestDistance == int.MaxValue)
            {
                return false;
            }

            path = targetNode.ShortestKnownPath;
            return true;
        }
    }
}
