using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.BaseAttack;

namespace SMUBE.AI.DecisionTree.EndNodes
{
    public class DecisionTreeActionAttack : DecisionTreeAction
    {
        public override BaseCommand GetCommand()
        {
            return new BaseAttack();
        }
    }
}
