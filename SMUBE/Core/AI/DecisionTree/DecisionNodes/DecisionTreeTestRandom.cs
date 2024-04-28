using System;
using SMUBE.BattleState;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree.DecisionNodes
{
    public class DecisionTreeTestRandom : DecisionTreeTest
    {
        private Random _random = new Random();
        private double _chance;

        public DecisionTreeTestRandom(double chance, DecisionTreeNode nodeIfTrue, DecisionTreeNode nodeIfFalse) : base(nodeIfTrue, nodeIfFalse)
        {
            _chance = chance;
        }

        protected override bool Test(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            return _random.NextDouble() <= _chance;
        }
    }
}
