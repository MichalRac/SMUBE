using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.BehaviorTree
{
    public class BehaviorTreeSelector : BehaviorTreeTask
    {
        private List<BehaviorTreeTask> childTasks;

        public BehaviorTreeSelector(List<BehaviorTreeTask> childTasks)
        {
            this.childTasks = childTasks;
        }

        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            foreach (BehaviorTreeTask task in childTasks)
            {
                if (task.Run(battleStateModel, activeUnitIdentifier))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
