using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree.EndNodes
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
