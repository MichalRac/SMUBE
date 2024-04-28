using System.Collections.Generic;
using SMUBE.AI.DecisionTree.DecisionNodes;
using SMUBE.AI.DecisionTree.EndNodes;
using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.BaseAttack;
using SMUBE.Commands.SpecificCommands.BaseBlock;
using SMUBE.Commands.SpecificCommands.BaseWalk;
using SMUBE.Commands.SpecificCommands.DefendAll;
using SMUBE.Commands.SpecificCommands.HealAll;
using SMUBE.Commands.SpecificCommands.HeavyAttack;
using SMUBE.Commands.SpecificCommands.LowerEnemyDefense;
using SMUBE.Commands.SpecificCommands.RaiseObstacle;
using SMUBE.Commands.SpecificCommands.ShieldPosition;
using SMUBE.Commands.SpecificCommands.Tackle;
using SMUBE.Commands.SpecificCommands.Taunt;
using SMUBE.Commands.SpecificCommands.Teleport;
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
                    return useSimpleBehavior ? GetBaseDecisionTree() : BuildDecisionTree(GetHunterExtensionDecisionTree());
                case Scholar _:
                    return useSimpleBehavior ? GetBaseDecisionTree() : BuildDecisionTree(GetScholarExtensionDecisionTree());
                case Squire _:
                    return useSimpleBehavior ? GetBaseDecisionTree() : BuildDecisionTree(GetSquireExtensionDecisionTree());
                default:
                    return null;
            }    
        }

        public static DecisionTreeNode FakeDecisionTreeExpander(DecisionTreeNode treeToExpand, int count = 0)
        {
            const int maxStepsToExpand = 2;
            count++;

            if(count > maxStepsToExpand)
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
        
        private static DecisionTreeNode GetSafeAction<T, TFallback>()
            where T : BaseCommand, new()
            where TFallback : BaseCommand, new()
        {
            return new DecisionTreeTestCanPerform<T>(
                new DecisionTreeActionSimple<T>(),
                new DecisionTreeActionSimple<TFallback>());
        }
        private static DecisionTreeNode GetSafeAction<T>(DecisionTreeNode fallback)
            where T : BaseCommand, new()
        {
            return new DecisionTreeTestCanPerform<T>(
                new DecisionTreeActionSimple<T>(),
                fallback);
        }
        
        private static DecisionTreeNode GetRandomAction(double chance, DecisionTreeNode actionA, DecisionTreeNode actionB)
        {
            return new DecisionTreeTestRandom(chance, actionA, actionB);
        }
        
        private static DecisionTreeNode GetRandomAction(DecisionTreeNode actionA, DecisionTreeNode actionB, DecisionTreeNode actionC)
        {
            return new DecisionTreeTestRandom(1/3D, 
                actionA, 
                new DecisionTreeTestRandom(1/3D, 
                    actionB, 
                    actionC));
        }
        
        private static DecisionTreeNode BuildDecisionTree(DecisionTreeNode extension)
        {
            var baseTree = GetBaseDecisionTree();
            
            return GetSafeAction<TauntedAttack>(
                new DecisionTreeTestAnySpecialActionViable(
                    new DecisionTreeTestRandom(0.4D,  // chance of using special if it's possible, todo parametrize 
                        extension,
                        baseTree),
                    baseTree));
        }
        
        private static DecisionTreeNode GetBaseDecisionTree()
        {
            var enemyInReachDecisionTree = GetRandomAction(0.8D, // chance of attack  todo parametrize
                new DecisionTreeActionSimple<BaseAttack>(),
                new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
                {
                    // todo parametrize weights
                    new DecisionTreeWeightedSet(new BaseWalk(), 10),
                    new DecisionTreeWeightedSet(new BaseBlock(), 10),
                }));
            
            var enemyOutOfReachDecisionTree = new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new BaseWalk(), 20),
                new DecisionTreeWeightedSet(new BaseBlock(), 20),
            });
            
            return new DecisionTreeTestCanPerform<BaseAttack>(
                enemyInReachDecisionTree,
                enemyOutOfReachDecisionTree);
        }
        
        public static DecisionTreeNode GetHunterExtensionDecisionTree()
        {
            // viable actions: Heavy Attack, Teleport, Raise Obstacle

            return new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new HeavyAttack(), 10),
                new DecisionTreeWeightedSet(new Teleport(), 10),
                new DecisionTreeWeightedSet(new RaiseObstacle(), 10),
            });
        }
        
        public static DecisionTreeNode GetScholarExtensionDecisionTree()
        {
            // viable actions: Heal All, Shield Position, Lower Enemy Defense
            
            return new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new HealAll(), 10),
                new DecisionTreeWeightedSet(new ShieldPosition(), 10),
                new DecisionTreeWeightedSet(new LowerEnemyDefense(), 10),
            });
        }
        
        public static DecisionTreeNode GetSquireExtensionDecisionTree()
        {
            // viable actions: Defend All, Taunt, Tackle
            
            return new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new DefendAll(), 10),
                new DecisionTreeWeightedSet(new Taunt(), 10),
                new DecisionTreeWeightedSet(new Tackle(), 10),
            });
        }       
    }
}
