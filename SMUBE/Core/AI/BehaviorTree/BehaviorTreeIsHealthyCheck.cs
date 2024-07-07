using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;

namespace SMUBE.AI.BehaviorTree
{
    public class BehaviorTreeIsHealthyCheck : BehaviorTreeTask
    {
        public float _healthThreshold;

        public BehaviorTreeIsHealthyCheck(float healthThreshold)
        {
            _healthThreshold = healthThreshold;
        }

        public override bool Run(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier, out BaseCommand finalCommand)
        {
            finalCommand = null;
            
            var stats = battleStateModel.ActiveUnit.UnitData.UnitStats;
            var healthPercentage = stats.CurrentHealth / stats.MaxHealth;

            return healthPercentage >= _healthThreshold;
        }
    }
}