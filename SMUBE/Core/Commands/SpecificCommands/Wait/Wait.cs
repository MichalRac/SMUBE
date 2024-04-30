using SMUBE.BattleState;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.Wait
{
    public class Wait : BaseCommand
    {
        public static int UseCounter = 0;
        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_Wait;
        public override int ManaCost => SpecificCommandCostConfiguration.Mana_Wait;
        public override CommandId CommandId => CommandId.Wait;
        public override BaseCommandArgsValidator CommandArgsValidator => new OneToSelfArgsValidator();
        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            var success = base.TryExecute(battleStateModel, commandArgs);
            if (success)
            {
                UseCounter++;
            }
            return success;
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var commandResults = new CommandResults();

            commandResults.performer = commandArgs.ActiveUnit;
            
            return commandResults;
        }
    }
}