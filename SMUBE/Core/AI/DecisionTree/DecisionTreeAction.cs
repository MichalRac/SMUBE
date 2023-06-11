using Commands;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.DecisionTree
{
    public abstract class DecisionTreeAction : DecisionTreeNode
    {
        public DecisionTreeNode MakeDecision()
        {
            return this;
        }

        public abstract ICommand GetCommand();
    }
}
