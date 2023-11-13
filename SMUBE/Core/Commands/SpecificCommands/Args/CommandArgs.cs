using SMUBE.BattleState;
using SMUBE.Commands.SpecificCommands.Args;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public abstract class CommandArgs
    {
        protected CommandArgs(UnitData activeUnit, List<UnitData> targetUnits, BattleStateModel battleStateModel, List<PositionDelta> positionDeltas = null)
        {
            ActiveUnit = activeUnit;
            TargetUnits = targetUnits;
            this.BattleStateModel = battleStateModel;
            this.PositionDeltas = positionDeltas;
        }

        public UnitData ActiveUnit { get; }
        public List<UnitData> TargetUnits { get; } = new List<UnitData>();
        public BattleStateModel BattleStateModel { get; }

        public List<PositionDelta> PositionDeltas { get; set; }
    }
}
