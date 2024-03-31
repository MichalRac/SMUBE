using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMUBE.Commands;

namespace SMUBE.AI.BehaviorTree
{
    public abstract class BehaviorTreeTask
    {
        public abstract bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier, out BaseCommand finalCommand);
    }
}
