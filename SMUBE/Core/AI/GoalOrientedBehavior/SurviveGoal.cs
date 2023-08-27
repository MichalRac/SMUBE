using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.GoalOrientedBehavior
{
    public class SurviveGoal : Goal
    {
        protected override float Importance => 30;

        public override float GetDiscontentment(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                return (1 - (unit.UnitData.UnitStats.CurrentHealth / unit.UnitData.UnitStats.MaxHealth)) * Importance;
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
            return float.MaxValue;
        }
    }
}
