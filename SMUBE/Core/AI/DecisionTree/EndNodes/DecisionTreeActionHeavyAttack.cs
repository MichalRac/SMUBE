using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.HeavyAttack;

namespace SMUBE.AI.DecisionTree.EndNodes
{
    public class DecisionTreeActionHeavyAttack : DecisionTreeAction
    {
        public override BaseCommand GetCommand()
        {
            return new HeavyAttack();
        }
    }

}
