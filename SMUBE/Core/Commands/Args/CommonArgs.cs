using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.DataStructures.Units;

namespace SMUBE.Commands.Args
{
    public class CommonArgs : CommandArgs
    {
        public CommonArgs(UnitData activeUnit, List<UnitData> targetUnits, BattleStateModel battleStateModel, PositionDelta positionDelta = null) 
            : base(activeUnit, targetUnits, battleStateModel, positionDelta)
        {
        }
    }
}
