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
        CommandArgsValidator CommandArgsValidator { get; }
        bool Execute(BattleStateModel battleStateModel, CommandArgs commandArgs);
        CommandResults GetCommandResults(CommandArgs commandArgs);
    }
}
