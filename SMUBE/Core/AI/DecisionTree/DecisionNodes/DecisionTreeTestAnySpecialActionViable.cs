using SMUBE.BattleState;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree.DecisionNodes
{
    public class DecisionTreeTestAnySpecialActionViable : DecisionTreeTest
    {
        private const int BaseCommandCount = 3; // walk, attack, block
        
        public DecisionTreeTestAnySpecialActionViable(DecisionTreeNode nodeIfTrue, DecisionTreeNode nodeIfFalse) 
            : base(nodeIfTrue, nodeIfFalse) { }

        protected override bool Test(BattleStateModel battleStateModel, CommandArgs commandArgs = null)
        {
            return battleStateModel.ActiveUnit.UnitCommandProvider.ViableCommands.Count > BaseCommandCount;
        }
    }
}