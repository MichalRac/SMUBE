using Commands.SpecificCommands.BaseAttack;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.BaseBlock;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.BehaviorTree
{
    public class BehaviorTreeBasicBlock : BehaviorTreeTask
    {
        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var viableCommands = unit.ViableCommands;

                foreach (var command in viableCommands)
                {
                    if (command is BaseBlock baseBlock)
                    {
                        var commandArgs = CommandArgsHelper.GetDumbCommandArgs(baseBlock, battleStateModel, activeUnitIdentifier);
                        battleStateModel.ExecuteCommand(baseBlock, commandArgs);
                        return true;
                    }
                }
                return false;
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
            return false;
        }

    }
}
