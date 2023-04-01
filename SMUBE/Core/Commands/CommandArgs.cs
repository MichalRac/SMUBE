using SMUBE.BattleState;
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
        protected CommandArgs(UnitData activeUnit, List<UnitData> targetUnits, BattleStateModel battleStateModel)
        {
            ActiveUnit = activeUnit;
            TargetUnits = targetUnits;
            this.battleStateModel = battleStateModel;
        }

        public UnitData ActiveUnit { get; }
        public List<UnitData> TargetUnits { get; } = new List<UnitData>();
        public BattleStateModel battleStateModel { get; }
    }
}
