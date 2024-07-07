using SMUBE.BattleState;
using SMUBE.Commands;

namespace SMUBE.AI.StateMachine
{
    internal class StateMachineCanPerformTransition<T> : StateMachineTransition
            where T : ICommand, new()
    {

        private bool _negate;
        
        public StateMachineCanPerformTransition(StateMachineState targetState) : base(targetState)
        {
        }

        public StateMachineCanPerformTransition<T> AsNegated()
        {
            _negate = true;
            return this;
        }
        
        public override bool IsTriggered(BattleStateModel battleStateModel)
        {
            if (battleStateModel.GetNextActiveUnit(out var nextUnit))
            {
                var canPerform = nextUnit.UnitData.UnitStats.CanUseAbility(new T());

                return _negate
                    ? !canPerform
                    : canPerform;
            }
            return false;
        }
    }
}
