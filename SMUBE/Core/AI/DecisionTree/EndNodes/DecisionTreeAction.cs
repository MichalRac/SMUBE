using Commands;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.DecisionTree
{
    public abstract class DecisionTreeAction : DecisionTreeNode
    {
        public abstract ICommand GetCommand();

        public DecisionTreeNode MakeDecision(BattleStateModel battleStateModel = null, CommandArgs commandArgs = null)
        {
            return this;
        }
    }
}
