using System.Collections.Generic;
using SMUBE.Commands;

namespace SMUBE.AI.StateMachine
{
    public class StateMachineState
    {
        public BaseCommand Command { get; }
        public List<StateMachineTransition> Transitions { get; private set; }

        public StateMachineState(BaseCommand command)
        {
            Command = command;
        }

        public void InjectTransitions(List<StateMachineTransition> transitions)
        {
            Transitions = transitions;
        }
    }
}
