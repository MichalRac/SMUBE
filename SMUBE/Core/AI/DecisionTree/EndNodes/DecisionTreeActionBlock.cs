using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.BaseBlock;

namespace SMUBE.AI.DecisionTree.EndNodes
{
    public class DecisionTreeActionBlock : DecisionTreeAction
    {
        public override ICommand GetCommand()
        {
            return new BaseBlock();
        }
    }
}
