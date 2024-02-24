using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args
{
    public abstract class CommandArgs
    {
        protected CommandArgs(UnitData activeUnit, List<UnitData> targetUnits, BattleStateModel battleStateModel, PositionDelta positionDelta = null)
        {
            ActiveUnit = activeUnit;
            TargetUnits = targetUnits;
            this.BattleStateModel = battleStateModel;
            this.PositionDelta = positionDelta;
        }

        public UnitData ActiveUnit { get; }
        public List<UnitData> TargetUnits { get; } = new List<UnitData>();
        public List<SMUBEVector2<int>> TargetPositions { get; }
        public BattleStateModel BattleStateModel { get; }

        public PositionDelta PositionDelta { get; set; }
    }
}
