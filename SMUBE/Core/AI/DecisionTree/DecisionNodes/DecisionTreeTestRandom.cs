using SMUBE.BattleState;
using SMUBE.Commands.Args;
using SMUBE.Core;

namespace SMUBE.AI.DecisionTree.DecisionNodes
{
    public class DecisionTreeTestRandom : DecisionTreeTest
    {
        private double _chance;

        public DecisionTreeTestRandom(double chance, DecisionTreeNode nodeIfTrue, DecisionTreeNode nodeIfFalse) : base(nodeIfTrue, nodeIfFalse)
        {
            _chance = chance;
        }

        protected override bool Test(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            return RngProvider.NextDouble() <= _chance;
        }
    }
}
