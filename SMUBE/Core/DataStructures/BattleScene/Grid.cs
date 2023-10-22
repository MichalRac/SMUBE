using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.BattleScene
{
    public class Grid
    {


/*        public List<BattleScenePosition> GetAdjacentPositions(BattleScenePosition targetPos)
        {
            var validPositions = new List<BattleScenePosition>();
            for (int x = targetPos.x - UnitData.DEFAULT_UNIT_GRID_SIZE; x < targetPos.x + UnitData.DEFAULT_UNIT_GRID_SIZE; x++)
            {
                for (int y = targetPos.y - UnitData.DEFAULT_UNIT_GRID_SIZE; y < targetPos.y + UnitData.DEFAULT_UNIT_GRID_SIZE; y++)
                {
                    if (x == targetPos.x && y == targetPos.y)
                    {
                        continue;
                    }

                    if (Math.Abs(x) > _width || Math.Abs(y) > _height)
                    {
                        continue;
                    }

                    validPositions.Add(new BattleScenePosition(x, y));
                }
            }
            return validPositions;
        }

        public bool IsOccupied(BattleScenePosition targetPos, BattleSceneState battleSceneState)
        {
            return battleSceneState.occupiedPositions.ContainsKey(targetPos);
        }

        public bool IsReachable(BattleScenePosition _, BattleScenePosition targetPos, BattleSceneState battleSceneState)
        {
            var adjacentPositions = GetAdjacentPositions(targetPos);

            foreach (var adjacentPosition in adjacentPositions)
            {
                if (IsOccupied(adjacentPosition, battleSceneState))
                {
                    continue;
                }

                // todo if
                // path exists

                // todo if
                // within turn speed distance

                return true;
            }
            return false;
        }

*/
/*        public bool IsValidPosition(BattleScenePosition targetPos)
        {
            if(targetPos == null)
            {
                return false;
            }

            if(Math.Abs(targetPos) > _width || Math.Abs(targetPos.y) > _height)
            {
                return false;
            }

            if (_gridContent[targetPos.x, targetPos.y])

            return true;
        }*/

/*        public static bool IsNextTo(BattleScenePosition basePos, BattleScenePosition targetPos)
        {
            if (Math.Abs(basePos.x - targetPos.x) > UnitData.DEFAULT_UNIT_GRID_SIZE)
                return true;

            if (Math.Abs(basePos.y - targetPos.y) > UnitData.DEFAULT_UNIT_GRID_SIZE)
                return true;

            return false;
        }

        public static double GetDistanceTo(BattleScenePosition basePos, BattleScenePosition targetPos)
        {
            return Math.Sqrt(((basePos.x - targetPos.x) * (basePos.x - targetPos.x)) + ((basePos.y - targetPos.y) * (basePos.y - targetPos.y)));
        }
*/
    }
}
