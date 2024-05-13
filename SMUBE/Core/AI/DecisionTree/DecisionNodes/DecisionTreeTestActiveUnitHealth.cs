using SMUBE.BattleState;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree.DecisionNodes
{
    public class DecisionTreeTestActiveUnitHealth : DecisionTreeTest
    {
        private readonly float _minHealthPercentage;

        public DecisionTreeTestActiveUnitHealth(float minHealthPercentage, DecisionTreeNode nodeIfTrue, DecisionTreeNode nodeIfFalse) 
            : base(nodeIfTrue, nodeIfFalse)
        {
            _minHealthPercentage = minHealthPercentage;
        }

        protected override bool Test(BattleStateModel battleStateModel = null, CommandArgs commandArgs = null)
        {
            var unitStats = battleStateModel.ActiveUnit.UnitData.UnitStats;
            var healthPercentage = (float)unitStats.CurrentHealth / unitStats.MaxHealth;
            return healthPercentage >= _minHealthPercentage;
        }
    }
}