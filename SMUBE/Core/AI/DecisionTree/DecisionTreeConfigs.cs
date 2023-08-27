using Commands.SpecificCommands.BaseAttack;
using SMUBE.AI.DecisionTree.DecisionNodes;
using SMUBE.AI.DecisionTree.EndNodes;
using SMUBE.Commands.SpecificCommands.BaseBlock;
using SMUBE.Commands.SpecificCommands.DefendAll;
using SMUBE.Commands.SpecificCommands.HealAll;
using SMUBE.Commands.SpecificCommands.HeavyAttack;
using SMUBE.Units.CharacterTypes;

namespace SMUBE.AI.DecisionTree
{
    public static class DecisionTreeConfigs
    {
        public static DecisionTreeNode GetDecisionTreeForArchetype(BaseCharacter character, bool useSimpleBehavior)
        {
            switch (character)
            {
                case Hunter _:
                    return useSimpleBehavior ? GetSimpleHunterDecisionTree() : FakeDecisionTreeExpander(GetHunterDecisionTree());
                case Scholar _:
                    return useSimpleBehavior ? GetSimpleScholarDecisionTree() : FakeDecisionTreeExpander(GetScholarDecisionTree());
                case Squire _:
                    return useSimpleBehavior ? GetSimpleSquireDecisionTree() : FakeDecisionTreeExpander(GetSquireDecisionTree());
                default:
                    return null;
            }    
        }

        public static DecisionTreeNode FakeDecisionTreeExpander(DecisionTreeNode treeToExpand, int count = 0)
        {
            count++;

            if(count > 2)
            {
                return treeToExpand;
            }
            else
            {
                var nextNode = FakeDecisionTreeExpander(treeToExpand, count);
                return new DecisionTreeTestRandom(1D,
                                nextNode,
                                nextNode);
            }
        }

        public static DecisionTreeNode GetHunterDecisionTree()
        {
            return new DecisionTreeTestRandom(0.5D,
                new DecisionTreeActionSimple<BaseAttack>(),
                new DecisionTreeTestCanPerform<HeavyAttack>(
                    new DecisionTreeActionSimple<HeavyAttack>(),
                    new DecisionTreeActionSimple<BaseBlock>()));
        }
        public static DecisionTreeNode GetScholarDecisionTree()
        {
            return new DecisionTreeTestRandom(0.5D,
                new DecisionTreeActionSimple<BaseAttack>(),
                new DecisionTreeTestCanPerform<HealAll>(
                    new DecisionTreeActionSimple<HealAll>(),
                    new DecisionTreeActionSimple<BaseBlock>()));
        }
        public static DecisionTreeNode GetSquireDecisionTree()
        {
            return new DecisionTreeTestRandom(0.5D,
                new DecisionTreeActionSimple<BaseAttack>(),
                new DecisionTreeTestCanPerform<DefendAll>(
                    new DecisionTreeActionSimple<DefendAll>(),
                    new DecisionTreeActionSimple<BaseBlock>()));
        }       
        
        public static DecisionTreeNode GetSimpleHunterDecisionTree()
        {
            return new DecisionTreeTestRandom(0.5D,
                new DecisionTreeActionSimple<BaseAttack>(),
                new DecisionTreeActionSimple<BaseBlock>());
        }
        public static DecisionTreeNode GetSimpleScholarDecisionTree()
        {
            return new DecisionTreeTestRandom(0.5D,
                new DecisionTreeActionSimple<BaseAttack>(),
                new DecisionTreeActionSimple<BaseBlock>());
        }
        public static DecisionTreeNode GetSimpleSquireDecisionTree()
        {
            return new DecisionTreeTestRandom(0.5D,
                new DecisionTreeActionSimple<BaseAttack>(),
                new DecisionTreeActionSimple<BaseBlock>());
        }

    }
}
