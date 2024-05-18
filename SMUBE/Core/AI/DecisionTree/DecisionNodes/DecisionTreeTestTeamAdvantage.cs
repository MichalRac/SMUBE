using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree.DecisionNodes
{
    public class DecisionTreeTestTeamAdvantage : DecisionTreeTest
    {
        public DecisionTreeTestTeamAdvantage(DecisionTreeNode nodeIfTrue, DecisionTreeNode nodeIfFalse) 
            : base(nodeIfTrue, nodeIfFalse) { }

        protected override bool Test(BattleStateModel battleStateModel = null, CommandArgs commandArgs = null)
        {
            var teamId = battleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId;
            var opponentTeamId = teamId == 0 ? 1 : 0;

            var teamUnits = battleStateModel.GetTeamUnits(teamId);
            var opponentUnits = battleStateModel.GetTeamUnits(opponentTeamId);

            var teamTotalHealthPercentage = teamUnits.Sum(unit => (float)unit.UnitData.UnitStats.CurrentHealth / unit.UnitData.UnitStats.MaxHealth);
            var opponentTotalHealthPercentage = opponentUnits.Sum(unit => (float)unit.UnitData.UnitStats.CurrentHealth / unit.UnitData.UnitStats.MaxHealth);

            var teamScore = teamTotalHealthPercentage * teamUnits.Count;
            var opponentTeamScore = opponentTotalHealthPercentage * opponentUnits.Count;

            return teamScore > opponentTeamScore;
        }
    }
}