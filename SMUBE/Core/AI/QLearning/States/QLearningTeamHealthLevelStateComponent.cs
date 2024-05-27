using System;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Units;

namespace SMUBE.AI.QLearning.States
{
    public class QLearningTeamHealthLevelStateComponent : BaseQLearningStateComponent
    {
        // Low / Mid / High
        
        public QLearningTeamHealthLevelStateComponent(int id) 
            : base(id) { }

        protected override long GetNonUniqueStateValue(BattleStateModel stateModel, Unit actor)
        {
            var teamId = actor.UnitData.UnitIdentifier.TeamId;
            var teamUnits = stateModel.GetTeamUnits(teamId);
            var teamTotalHealthPercentage = teamUnits.Sum(unit => (float)unit.UnitData.UnitStats.CurrentHealth / unit.UnitData.UnitStats.MaxHealth);

            if (teamTotalHealthPercentage <= 1f)
                return 0; // low
            if (teamTotalHealthPercentage <= 2f)
                return 1; // mid
            else
                return 2; // high
        }
        
        internal override string GetValueWithDescriptions(BattleStateModel stateModel, Unit actor)
        {
            var value = GetNonUniqueStateValue(stateModel, actor);
            return ValueToDescription(value);
        }

        internal override string ValueToDescription(long value)
        {
            switch (value)
            {
                case 0:
                    return $"TeamHealth - {value}: Low";
                case 1:
                    return $"TeamHealth - {value}: Mid";
                case 2:
                    return $"TeamHealth - {value}: High";
                default:
                    throw new ArgumentException();
            }
        }
    }
}