using SMUBE.BattleState;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Pathfinding
{
    public abstract class PathfindingAlgorithm
    {
        protected readonly static int STRAIGHT_COST_APPROXIMATION = 10;
        protected readonly static int DIAGONAL_COST_APPROXIMATION = 14;

        public abstract bool TryFindPathFromTo(GridBattleScene battleScene, BattleScenePosition start,
            BattleScenePosition target, out List<BattleScenePosition> path, out int visitedNodesCount);

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
