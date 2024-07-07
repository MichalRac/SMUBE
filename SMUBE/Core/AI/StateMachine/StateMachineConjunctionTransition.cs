using System.Linq;
using SMUBE.BattleState;

namespace SMUBE.AI.StateMachine
{
    public class StateMachineConjunctionTransition : StateMachineTransition
    {
        private readonly StateMachineTransition[] _transitions;

        public StateMachineConjunctionTransition(StateMachineState targetState, params StateMachineTransition[] transitions) : base(targetState)
        {
            _transitions = transitions;
        }

        public override bool IsTriggered(BattleStateModel battleStateModel)
        {
            return _transitions.All(transition => transition.IsTriggered(battleStateModel));
        }
    }
}