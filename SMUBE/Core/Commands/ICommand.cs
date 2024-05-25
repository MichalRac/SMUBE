using SMUBE.BattleState;
using SMUBE.Commands.Args;
using SMUBE.Commands.Results;

namespace SMUBE.Commands
{
    public interface ICommand
    {
        int StaminaCost { get; }
        int ManaCost { get; }
        CommandArgs ArgsCache { get; set; }

        CommandId CommandId { get; }
        BaseCommandArgsValidator CommandArgsValidator { get; }
        ArgsPreferences ArgsPreferences { get; } 
        bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs);
        CommandResults GetCommandResults(CommandArgs commandArgs);
    }
}
