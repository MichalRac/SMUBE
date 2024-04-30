using SMUBE.BattleState;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.RaiseObstacle
{
    public class RaiseObstacle : BaseCommand
    {
        public static int UseCounter = 0;
        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_RaiseObstacle;
        public override int ManaCost => SpecificCommandCostConfiguration.Mana_RaiseObstacle;
        
        public override CommandId CommandId => CommandId.RaiseObstacle;
        public override BaseCommandArgsValidator CommandArgsValidator => new OneToPositionValidator(false, false, true, false);

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
            battleStateModel.BattleSceneState.Grid[target.x, target.y].ApplyObstacleTimed(9);
            UseCounter++;
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