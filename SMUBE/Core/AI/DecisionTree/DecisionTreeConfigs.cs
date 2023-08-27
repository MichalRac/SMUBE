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
        public static DecisionTreeNode GetDecisionTreeForArchetype(BaseCharacter character)
        {
            switch (character)
            {
                case Hunter _:
                    return GetHunterDecisionTree();
                case Scholar _:
                    return GetScholarDecisionTree();
                case Squire _:
                    return GetSquireDecisionTree();
                default:
                    return null;
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

    }
}
