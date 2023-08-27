using Commands;
using SMUBE.Commands.SpecificCommands.HeavyAttack;
using System;

namespace SMUBE.AI.DecisionTree
{
    public class DecisionTreeActionHeavyAttack : DecisionTreeAction
    {
        public override ICommand GetCommand()
        {
            return new HeavyAttack();
        }
    }

}
