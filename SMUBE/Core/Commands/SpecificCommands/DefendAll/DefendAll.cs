using SMUBE.BattleState;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Effects;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.DefendAll
{
    public class DefendAll : BaseCommand
    {
        public static int UseCounter = 0;
        private const int DEFEND_ALL_PERSISTANCE = 2;

        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_DefendAll;
        public override int ManaCost => SpecificCommandCostConfiguration.Mana_DefendAll;
        public override CommandId CommandId => CommandId.DefendAll;
        public override BaseCommandArgsValidator CommandArgsValidator => new OneToEveryArgsValidator(ArgsConstraint.Ally);

        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!CommandArgsValidator.Validate(commandArgs, battleStateModel))
            {
                return false;
            }

            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);
            if (activeUnit == null)
            {
                return false;
            }

            var success = commandArgs.ActiveUnit.UnitStats.CanUseAbility(this);
            if(!success)
            { 
                return false;
            }

            activeUnit.UnitData.UnitStats.TryUseAbility(this);

            foreach (var targetUnitData in commandArgs.TargetUnits)
            {
                battleStateModel.TryGetUnit(targetUnitData.UnitIdentifier, out var targetUnit);

                if (targetUnit == null)
                {
                    return false;
                }

                targetUnit.UnitData.UnitStats.AddEffects(GetCommandResults(commandArgs));
            }

            UseCounter++;
            return true;
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();
            results.performer = commandArgs.ActiveUnit;
            results.targets.AddRange(commandArgs.TargetUnits);
            results.effects.Add(new BlockEffect(DEFEND_ALL_PERSISTANCE));
            return results;
        }
    }
}
