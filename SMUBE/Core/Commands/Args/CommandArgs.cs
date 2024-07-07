using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args
{
    public abstract class CommandArgs
    {
        internal string DebugSource { get; set; }
        public UnitData ActiveUnit { get; protected set; }
        public List<UnitData> TargetUnits { get; protected set;}
        public List<SMUBEVector2<int>> TargetPositions { get; protected set;}
        public BattleStateModel BattleStateModel { get; protected set;}

        public PositionDelta PositionDelta { get; set; }
        
        protected CommandArgs()
        {}
        
        protected CommandArgs(UnitData activeUnit, List<UnitData> targetUnits, BattleStateModel battleStateModel, PositionDelta positionDelta = null,
            List<SMUBEVector2<int>> targetPositions = null)
        {
            ActiveUnit = activeUnit;
            TargetUnits = targetUnits;
            this.BattleStateModel = battleStateModel;
            this.PositionDelta = positionDelta;
            TargetPositions = targetPositions;
        }
        
        public abstract CommandArgs DeepCopyWithNewBattleStateModel(BattleStateModel newModel);
    }
}
