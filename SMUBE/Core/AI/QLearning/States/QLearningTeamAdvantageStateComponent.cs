using System;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Units;

namespace SMUBE.AI.QLearning.States
{
    public class QLearningTeamAdvantageStateComponent : BaseQLearningStateComponent
    {
        // Losing / Even / Winning
        private const float EVEN_TRESHOLD = 0.5f;
        
        public QLearningTeamAdvantageStateComponent(int id) 
            : base(id) { }

        protected override long GetNonUniqueStateValue(BattleStateModel stateModel, Unit actor)
        {
            var teamId = actor.UnitData.UnitIdentifier.TeamId;
            var opponentTeamId = teamId == 0 ? 1 : 0;

            var teamUnits = stateModel.GetTeamUnits(teamId);
            var opponentUnits = stateModel.GetTeamUnits(opponentTeamId);

            var teamTotalHealthPercentage = teamUnits.Sum(unit => (float)unit.UnitData.UnitStats.CurrentHealth / unit.UnitData.UnitStats.MaxHealth);
            var opponentTotalHealthPercentage = opponentUnits.Sum(unit => (float)unit.UnitData.UnitStats.CurrentHealth / unit.UnitData.UnitStats.MaxHealth);

            var teamScore = teamTotalHealthPercentage * teamUnits.Count;
            var opponentTeamScore = opponentTotalHealthPercentage * opponentUnits.Count;

            var scoreDifference = teamScore - opponentTeamScore;
            var evenThresholdAdjusted = EVEN_TRESHOLD * (teamUnits.Count + opponentUnits.Count);
            
            if (scoreDifference < -evenThresholdAdjusted)
                return 0; // losing
            if (scoreDifference > evenThresholdAdjusted)
                return 2; // winning
            else
                return 1; // even

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
                    return $"TeamAdvantage - {value}: Losing";
                case 1:
                    return $"TeamAdvantage - {value}: Even";
                case 2:
                    return $"TeamAdvantage - {value}: Winning";
                default:
                    throw new ArgumentException();
            }
        }
    }
}