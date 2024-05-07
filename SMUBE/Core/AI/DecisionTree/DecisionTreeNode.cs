using SMUBE.BattleState;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree
{
    public interface DecisionTreeNode
    {
        DecisionTreeNode MakeDecision(BattleStateModel battleStateModel = null, CommandArgs commandArgs = null);
    }
}
