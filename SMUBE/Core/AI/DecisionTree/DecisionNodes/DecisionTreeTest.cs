using Commands;
using SMUBE.BattleState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.DecisionTree
{
    public abstract class DecisionTreeTest : DecisionTreeNode
    {
        private DecisionTreeNode _nodeIfTrue;
        private DecisionTreeNode _nodeIfFalse;

        protected DecisionTreeTest(DecisionTreeNode nodeIfTrue, DecisionTreeNode nodeIfFalse)
        {
            _nodeIfTrue = nodeIfTrue;
            _nodeIfFalse = nodeIfFalse;
        }

        protected abstract bool Test(BattleStateModel battleStateModel = null, CommandArgs commandArgs = null);

        public DecisionTreeNode MakeDecision(BattleStateModel battleStateModel = null, CommandArgs commandArgs = null)
        {
            return Test(battleStateModel, commandArgs)
                ? _nodeIfTrue.MakeDecision(battleStateModel, commandArgs) 
                : _nodeIfFalse.MakeDecision(battleStateModel, commandArgs);
        }
    }
}
