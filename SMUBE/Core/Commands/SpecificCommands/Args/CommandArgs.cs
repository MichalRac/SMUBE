using SMUBE.BattleState;
using SMUBE.Commands.SpecificCommands.Args;
using SMUBE.DataStructures.Units;
using System.Collections.Generic;

namespace Commands
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
        public BattleStateModel BattleStateModel { get; }

        public PositionDelta PositionDelta { get; set; }
    }
}
