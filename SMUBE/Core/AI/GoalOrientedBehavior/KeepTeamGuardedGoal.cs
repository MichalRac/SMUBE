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
        protected override float Importance => 85;

        public override float GetDiscontentment(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            var TeamId = activeUnitIdentifier.TeamId == 0 ? 0 : 1;

            var teamUnits = battleStateModel.GetTeamUnits(TeamId);

            // no need to guard if you're the last unit
            if(teamUnits.Count <= 1)
            {
                return Importance;
            }

            var guardedTeamUnits = teamUnits.Count(u => u.UnitData.UnitStats.PersistentEffects.Where(e => e is BlockEffect).Count() > 0);


            return (1 - (guardedTeamUnits / teamUnits.Count)) * Importance;
        }
    }
}
