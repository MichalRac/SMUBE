using System.Collections.Generic;
using SMUBE.AI.DecisionTree.DecisionNodes;
using SMUBE.AI.DecisionTree.EndNodes;
using SMUBE.Commands;
using SMUBE.Commands.Args;
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
                    new DecisionTreeTestRandom(0.6D,  // chance of using special if it's possible, todo parametrize 
                        extension,
                        baseTree),
                    baseTree));
        }
        
        private static DecisionTreeNode GetBaseDecisionTree()
        {
            var enemyInReachDecisionTree = GetRandomAction(0.95D, // chance of attack  todo parametrize
                new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
                {
                    // todo parametrize weights
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.None), 5),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest), 5),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 50),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 5),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 5),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),
                }),
                new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
                {
                    // todo parametrize weights
                    new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)0), 1),
                    new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)1), 10),
                    new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)2), 5),
                    new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)3), 5),
                    new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)4), 15),
                    new DecisionTreeWeightedSet(new BaseBlock(), 50),
                }));
            
            var enemyOutOfReachDecisionTree = new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)0), 1),
                new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)1), 5),
                new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)2), 30),
                new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)3), 15),
                new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)4), 5),
                new DecisionTreeWeightedSet(new BaseBlock(), 25),
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
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.None), 5),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest), 5),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 50),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 5),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 5),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),

                new DecisionTreeWeightedSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.None), 1),
                new DecisionTreeWeightedSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach), 5),
                new DecisionTreeWeightedSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully), 30),
                new DecisionTreeWeightedSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively), 15),
                new DecisionTreeWeightedSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition), 5),

                new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.None), 1),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly), 15),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly), 15),
                new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), 15),
                new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), 15),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach), 5),
                new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams), 5),
            });
        }
        
        public static DecisionTreeNode GetScholarExtensionDecisionTree()
        {
            // viable actions: Heal All, Shield Position, Lower Enemy Defense
            
            return new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new HealAll(), 500),

                new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.None), 20),
                new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.Closest), 100),
                new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 100),
                new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 100),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 100),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),

                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.None), 20),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly), 100),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly), 100),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), 100),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), 100),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach), 100),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams), 100),

            });
        }
        
        public static DecisionTreeNode GetSquireExtensionDecisionTree()
        {
            // viable actions: Defend All, Taunt, Tackle
            
            return new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new DefendAll(), 500),
                
                new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.None), 20),
                new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.Closest), 100),
                new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 100),
                new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 100),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 100),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),

                new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.None), 20),
                new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.Closest), 100),
                new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 100),
                new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 100),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 100),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),

            });
        }       
    }
}
