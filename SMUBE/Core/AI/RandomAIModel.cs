using Commands;
using Commands.SpecificCommands._Common;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands._Common;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SMUBE.AI
{
    // always pick random option
    public class RandomAIModel : AIModel
    {
        public RandomAIModel(bool useSimpleBehavior) : base(useSimpleBehavior)
        {
        }

        public override ICommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if(battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var viableCommands = unit.ViableCommands;
                
                if(viableCommands == null || viableCommands.Count == 0) 
                {
                    Console.WriteLine($"Unit {unit.UnitData.Name} has no viable actions!");
                    return null;
                }

                return viableCommands[new Random().Next(viableCommands.Count)];
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
            return null;
        }

        public override CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return CommandArgsHelper.GetRandomCommandArgs(command, battleStateModel, activeUnitIdentifier);
        }
    }
}
