using SMUBE.BattleState;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree.EndNodes.SecondaryChecks
{
    public delegate bool DecisionTreeSecondaryCheck(BattleStateModel battleStateModel, CommandArgs commandArgs);

    public static class DecisionTreeSecondaryCheckHelper
    {
        public static bool AnyEnemyInRange(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            return true;
        }
    }
}