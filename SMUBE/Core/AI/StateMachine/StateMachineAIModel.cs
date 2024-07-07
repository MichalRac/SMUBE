﻿using SMUBE.AI.DecisionTree;
using SMUBE.AI.GoalOrientedBehavior;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Args;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;

namespace SMUBE.AI.StateMachine
{
    public class StateMachineAIModel : AIModel
    {
        public StateMachineState initialState;
        public StateMachineState currentState;

        public bool IsCompetent { get; private set; }

        public StateMachineAIModel(StateMachineState initialState, bool useSimpleBehavior) : base(useSimpleBehavior)
        {
            this.initialState = initialState;
            currentState = initialState;
        }

        public StateMachineAIModel AsCompetent()
        {
            IsCompetent = true;
            return this;
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

        public override BaseCommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            ResolveTransitions(battleStateModel);

            return currentState.GetCommand(battleStateModel);
        }

        public override CommandArgs GetCommandArgs(BaseCommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return command.GetSuggestedPseudoRandomArgs(battleStateModel);
        }
    }
}
