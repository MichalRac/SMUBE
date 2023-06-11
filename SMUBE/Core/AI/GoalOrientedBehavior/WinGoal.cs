using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.GoalOrientedBehavior
{
    public class WinGoal : Goal
    {
        protected override float Importance => 10;

        public override float GetDiscontentment(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            var ownTeamId = activeUnitIdentifier.TeamId;
            var opponentTeamId = activeUnitIdentifier.TeamId == 0 ? 1 : 0;

            var teamSurvivorCount = battleStateModel.GetTeamUnits(ownTeamId).Where(u => u.UnitData.UnitStats.CurrentHealth > 0).Count();
            var opponentSurvivorCount = battleStateModel.GetTeamUnits(opponentTeamId).Where(u => u.UnitData.UnitStats.CurrentHealth > 0).Count();

            if(teamSurvivorCount == 0)
            {
                return Importance * Importance;
            }

            return (opponentSurvivorCount / battleStateModel.GetTeamUnits(opponentTeamId).Count()) * (Importance - 1);
        }
    }
}
