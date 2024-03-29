using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.Teleport
{
    public class Teleport : BaseCommand
    {
        public override int StaminaCost => 25;
        public override int ManaCost => 0;
        public override CommandId CommandId => CommandId.Teleport;
        public override BaseCommandArgsValidator CommandArgsValidator => new OneMoveToPositionValidator(true);
        
        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!base.TryExecute(battleStateModel, commandArgs))
            {
                return false;
            }

            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);
            
            var startPos = commandArgs.BattleStateModel.BattleSceneState
                .Grid[activeUnit.UnitData.BattleScenePosition.Coordinates.x, activeUnit.UnitData.BattleScenePosition.Coordinates.y];
            startPos.Clear();

            var target = commandArgs.PositionDelta.Target;
            var targetPos = commandArgs.BattleStateModel.BattleSceneState.Grid[target.x, target.y];
            activeUnit.UnitData.BattleScenePosition = targetPos;
            activeUnit.UnitData.BattleScenePosition.ApplyUnit(activeUnit.UnitData.UnitIdentifier);

            return TryUseCommand(commandArgs, activeUnit);
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();

            results.performer = commandArgs.ActiveUnit;
            results.PositionDeltas = new List<PositionDelta>{ commandArgs.PositionDelta };
            
            return results;
        }
    }
}