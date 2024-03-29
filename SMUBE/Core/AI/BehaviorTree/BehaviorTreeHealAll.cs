using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.HealAll;
using SMUBE.Commands.SpecificCommands.HeavyAttack;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMUBE.Commands.Args;

namespace SMUBE.AI.BehaviorTree
{
    internal class BehaviorTreeHealAll : BehaviorTreeTask, IBehaviorTreeCommand
    {
        private CommandArgs _commandArgsCache;
        public CommandArgs CommandArgsCache => _commandArgsCache;
        public CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            _commandArgsCache = CommandArgsHelper.GetRandomCommandArgs(command, battleStateModel, activeUnitIdentifier);
            return _commandArgsCache;
        }

        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier, out ICommand finalCommand)
        {
            finalCommand = null;
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var viableCommands = unit.UnitCommandProvider.ViableCommands;

                foreach (var command in viableCommands)
                {
                    if (command is HealAll healAll)
                    {
                        var commandArgs = GetCommandArgs(healAll, battleStateModel, activeUnitIdentifier);
                        var success = unit.UnitData.UnitStats.CanUseAbility(healAll);
                        if (!success)
                        {
                            return false;
                        }

                        success = success = battleStateModel.ExecuteCommand(healAll, commandArgs);

                        if (success)
                        {
                            finalCommand = command;
                            finalCommand.ArgsCache = CommandArgsCache;
                        }

                        return success;
                    }
                }
                return false;
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
            return false;
        }
    }
}
