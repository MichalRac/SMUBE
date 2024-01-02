using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SMUBE.Commands;

namespace SMUBE.AI.BehaviorTree
{
    public class BehaviorTreeSelector : BehaviorTreeTask
    {
        private List<BehaviorTreeTask> childTasks;

        public BehaviorTreeSelector(List<BehaviorTreeTask> childTasks)
        {
            this.childTasks = childTasks;
        }

        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier, out ICommand finalCommand)
        {
            finalCommand = null;
            foreach (BehaviorTreeTask task in childTasks)
            {
                if (task.Run(battleStateModel, activeUnitIdentifier, out finalCommand))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
