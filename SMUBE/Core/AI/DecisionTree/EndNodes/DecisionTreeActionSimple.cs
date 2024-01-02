using SMUBE.Commands.SpecificCommands.HeavyAttack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMUBE.Commands;

namespace SMUBE.AI.DecisionTree.EndNodes
{
    internal class DecisionTreeActionSimple<T> : DecisionTreeAction 
        where T : ICommand, new()    
    {
        public override ICommand GetCommand()
        {
            return new T();
        }
    }
}
