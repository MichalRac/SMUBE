using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.GoalOrientedBehavior
{
    internal class LowerEnemyHealthGoal : Goal
    {
        protected override float Importance => 50;

        public override float GetDiscontentment(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            var opponentTeamId = activeUnitIdentifier.TeamId == 0 ? 1 : 0;

            var totalHp = battleStateModel.GetTeamUnits(opponentTeamId).Sum(u => u.UnitData.UnitStats.CurrentHealth);
            var totalMaxHp = battleStateModel.GetTeamUnits(opponentTeamId).Sum(u => u.UnitData.UnitStats.MaxHealth);

            return ((totalHp / totalMaxHp)) * Importance;
        }
    }
}
