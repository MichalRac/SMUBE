using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Effects;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.LowerEnemyDefense
{
    public class LowerEnemyDefense : BaseCommand
    {
        public override int StaminaCost => 0;
        public override int ManaCost => 0;
        public override CommandId CommandId => CommandId.LowerEnemyDefense;
        public override BaseCommandArgsValidator CommandArgsValidator => new OneToOneArgsValidator(ArgsConstraint.Enemy);

        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!base.TryExecute(battleStateModel, commandArgs))
            {
                return false;
            }

            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);
            battleStateModel.TryGetUnit(commandArgs.TargetUnits.First().UnitIdentifier, out var targetUnit);

            return TryUseCommand(commandArgs, activeUnit, targetUnit);
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();

            results.performer = commandArgs.ActiveUnit;
            results.targets = commandArgs.TargetUnits;
            results.effects.Add(new DamageAppliedMultiplier(1.5f, UnitRoundStartTrigger.OnActiveUnitRoundStart));
            
            return results;
        }
    }
}