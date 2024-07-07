using SMUBE.AI.StateMachine;
using SMUBE.BattleState;
using SMUBE.Commands;

namespace SMUBE.AI.Common
{
    public class SimpleWeightedSelectionSetState : StateMachineState
    {
        private readonly SimpleWeightedCommandActionSelection _weightedCommandActionSelection;

        public SimpleWeightedSelectionSetState(SimpleWeightedCommandActionSelection weightedCommandActionSelection)
        {
            _weightedCommandActionSelection = weightedCommandActionSelection;
        }

        public override BaseCommand GetCommand(BattleStateModel battleStateModel)
        {
            return _weightedCommandActionSelection.GetCommand(battleStateModel);
        }
    }
}