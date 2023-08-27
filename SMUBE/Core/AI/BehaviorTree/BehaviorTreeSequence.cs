using Commands;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.BehaviorTree
{
    public class BehaviorTreeSequence : BehaviorTreeTask
    {

        private List<BehaviorTreeTask> childTasks;

        public BehaviorTreeSequence(List<BehaviorTreeTask> childTasks)
        {
            this.childTasks = childTasks;
        }

        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier, out ICommand finalCommand)
        {
            finalCommand = null;
            foreach (BehaviorTreeTask task in childTasks)
            {
                if (!task.Run(battleStateModel, activeUnitIdentifier, out finalCommand))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
