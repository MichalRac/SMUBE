using Commands;
using SMUBE.Commands.SpecificCommands.HealAll;
using System;

namespace SMUBE.AI.DecisionTree
{
    public class DecisionTreeActionHealAll : DecisionTreeAction
    {
        public override ICommand GetCommand()
        {
            return new HealAll();
        }
    }
}
