using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.DefendAll;

namespace SMUBE.AI.DecisionTree.EndNodes
{
    public class DecisionTreeActionDefendAll : DecisionTreeAction
    {
        public override BaseCommand GetCommand()
        {
            return new DefendAll();
        }
    }

}
