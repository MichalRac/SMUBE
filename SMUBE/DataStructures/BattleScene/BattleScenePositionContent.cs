using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.BattleScene
{
    public class BattleScenePositionContent
    {
        public bool IsOccupied { get; set; }
        public UnitIdentifier UnitIdentifier { get; }
    }
}
