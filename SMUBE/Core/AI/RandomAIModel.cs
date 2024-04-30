using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using System;
using SMUBE.Commands.Args;

namespace SMUBE.AI
{
    // always pick random option
    public class RandomAIModel : AIModel
    {
        private Random _random;

        public RandomAIModel(bool useSimpleBehavior) : base(useSimpleBehavior)
        {
            _random = new Random();
        }

        public override BaseCommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if(battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var viableCommands = unit.UnitCommandProvider.ViableCommands;
                
                if(viableCommands == null || viableCommands.Count == 0) 
                {
                    Console.WriteLine($"Unit {unit.UnitData.Name} has no viable actions!");
                    return null;
                }

                return viableCommands[_random.Next(viableCommands.Count)];
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
            return null;
        }

        public override CommandArgs GetCommandArgs(BaseCommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return command.GetSuggestedPseudoRandomArgs(battleStateModel);
        }
    }
}
