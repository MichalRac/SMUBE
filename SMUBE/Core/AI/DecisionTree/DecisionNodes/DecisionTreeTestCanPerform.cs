using Commands;
using SMUBE.BattleState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.DecisionTree.DecisionNodes
{
    public class DecisionTreeTestCanPerform<T> : DecisionTreeTest
        where T : ICommand, new()
    {
        public DecisionTreeTestCanPerform(DecisionTreeNode nodeIfTrue, DecisionTreeNode nodeIfFalse) : base(nodeIfTrue, nodeIfFalse)
        {
        }

        protected override bool Test(BattleStateModel battleStateModel, CommandArgs _)
        {
            if(battleStateModel.GetNextActiveUnit(out var nextUnit))
            {
                return nextUnit.UnitData.UnitStats.CanUseAbility(new T());
            }
            return false;
        }
    }
}
