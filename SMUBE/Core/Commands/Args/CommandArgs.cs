using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args
{
    public abstract class CommandArgs
    {
        public UnitData ActiveUnit { get; }
        public List<UnitData> TargetUnits { get; }
        public List<SMUBEVector2<int>> TargetPositions { get; }
        public BattleStateModel BattleStateModel { get; }

        public PositionDelta PositionDelta { get; set; }
        
        protected CommandArgs(UnitData activeUnit, List<UnitData> targetUnits, BattleStateModel battleStateModel, PositionDelta positionDelta = null,
            List<SMUBEVector2<int>> targetPositions = null)
        {
            ActiveUnit = activeUnit;
            TargetUnits = targetUnits;
            this.BattleStateModel = battleStateModel;
            this.PositionDelta = positionDelta;
            TargetPositions = targetPositions;
        }

    }
}
