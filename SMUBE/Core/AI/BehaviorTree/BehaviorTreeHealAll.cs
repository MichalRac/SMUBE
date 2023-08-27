using Commands;
using Commands.SpecificCommands.BaseAttack;
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

namespace SMUBE.AI.BehaviorTree
{
    internal class BehaviorTreeHealAll : BehaviorTreeTask
    {
        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier, out ICommand finalCommand)
        {
            finalCommand = null;
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var viableCommands = unit.ViableCommands;

                foreach (var command in viableCommands)
                {
                    if (command is HealAll healAll)
                    {
                        var commandArgs = CommandArgsHelper.GetRandomCommandArgs(healAll, battleStateModel, activeUnitIdentifier);
                        var success = unit.UnitData.UnitStats.CanUseAbility(healAll);
                        if (!success)
                        {
                            return false;
                        }

                        success = success = battleStateModel.ExecuteCommand(healAll, commandArgs);

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
