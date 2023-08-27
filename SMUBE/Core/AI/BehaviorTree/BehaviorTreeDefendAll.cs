using Commands;
using Commands.SpecificCommands.BaseAttack;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.DefendAll;
using SMUBE.Commands.SpecificCommands.HeavyAttack;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.BehaviorTree
{
    internal class BehaviorTreeDefendAll : BehaviorTreeTask
    {
        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier, out ICommand finalCommand)
        {
            finalCommand = null;
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var viableCommands = unit.ViableCommands;

                foreach (var command in viableCommands)
                {
                    if (command is DefendAll defendAll)
                    {
                        var commandArgs = CommandArgsHelper.GetRandomCommandArgs(defendAll, battleStateModel, activeUnitIdentifier);
                        var success = unit.UnitData.UnitStats.CanUseAbility(defendAll);
                        if (!success)
                        {
                            return false;
                        }

                        success = battleStateModel.ExecuteCommand(defendAll, commandArgs);

                        if (success)
                        {
                            finalCommand = command;
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
