﻿using SMUBE.AI.Common;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;

namespace SMUBE.AI.BehaviorTree
{
    public class BehaviorTreePerformHurtAction : BehaviorTreeTask
    {
        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier, out BaseCommand finalCommand)
        {
            finalCommand = SimpleWeightedCommandSelectionConfig.OnHurtActionSelection().GetCommand(battleStateModel);
            var args = finalCommand.GetSuggestedPseudoRandomArgs(battleStateModel);
            var success = battleStateModel.ExecuteCommand(finalCommand, args);
            if(success)
            {
                finalCommand.ArgsCache = args;
            }

            return success && finalCommand != null;
        }
    }
}