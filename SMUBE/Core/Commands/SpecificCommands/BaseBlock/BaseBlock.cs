using Commands;
using SMUBE.BattleState;
using SMUBE.Commands.SpecificCommands._Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands.SpecificCommands.BaseBlock
{
    public class BaseBlock : ICommand
    {
        public int StaminaCost => 0;

        public int ManaCost => 0;

        public CommandId CommandId => CommandId.BaseBlock;

        public CommandArgsValidator CommandArgsValidator => new OneToZeroArgsValidator();

        public bool Execute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            return CommandArgsValidator.Validate(commandArgs);
        }

        public CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();

            return new CommandResults();
        }
    }
}
