using Commands;
using SMUBE.BattleState;
using SMUBE.Commands.SpecificCommands.BaseBlock;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.DecisionTree
{
    public class DecisionTreeActionBlock : DecisionTreeAction
    {
        public override ICommand GetCommand()
        {
            return new BaseBlock();
        }
    }
}
