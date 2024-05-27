using System;
using SMUBE.BattleState;
using SMUBE.Commands.Effects;
using SMUBE.Units;

namespace SMUBE.AI.QLearning.States
{
    public class QLearningEffectLevelStateComponent : BaseQLearningStateComponent
    {
        // Weakened / Normal / Fortified
        
        public QLearningEffectLevelStateComponent(int id) 
            : base(id) { }

        protected override long GetNonUniqueStateValue(BattleStateModel stateModel, Unit actor)
        {
            var persistentEffects = actor.UnitData.UnitStats.PersistentEffects;

            float totalScore = 1;
            foreach (var effect in persistentEffects)
            {
                if (effect is DamageAppliedMultiplier dam)
                {
                    totalScore /= dam.Multiplier;
                }
                else if (effect is BlockEffect be)
                {
                    totalScore /= be.Multiplier;
                }
                else if(effect is TauntEffect)
                {
                    totalScore *= 0.5f;
                }
            }

            if (totalScore < 1f)
                return 0; // weakened
            if (Math.Abs(totalScore - 1f) < float.Epsilon)
                return 1; // normal
            if (totalScore > 1f)
                return 2; // fortified

            throw new ArithmeticException();
        }

        internal override string GetValueWithDescriptions(BattleStateModel stateModel, Unit actor)
        {
            var value = GetNonUniqueStateValue(stateModel, actor);
            return ValueToDescription(value);
        }

        internal override string ValueToDescription(long value)
        {
            switch (value)
            {
                case 0:
                    return $"Effect - {value}: weakened";
                case 1:
                    return $"Effect - {value}: normal";
                case 2:
                    return $"Effect - {value}: fortified";
                default:
                    throw new ArgumentException();
            }
        }
    }
}