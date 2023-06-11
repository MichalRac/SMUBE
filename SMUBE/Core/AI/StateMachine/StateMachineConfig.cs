using Commands.SpecificCommands.BaseAttack;
using SMUBE.AI.DecisionTree;
using SMUBE.Commands.SpecificCommands.BaseBlock;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.StateMachine
{
    public static class StateMachineConfig
    {
        public static StateMachineAIModel GetStateMachineForArchetype(BaseCharacter character)
        {
            switch (character)
            {
                case Hunter _:
                case Squire _:
                case Scholar _:
                    return GetBasicStateMachine();
                default:
                    return null;
            }
        }

        public static StateMachineAIModel GetBasicStateMachine()
        {
            var attackState = new StateMachineState(new BaseAttack());
            var blockState = new StateMachineState(new BaseBlock());

            var attackToBlockTransition = new StateMachineRandomTransition(blockState);
            var blockToAttackTransition = new StateMachineRandomTransition(attackState);

            attackState.InjectTransitions(new List<StateMachineTransition>() { attackToBlockTransition });
            blockState.InjectTransitions(new List<StateMachineTransition>() { blockToAttackTransition });

            return new StateMachineAIModel(attackState);
        }

        public static StateMachineAIModel GetHunterStateMachine()
        {
            return GetBasicStateMachine();
        }

        public static StateMachineAIModel GetScholarStateMachine()
        {
            return GetBasicStateMachine();
        }

        public static StateMachineAIModel GetSquireStateMachine()
        {
            return GetBasicStateMachine();
        }

    }
}
