using Commands.SpecificCommands.BaseAttack;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace SMUBE.AI.BehaviorTree
{
    public class BehaviorTreeBasicAttack : BehaviorTreeTask
    {
        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var viableCommands = unit.ViableCommands;

                foreach(var command in viableCommands)
                {
                    if(command is BaseAttack baseAttack)
                    {
                        var commandArgs = CommandArgsHelper.GetRandomCommandArgs(baseAttack, battleStateModel, activeUnitIdentifier);
                        battleStateModel.ExecuteCommand(baseAttack, commandArgs);
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
