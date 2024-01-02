using SMUBE.BattleState;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Effects;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.BaseBlock
{
    public class BaseBlock : ICommand
    {
        public int StaminaCost => 0;

        public int ManaCost => 0;

        public CommandId CommandId => CommandId.BaseBlock;

        public CommandArgsValidator CommandArgsValidator => new OneToSelfArgsValidator();

        private CommandArgs _argsCache;
        public CommandArgs ArgsCache { get => _argsCache; set => _argsCache = value; }

        public bool Execute(BattleStateModel battleStateModel, CommandArgs commandArgs)
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

            return true;
        }

        public CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();

            results.performer = commandArgs.ActiveUnit;
            results.targets.Add(commandArgs.ActiveUnit);
            results.effects.Add(new BlockEffect());

            return results;
        }
    }
}
