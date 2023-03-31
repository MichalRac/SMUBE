using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.BattleScene
{
    public class BattleScenePosition : SMUBEVector2<int>
    {
        public BattleScenePosition(int x, int y) : base(x, y)
        {
        }
    }

    public static class BattleScenePositionExtensions
    {
        public static bool IsApproximatelyCloseTo(this BattleScenePosition basePos, BattleScenePosition targetPos)
        {
            if (Math.Abs(basePos.x - targetPos.x) > UnitData.DEFAULT_UNIT_GRID_SIZE * 3)
                return true;

            if (Math.Abs(basePos.y - targetPos.y) > UnitData.DEFAULT_UNIT_GRID_SIZE * 3)
                return true;

            return false;
        }

        public static double GetDistanceTo(this BattleScenePosition basePos, BattleScenePosition targetPos)
        {
            return Math.Sqrt(((basePos.x - targetPos.x) * (basePos.x - targetPos.x)) + ((basePos.y - targetPos.y) * (basePos.y - targetPos.y)));
        }
    }
}
