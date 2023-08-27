using Commands;
using SMUBE.Commands.SpecificCommands.DefendAll;
using System;

namespace SMUBE.AI.DecisionTree
{
    public class DecisionTreeActionDefendAll : DecisionTreeAction
    {
        public override ICommand GetCommand()
        {
            return new DefendAll();
        }
    }

}
