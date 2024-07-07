using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;

namespace SMUBE.AI.BehaviorTree
{
    public class BehaviorTreeAnyEnemyInReachCheck : BehaviorTreeTask
    {
        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier, out BaseCommand finalCommand)
        {
            finalCommand = null;
            return battleStateModel.ActiveUnit.UnitCommandProvider.ViableCommands.Any(bc => bc.CommandId == CommandId.BaseAttack);
        }
    }
}