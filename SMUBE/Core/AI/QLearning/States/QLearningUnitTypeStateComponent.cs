using System;
using SMUBE.BattleState;
using SMUBE.Units;
using SMUBE.Units.CharacterTypes;

namespace SMUBE.AI.QLearning.States
{
    public class QLearningUnitTypeStateComponent : BaseQLearningStateComponent
    {
        public QLearningUnitTypeStateComponent(int id) : base(id)
        {
        }

        protected override long GetNonUniqueStateValue(BattleStateModel stateModel, Unit actor)
        {
            switch (actor.UnitData.UnitStats.BaseCharacter.BaseCharacterType)
            {
                case BaseCharacterType.Scholar:
                    return 0;
                case BaseCharacterType.Squire:
                    return 1;
                case BaseCharacterType.Hunter:
                    return 2;
                case BaseCharacterType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        internal override string GetValueWithDescriptions(BattleStateModel stateModel, Unit actor)
        {
            var value = GetNonUniqueStateValue(stateModel, actor);
            switch (value)
            {
                case 0:
                    return $"UnitType - {value}: Scholar";
                case 1:
                    return $"UnitType - {value}: Squire";
                case 2:
                    return $"UnitType - {value}: Hunter";
                default:
                    throw new ArgumentException();
            }
        }
    }
}