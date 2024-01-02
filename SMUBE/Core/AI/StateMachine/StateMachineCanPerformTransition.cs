using SMUBE.BattleState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMUBE.Commands;

namespace SMUBE.AI.StateMachine
{
    internal class StateMachineCanPerformTransition<T> : StateMachineTransition
            where T : ICommand, new()
    {

        public StateMachineCanPerformTransition(StateMachineState targetState) : base(targetState)
        {
        }

        public override bool IsTriggered(BattleStateModel battleStateModel)
        {
            if (battleStateModel.GetNextActiveUnit(out var nextUnit))
            {
                return nextUnit.UnitData.UnitStats.CanUseAbility(new T());
            }
            return false;
        }
    }
}
