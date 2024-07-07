using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Core;

namespace SMUBE.AI.Common
{
    public class SimpleWeightedCommandSet
    {
        public BaseCommand Result;
        public int Weight;

        public SimpleWeightedCommandSet(BaseCommand result, int weight)
        {
            Result = result;
            Weight = weight;
        }
    }

    
    public class SimpleWeightedCommandActionSelection
    {
        private readonly IReadOnlyList<SimpleWeightedCommandSet> _options;

        public SimpleWeightedCommandActionSelection(IReadOnlyList<SimpleWeightedCommandSet> options)
        {
            _options = options;
        }
        
        public BaseCommand GetCommand(BattleStateModel battleStateModel)
        {
            var viableOptions = new List<SimpleWeightedCommandSet>();
            foreach (var currentOption in _options)
            {
                if (!battleStateModel.GetNextActiveUnit(out var nextUnit))
                    continue;
                
                if(nextUnit.UnitCommandProvider.ViableCommands.Any(command => command.GetType() == currentOption.Result.GetType()))
                    viableOptions.Add(currentOption);
            }

            if (viableOptions.Count == 0)
            {
                throw new ArgumentException();
            }

            var max = viableOptions.Sum(viableOption => viableOption.Weight);
            var test =  RngProvider.NextDouble() * max;
            var currentSum = 0;
   
            foreach (var currentOption in viableOptions)
            {
                currentSum += currentOption.Weight;

                if (currentSum >= test)
                    return currentOption.Result;
            }
            
            throw new IndexOutOfRangeException();
        }
    }
}