using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.BaseAttack;

namespace SMUBE.AI.DecisionTree.EndNodes
{
    public class DecisionTreeActionAttack : DecisionTreeAction
    {
        public override ICommand GetCommand()
        {
            return new BaseAttack();
        }
    }
}
