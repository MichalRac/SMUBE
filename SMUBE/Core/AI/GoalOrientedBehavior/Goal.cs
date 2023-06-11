using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.GoalOrientedBehavior
{
    public abstract class Goal
    {
        protected abstract float Importance { get; }

        public abstract float GetDiscontentment(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier);
    }
}
