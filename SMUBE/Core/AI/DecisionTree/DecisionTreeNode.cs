using SMUBE.BattleState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree
{
    public interface DecisionTreeNode
    {
        DecisionTreeNode MakeDecision(BattleStateModel battleStateModel = null, CommandArgs commandArgs = null);
    }
}
