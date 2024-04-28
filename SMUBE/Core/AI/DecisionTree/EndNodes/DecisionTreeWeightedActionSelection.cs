using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.AI.DecisionTree.EndNodes.SecondaryChecks;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree.EndNodes
{
    public class DecisionTreeWeightedSet
    {
        public BaseCommand Result;
        public int Weight;
        public DecisionTreeSecondaryCheck SecondaryCheck;

        public DecisionTreeWeightedSet(BaseCommand result, int weight, DecisionTreeSecondaryCheck secondaryCheck = null)
        {
            Result = result;
            Weight = weight;
            SecondaryCheck = secondaryCheck;
        }
    }
    
    public class DecisionTreeWeightedActionSelection : DecisionTreeNode
    {
        private readonly Random _random = new Random();
        private readonly IReadOnlyList<DecisionTreeWeightedSet> _options;

        public DecisionTreeWeightedActionSelection(IReadOnlyList<DecisionTreeWeightedSet> options)
        {
            _options = options;
        }
        
        public DecisionTreeNode MakeDecision(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            var viableOptions = new List<DecisionTreeWeightedSet>();
            foreach (var currentOption in _options)
            {
                if (!battleStateModel.GetNextActiveUnit(out var nextUnit))
                    continue;

                if (currentOption.SecondaryCheck != null && !currentOption.SecondaryCheck.Invoke(battleStateModel, commandArgs))
                    continue;
                
                if(nextUnit.UnitCommandProvider.ViableCommands.Any(command => command.GetType() == currentOption.Result.GetType()))
                    viableOptions.Add(currentOption);
            }

            if (viableOptions.Count == 0)
            {
                throw new ArgumentException();
            }

            var max = viableOptions.Sum(viableOption => viableOption.Weight);
            var test =  _random.NextDouble() * max;
            var currentSum = 0;
   
            foreach (var currentOption in viableOptions)
            {
                currentSum += currentOption.Weight;

                if (currentSum >= test)
                    return new DecisionTreeActionSimple(currentOption.Result);
            }
            
            throw new IndexOutOfRangeException();
        }
    }
}