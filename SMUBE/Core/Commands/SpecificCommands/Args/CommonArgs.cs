using SMUBE.BattleState;
using SMUBE.Commands.SpecificCommands.Args;
using SMUBE.DataStructures.Units;
using System.Collections.Generic;

namespace Commands.SpecificCommands._Common
{
    public class CommonArgs : CommandArgs
    {
        public CommonArgs(UnitData activeUnit, List<UnitData> targetUnits, BattleStateModel battleStateModel, PositionDelta positionDelta = null) 
            : base(activeUnit, targetUnits, battleStateModel, positionDelta)
        {
        }
    }
}
