using SMUBE.BattleState;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Effects;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.BaseBlock
{
    public class BaseBlock : BaseCommand
    {
        public static int UseCounter = 0;
        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_BaseBlock;

        public override int ManaCost => SpecificCommandCostConfiguration.Mana_BaseBlock;

        public override CommandId CommandId => CommandId.BaseBlock;

        public override BaseCommandArgsValidator CommandArgsValidator => new OneToSelfArgsValidator();

        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!CommandArgsValidator.Validate(commandArgs, battleStateModel))
            {
                return false;
            }

            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);

            if(activeUnit == null)
            {
                return false;
            }

            activeUnit.UnitData.UnitStats.TryUseAbility(this);
            activeUnit.UnitData.UnitStats.AddEffects(GetCommandResults(commandArgs));

            UseCounter++;
            return true;
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();

            results.performer = commandArgs.ActiveUnit;
            results.targets.Add(commandArgs.ActiveUnit);
            results.effects.Add(new BlockEffect());

            return results;
        }
    }
}
