﻿using SMUBE.BattleState;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.ShieldPosition
{
    public class ShieldPosition : BaseCommand
    {
        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_ShieldPosition;
        public override int ManaCost => SpecificCommandCostConfiguration.Mana_ShieldPosition;
        
        public override CommandId CommandId => CommandId.ShieldPosition;
        public override BaseCommandArgsValidator CommandArgsValidator => new OneToPositionValidator(true, false, true, false);

        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!base.TryExecute(battleStateModel, commandArgs))
            {
                return false;
            }

            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);
            var success = base.TryUseCommand(commandArgs, activeUnit);

            if (!success) return false;
            
            var target = commandArgs.TargetPositions[0];
            battleStateModel.BattleSceneState.Grid[target.x, target.y].ApplyDefensiveTimed(9);
            return true;
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();

            results.performer = commandArgs.ActiveUnit;
            results.positionTargets = commandArgs.TargetPositions;
            
            return results;
        }
    }
}