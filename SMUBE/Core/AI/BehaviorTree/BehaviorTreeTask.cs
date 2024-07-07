using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using SMUBE.Commands;

namespace SMUBE.AI.BehaviorTree
{
    public abstract class BehaviorTreeTask
    {
        public abstract bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier, out BaseCommand finalCommand);
    }
}
