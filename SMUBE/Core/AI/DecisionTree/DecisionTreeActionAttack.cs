using Commands;
using Commands.SpecificCommands.BaseAttack;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.DecisionTree
{
    public class DecisionTreeActionAttack : DecisionTreeAction
    {
        public override ICommand GetCommand()
        {
            return new BaseAttack();
        }
    }
}
