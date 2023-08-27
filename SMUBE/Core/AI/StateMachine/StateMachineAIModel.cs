using Commands;
using SMUBE.AI.DecisionTree;
using SMUBE.AI.GoalOrientedBehavior;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;

namespace SMUBE.AI.StateMachine
{
    public class StateMachineAIModel : AIModel
    {
        public StateMachineState initialState;
        public StateMachineState currentState;

        public StateMachineAIModel(StateMachineState initialState, bool useSimpleBehavior) : base(useSimpleBehavior)
        {
            this.initialState = initialState;
            currentState = initialState;
        }

        public void ResolveTransitions(BattleStateModel battleStateModel)
        {
            StateMachineTransition triggeredTransition = null;

            foreach (var transition in currentState.Transitions)
            {
                if (transition.IsTriggered(battleStateModel))
                {
                    triggeredTransition = transition;
                    break;
                }
            }

            if (triggeredTransition != null)
            {
                currentState = triggeredTransition.TargetState;
            }
        }

        public override ICommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            ResolveTransitions(battleStateModel);

            return currentState.Command;
        }

        public override CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return CommandArgsHelper.GetDumbCommandArgs(command, battleStateModel, activeUnitIdentifier);
        }
    }
}
