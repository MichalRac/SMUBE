using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.Commands;

namespace SMUBE.AI.StateMachine
{
    public class StateMachineState
    {
        private BaseCommand Command { get; }
        public List<StateMachineTransition> Transitions { get; private set; }

        protected StateMachineState()
        {
        }
        
        public StateMachineState(BaseCommand command)
        {
            Command = command;
        }

        public void InjectTransitions(List<StateMachineTransition> transitions)
        {
            Transitions = transitions;
        }

        public virtual BaseCommand GetCommand(BattleStateModel battleStateModel)
        {
            return Command;
        }
    }
}
