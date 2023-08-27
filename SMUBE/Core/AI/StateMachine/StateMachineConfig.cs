using Commands.SpecificCommands.BaseAttack;
using SMUBE.AI.DecisionTree;
using SMUBE.Commands.SpecificCommands.BaseBlock;
using SMUBE.Commands.SpecificCommands.DefendAll;
using SMUBE.Commands.SpecificCommands.HealAll;
using SMUBE.Commands.SpecificCommands.HeavyAttack;
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
                    return GetHunterStateMachine();
                case Squire _:
                    return GetSquireStateMachine();
                case Scholar _:
                    return GetScholarStateMachine();
                default:
                    return GetBasicStateMachine();
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
            var attackState = new StateMachineState(new BaseAttack());
            var blockState = new StateMachineState(new BaseBlock());
            var heavyAttackState = new StateMachineState(new HeavyAttack());

            var transitionToAttack = new StateMachineRandomTransition(attackState, 0.5f);
            var transitionToBlock = new StateMachineRandomTransition(blockState, 0.5f);
            var transitionToHeavyAttack = new StateMachineCanPerformTransition<HeavyAttack>(heavyAttackState);

            attackState.InjectTransitions(new List<StateMachineTransition>() { transitionToBlock, transitionToHeavyAttack });
            blockState.InjectTransitions(new List<StateMachineTransition>() { transitionToAttack, transitionToHeavyAttack });
            heavyAttackState.InjectTransitions(new List<StateMachineTransition>() { transitionToAttack, transitionToBlock});

            return new StateMachineAIModel(attackState);
        }

        public static StateMachineAIModel GetScholarStateMachine()
        {
            var attackState = new StateMachineState(new BaseAttack());
            var blockState = new StateMachineState(new BaseBlock());
            var healAllState = new StateMachineState(new HealAll());

            var transitionToAttack = new StateMachineRandomTransition(attackState, 0.5f);
            var transitionToBlock = new StateMachineRandomTransition(blockState, 0.5f);
            var transitionToHealAll = new StateMachineCanPerformTransition<HealAll>(healAllState);

            attackState.InjectTransitions(new List<StateMachineTransition>() { transitionToBlock, transitionToHealAll });
            blockState.InjectTransitions(new List<StateMachineTransition>() { transitionToAttack, transitionToHealAll });
            healAllState.InjectTransitions(new List<StateMachineTransition>() { transitionToAttack, transitionToBlock });

            return new StateMachineAIModel(attackState);
        }

        public static StateMachineAIModel GetSquireStateMachine()
        {
            var attackState = new StateMachineState(new BaseAttack());
            var blockState = new StateMachineState(new BaseBlock());
            var defendAllState = new StateMachineState(new DefendAll());

            var transitionToAttack = new StateMachineRandomTransition(attackState, 0.5f);
            var transitionToBlock = new StateMachineRandomTransition(blockState, 0.5f);
            var transitionToDefendAll = new StateMachineCanPerformTransition<DefendAll>(defendAllState);

            attackState.InjectTransitions(new List<StateMachineTransition>() { transitionToBlock, transitionToDefendAll });
            blockState.InjectTransitions(new List<StateMachineTransition>() { transitionToAttack, transitionToDefendAll });
            defendAllState.InjectTransitions(new List<StateMachineTransition>() { transitionToAttack, transitionToBlock });

            return new StateMachineAIModel(attackState);
        }

    }
}
