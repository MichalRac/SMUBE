using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree.DecisionNodes
{
    public class DecisionTreeTestCanPerform<T> : DecisionTreeTest
        where T : ICommand, new()
    {
        public DecisionTreeTestCanPerform(DecisionTreeNode nodeIfTrue, DecisionTreeNode nodeIfFalse) : base(nodeIfTrue, nodeIfFalse)
        {
        }

        protected override bool Test(BattleStateModel battleStateModel, CommandArgs _)
        {
            if(battleStateModel.GetNextActiveUnit(out var nextUnit))
            {
                return nextUnit.UnitCommandProvider.ViableCommands.Any(command => command is T);
            }
            return false;
        }
    }
}
