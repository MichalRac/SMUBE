using SMUBE.BattleState;
using SMUBE.Commands.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
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
