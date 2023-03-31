using Commands;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;

namespace AI
{
    public class RandomAIModel : AIModel
    {
        public override ICommand GetNextCommand(BattleStateModel battleStateModel, UnitIdentifier unitIdentifier)
        {
            if(battleStateModel.TryGetUnit(unitIdentifier, out var unit))
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
    }
}
