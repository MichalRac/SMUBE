using SMUBE.Commands.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public interface Command
    {
        CommandId CommandId { get; }
        CommandResults GetCommandResults(CommandArgs commandArgs);
    }
}
