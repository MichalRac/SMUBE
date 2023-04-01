using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.BattleScene
{
    public class GridSceneType : BattleSceneType
    {
        public GridSceneType(int argSceneSize) : base(argSceneSize)
        {
        }

        public override List<BattleScenePosition> GetAdjacentPositions(BattleScenePosition targetPos)
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

                    if (Math.Abs(x) > SceneSize || Math.Abs(y) > SceneSize)
                    {
                        continue;
                    }

                    validPositions.Add(new BattleScenePosition(x, y));
                }
            }
            return validPositions;
        }

        public override bool IsOccupied(BattleScenePosition targetPos, BattleSceneState battleSceneState)
        {
            return battleSceneState.occupiedPositions.ContainsKey(targetPos);
        }

        public override bool IsReachable(BattleScenePosition _, BattleScenePosition targetPos, BattleSceneState battleSceneState)
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


        public override bool IsValidPosition(BattleScenePosition targetPos)
        {
            if(targetPos == null)
            {
                return false;
            }

            if (Math.Abs(targetPos.x) > SceneSize || Math.Abs(targetPos.y) > SceneSize)
            {
                return false;
            }

            return true;
        }
    }
}
