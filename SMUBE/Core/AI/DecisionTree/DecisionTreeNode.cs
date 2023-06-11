using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.DecisionTree
{
    public interface DecisionTreeNode
    {
        DecisionTreeNode MakeDecision();
    }
}
