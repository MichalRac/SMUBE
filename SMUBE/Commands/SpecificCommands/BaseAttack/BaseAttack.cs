using SMUBE.Commands.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.SpecificCommands.BaseAttack
{
    public class BaseAttack : ICommand
    {
        public CommandId CommandId => CommandId.BaseAttack;

        public bool Execute(CommandArgs commandArgs)
        {
            throw new NotImplementedException();
        }

        public CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            if(!commandArgs.CommandArgsValidator.Validate(commandArgs))
            {
                return null;
            }

            throw new NotImplementedException();
        }
    }
}
