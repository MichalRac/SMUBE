using Commands;
using SMUBE.BattleState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.DecisionTree
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
            return new Random().NextDouble() <= _chance;
        }
    }
}
