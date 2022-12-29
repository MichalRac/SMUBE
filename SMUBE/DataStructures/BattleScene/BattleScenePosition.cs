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
}
