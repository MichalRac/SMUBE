using System;
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
            : base(activeUnit, targetUnit == null ? null : new List<UnitData>{targetUnit}, battleStateModel)
        {
        }

        
        public CommonArgs(UnitData activeUnit, List<UnitData> targetUnits, BattleStateModel battleStateModel, PositionDelta positionDelta = null, List<SMUBEVector2<int>> targetPositions = null) 
            : base(activeUnit, targetUnits, battleStateModel, positionDelta, targetPositions)
        {
        }

        private CommonArgs(CommandArgs source, BattleStateModel newModel)
        {
            ActiveUnit = source.ActiveUnit.DeepCopy();
            this.BattleStateModel = newModel;
            this.PositionDelta = source.PositionDelta;
            TargetPositions = source.TargetPositions;
            
            var targetUnits = new List<UnitData>();
            if(source.TargetUnits?.Count > 0)
            {
                foreach (var deepCopyTargetUnit in source.TargetUnits)
                {
                    if (newModel.TryGetUnit(deepCopyTargetUnit.UnitIdentifier, out var targetUnit))
                    {
                        targetUnits.Add(targetUnit.UnitData);
                        continue;
                    }
                    throw new Exception($"Error! Trying to copy unit that is not present in another battle state model!");
                }
            }
            TargetUnits = targetUnits;
        }

        public override CommandArgs DeepCopyWithNewBattleStateModel(BattleStateModel newModel)
        {
            return new CommonArgs(this, newModel);
        }
    }
}
