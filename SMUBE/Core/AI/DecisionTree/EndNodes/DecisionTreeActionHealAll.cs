using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.HealAll;

namespace SMUBE.AI.DecisionTree.EndNodes
{
    public class DecisionTreeActionHealAll : DecisionTreeAction
    {
        public override ICommand GetCommand()
        {
            return new HealAll();
        }
    }
}
