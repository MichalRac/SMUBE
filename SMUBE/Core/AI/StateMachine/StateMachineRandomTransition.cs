using SMUBE.BattleState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMUBE.Core;

namespace SMUBE.AI.StateMachine
{
    public class StateMachineRandomTransition : StateMachineTransition
    {
        private const float DEFAULT_CHANCE = 0.5f;
        private float _chance;

        public StateMachineRandomTransition(StateMachineState targetState, float chance = DEFAULT_CHANCE) : base(targetState)
        {
            _chance = chance;
        }

        public override bool IsTriggered(BattleStateModel _)
        {
            return RngProvider.NextDouble() <= _chance;
        }
    }
}
