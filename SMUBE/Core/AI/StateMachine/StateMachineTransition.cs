using SMUBE.BattleState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.StateMachine
{
    public abstract class StateMachineTransition
    {
        public StateMachineState TargetState { get; }

        public StateMachineTransition(StateMachineState targetState)
        {
            TargetState = targetState;
        }

        public abstract bool IsTriggered(BattleStateModel battleStateModel);
    }
}
