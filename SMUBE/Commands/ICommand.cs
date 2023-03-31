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
        CommandId CommandId { get; }
        bool Execute(CommandArgs commandArgs);
        CommandResults GetCommandResults(CommandArgs commandArgs);
    }
}
