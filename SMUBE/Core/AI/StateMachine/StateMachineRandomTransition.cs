using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.StateMachine
{
    public class StateMachineRandomTransition : StateMachineTransition
    {
        public StateMachineRandomTransition(StateMachineState targetState) : base(targetState)
        {
        }

        public override bool IsTriggered()
        {
            var rng = new Random();
            return rng.NextDouble() >= 0.5f;
        }
    }
}
