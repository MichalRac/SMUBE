using System;
using SMUBE.BattleState;
using SMUBE.Units;

namespace SMUBE.AI.QLearning.States
{
    public class QLearningHealthLevelStateComponent : BaseQLearningStateComponent
    {
        // Low / Mid / High
        
        public QLearningHealthLevelStateComponent(int id) 
            : base(id) { }

        protected override long GetNonUniqueStateValue(BattleStateModel stateModel, Unit actor)
        {
            var unitStats = actor.UnitData.UnitStats;
            var healthPercentage = (float)unitStats.CurrentHealth / unitStats.MaxHealth;

            if (healthPercentage < 1 / 3f)
                return 0; // low
            if (healthPercentage < 2 / 3f)
                return 1; // mid
            else
                return 2; // high
        }
        
        internal override string GetValueWithDescriptions(BattleStateModel stateModel, Unit actor)
        {
            var value = GetNonUniqueStateValue(stateModel, actor);
            switch (value)
            {
                case 0:
                    return $"PersonalHealth - {value}: low";
                case 1:
                    return $"PersonalHealth - {value}: mid";
                case 2:
                    return $"PersonalHealth - {value}: high";
                default:
                    throw new ArgumentException();
            }
        }
    }
}