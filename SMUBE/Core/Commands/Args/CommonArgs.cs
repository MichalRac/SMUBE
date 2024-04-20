using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args
{
    public class CommonArgs : CommandArgs
    {
        public CommonArgs(UnitData activeUnit, UnitData targetUnit, BattleStateModel battleStateModel) 
            : base(activeUnit, new List<UnitData>{targetUnit}, battleStateModel)
        {
        }

        
        public CommonArgs(UnitData activeUnit, List<UnitData> targetUnits, BattleStateModel battleStateModel, PositionDelta positionDelta = null, List<SMUBEVector2<int>> targetPositions = null) 
            : base(activeUnit, targetUnits, battleStateModel, positionDelta, targetPositions)
        {
        }
    }
}
