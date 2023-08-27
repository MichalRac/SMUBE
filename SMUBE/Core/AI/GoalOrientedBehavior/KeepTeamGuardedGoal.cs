using SMUBE.BattleState;
using SMUBE.Commands.Effects;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.GoalOrientedBehavior
{
    internal class KeepTeamGuardedGoal : Goal
    {
        protected override float Importance => 8;

        public override float GetDiscontentment(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            var TeamId = activeUnitIdentifier.TeamId == 0 ? 0 : 1;

            var teamUnit = battleStateModel.GetTeamUnits(TeamId);
            var guardedTeamUnits = teamUnit.Count(u => u.UnitData.UnitStats.PersistentEffects.Where(e => e is BlockEffect).Count() > 0);


            return (1 - (guardedTeamUnits / teamUnit.Count)) * Importance;
        }
    }
}
