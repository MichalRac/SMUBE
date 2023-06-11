using Commands;
using SMUBE.AI.GoalOrientedBehavior;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.StateMachine
{
    public class StateMachineAIModel : AIModel
    {
        // Todo Replace with FSM implementation
        public override ICommand GetNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var viableCommands = unit.ViableCommands;

                if (viableCommands == null || viableCommands.Count == 0)
                {
                    Console.WriteLine($"Unit {unit.UnitData.Name} has no viable actions!");
                    return null;
                }

                return viableCommands[0];
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
            return null;
        }

        public override CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return CommandArgsHelper.GetDumbCommandArgs(command, battleStateModel, activeUnitIdentifier);
        }
    }
}
