using System;
using SMUBE.BattleState;
using SMUBE.Units;

namespace SMUBE.AI.QLearning
{
    public abstract class BaseQLearningStateComponent
    {
        protected int Id;
        
        protected BaseQLearningStateComponent(int id)
        {
            Id = id;
        }
        
        protected abstract long GetNonUniqueStateValue(BattleStateModel stateModel, Unit actor);

        internal abstract string GetValueWithDescriptions(BattleStateModel stateModel, Unit actor);

        public long GetValue(BattleStateModel stateModel, Unit actor)
        {
            var order = (int)Math.Pow(10, Id);
            var nonUniqueStateValue = GetNonUniqueStateValue(stateModel, actor);

            if (nonUniqueStateValue > 9)
            {
                throw new ArgumentOutOfRangeException();
            }

            return nonUniqueStateValue * order;
        }
    }
}