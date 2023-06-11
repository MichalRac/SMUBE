using Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.StateMachine
{
    public class StateMachineState
    {
        public ICommand Command { get; }
        public List<StateMachineTransition> Transitions { get; private set; }

        public StateMachineState(ICommand command)
        {
            Command = command;
        }

        public void InjectTransitions(List<StateMachineTransition> transitions)
        {
            Transitions = transitions;
        }
    }
}
