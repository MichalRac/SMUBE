using System;
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
        public static DecisionTreeDataSet DataSetFallback = DecisionTreeDataSetConfigs.CompetentPlayConfig;
        public static DecisionTreeNode GetDecisionTreeForArchetype(BaseCharacter character, bool useSimpleBehavior)
        {
            switch (character)
            {
                case Hunter _:
                    return useSimpleBehavior ? GetBaseDecisionTree(string.Empty, DataSetFallback) : BuildDecisionTree(GetHunterExtensionDecisionTree(string.Empty, DataSetFallback));
                case Scholar _:
                    return useSimpleBehavior ? GetBaseDecisionTree(string.Empty, DataSetFallback) : BuildDecisionTree(GetScholarExtensionDecisionTree(string.Empty, DataSetFallback));
                case Squire _:
                    return useSimpleBehavior ? GetBaseDecisionTree(string.Empty, DataSetFallback) : BuildDecisionTree(GetSquireExtensionDecisionTree(string.Empty, DataSetFallback));
                default:
                    return null;
            }    
        }
        
        public static DecisionTreeNode GetBasicDecisionTreeForArchetype(BaseCharacter character)
        {
            switch (character)
            {
                case Hunter _:
                    return BuildDecisionTree(GetHunterExtensionDecisionTree(string.Empty, DataSetFallback));
                case Scholar _:
                    return BuildDecisionTree(GetScholarExtensionDecisionTree(string.Empty, DataSetFallback));
                case Squire _:
                    return BuildDecisionTree(GetSquireExtensionDecisionTree(string.Empty, DataSetFallback));
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
            var baseTree = GetBaseDecisionTree(string.Empty, DataSetFallback);
            
            return GetSafeAction<TauntedAttack>(
                new DecisionTreeTestAnySpecialActionViable(
                    new DecisionTreeTestRandom(DataSetFallback.Probabilities["SpecialIfPossible"],  // chance of using special if it's possible, todo parametrize 
                        extension,
                        baseTree),
                    baseTree));
        }
        
        public static DecisionTreeNode GetConditionalDecisionTree(BaseCharacter character, DecisionTreeDataSet dataSet = null)
        {
            var conditionalDataSet = dataSet ?? DecisionTreeDataSetConfigs.ConditionalPlayConfig;
            
            float hurtThreshold =  conditionalDataSet.Probabilities["HurtThreshold"];
            
            string healthyWeightGroup = "-HealthyStatus";
            string hurtWeightGroup = "-HurtStatus";
                
            var baseTreeForHealthy = GetBaseDecisionTree(healthyWeightGroup, conditionalDataSet);
            var baseTreeForHurt = GetBaseDecisionTree(hurtWeightGroup, conditionalDataSet);

            DecisionTreeNode healthyExtensionTree;
            DecisionTreeNode hurtExtensionTree;

            switch (character)
            {
                case Hunter _:
                    healthyExtensionTree = GetHunterExtensionDecisionTree(healthyWeightGroup, conditionalDataSet);
                    hurtExtensionTree = GetHunterExtensionDecisionTree(hurtWeightGroup, conditionalDataSet);
                    break;
                case Scholar _:
                    healthyExtensionTree = GetScholarExtensionDecisionTree(healthyWeightGroup, conditionalDataSet);
                    hurtExtensionTree = GetScholarExtensionDecisionTree(hurtWeightGroup, conditionalDataSet);
                    break;
                case Squire _:
                    healthyExtensionTree = GetSquireExtensionDecisionTree(healthyWeightGroup, conditionalDataSet);
                    hurtExtensionTree = GetSquireExtensionDecisionTree(hurtWeightGroup, conditionalDataSet);
                    break;
                default:
                    return null;
            }    

            return GetSafeAction<TauntedAttack>(
                new DecisionTreeTestActiveUnitHealth(hurtThreshold,
                    // true if healthy
                    new DecisionTreeTestAnySpecialActionViable(
                        new DecisionTreeTestRandom(conditionalDataSet.Probabilities["SpecialIfPossible" + healthyWeightGroup], // chance of using special if it's possible WHEN HEALTHY 
                            healthyExtensionTree,
                            baseTreeForHealthy),
                        baseTreeForHealthy), 
                    // false if hurt
                    new DecisionTreeTestAnySpecialActionViable(
                        new DecisionTreeTestRandom(conditionalDataSet.Probabilities["SpecialIfPossible" + hurtWeightGroup], // chance of using special if it's possible WHEN HURT 
                            hurtExtensionTree,
                            baseTreeForHurt),
                        baseTreeForHurt)));
        }
        
        private static DecisionTreeNode GetBaseDecisionTree(string weightGroupSuffix, DecisionTreeDataSet dataSet)
        {
            var enemyInReachDecisionTree = GetRandomAction(dataSet.Probabilities["BaseTree_EnemyInReach_BaseAttackChance" + weightGroupSuffix], // chance of attack if enemy in reach
                new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
                {
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.None), dataSet.Weights["BaseTree_EnemyInReach_AttackWeight-Pref_None" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest), dataSet.Weights["BaseTree_EnemyInReach_AttackWeight-Pref_Closest" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), dataSet.Weights["BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), dataSet.Weights["BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), dataSet.Weights["BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), dataSet.Weights["BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), dataSet.Weights["BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), dataSet.Weights["BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), dataSet.Weights["BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), dataSet.Weights["BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn" + weightGroupSuffix]),
                }),
                new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
                {
                    new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)0), dataSet.Weights["BaseTree_EnemyInReach_BaseWalk-Pref_None" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)1), dataSet.Weights["BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)2), dataSet.Weights["BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)3), dataSet.Weights["BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)4), dataSet.Weights["BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition" + weightGroupSuffix]),
                    new DecisionTreeWeightedSet(new BaseBlock(), dataSet.Weights["BaseTree_EnemyInReach_BaseBlock" + weightGroupSuffix]),
                }));
            
            var enemyOutOfReachDecisionTree = new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)0), dataSet.Weights["BaseTree_EnemyOutOfReach_BaseWalk-Pref_None" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)1), dataSet.Weights["BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)2), dataSet.Weights["BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)3), dataSet.Weights["BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)4), dataSet.Weights["BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new BaseBlock(), dataSet.Weights["BaseTree_EnemyOutOfReach_BaseBlock" + weightGroupSuffix]),
            });
            
            return new DecisionTreeTestCanPerform<BaseAttack>(
                enemyInReachDecisionTree,
                enemyOutOfReachDecisionTree);
        }
        
        public static DecisionTreeNode GetHunterExtensionDecisionTree(string weightGroupSuffix, DecisionTreeDataSet dataSet)
        {
            // viable actions: Heavy Attack, Teleport, Raise Obstacle

            return new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.None), dataSet.Weights["ExtensionTree-Hunter-HeavyAttack-Pref_None" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest), dataSet.Weights["ExtensionTree-Hunter-HeavyAttack-Pref_Closest" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), dataSet.Weights["ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), dataSet.Weights["ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), dataSet.Weights["ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), dataSet.Weights["ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), dataSet.Weights["ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), dataSet.Weights["ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), dataSet.Weights["ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), dataSet.Weights["ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn" + weightGroupSuffix]),

                new DecisionTreeWeightedSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.None), dataSet.Weights["ExtensionTree-Hunter-Teleport-Pref_None" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach), dataSet.Weights["ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully), dataSet.Weights["ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively), dataSet.Weights["ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition), dataSet.Weights["ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition" + weightGroupSuffix]),

                new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.None), dataSet.Weights["ExtensionTree-Hunter-RaiseObstacle-Pref_None" + weightGroupSuffix]),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly), 15),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly), 15),
                new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), dataSet.Weights["ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy" + weightGroupSuffix]),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach), 5),
                new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenEnemies), dataSet.Weights["ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams), dataSet.Weights["ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams" + weightGroupSuffix]),
            });
        }
        
        public static DecisionTreeNode GetScholarExtensionDecisionTree(string weightGroupSuffix, DecisionTreeDataSet dataSet)
        {
            // viable actions: Heal All, Shield Position, Lower Enemy Defense
            
            return new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new HealAll(), dataSet.Weights["ExtensionTree-Scholar-HealAll" + weightGroupSuffix]),

                new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.None), dataSet.Weights["ExtensionTree-Scholar-LowerEnemyDefense-Pref_None" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.Closest), dataSet.Weights["ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), dataSet.Weights["ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), dataSet.Weights["ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage" + weightGroupSuffix]),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), dataSet.Weights["ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange" + weightGroupSuffix]),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),

                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.None), dataSet.Weights["ExtensionTree-Scholar-ShieldPosition-Pref_None" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly), dataSet.Weights["ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly), dataSet.Weights["ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), dataSet.Weights["ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach), dataSet.Weights["ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams), dataSet.Weights["ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams" + weightGroupSuffix]),

            });
        }
        
        public static DecisionTreeNode GetSquireExtensionDecisionTree(string weightGroupSuffix, DecisionTreeDataSet dataSet)
        {
            // viable actions: Defend All, Taunt, Tackle
            
            return new DecisionTreeWeightedActionSelection(new List<DecisionTreeWeightedSet>
            {
                // todo parametrize weights
                new DecisionTreeWeightedSet(new DefendAll(), dataSet.Weights["ExtensionTree-Squire-DefendAll" + weightGroupSuffix]),
                
                new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.None), dataSet.Weights["ExtensionTree-Squire-Taunt-Pref_None" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.Closest), dataSet.Weights["ExtensionTree-Squire-Taunt-Pref_Closest" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), dataSet.Weights["ExtensionTree-Squire-Taunt-Pref_LeastHpPoints" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), dataSet.Weights["ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage" + weightGroupSuffix]),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), dataSet.Weights["ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange" + weightGroupSuffix]),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),

                new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.None), dataSet.Weights["ExtensionTree-Squire-Tackle-Pref_None" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.Closest), dataSet.Weights["ExtensionTree-Squire-Tackle-Pref_Closest" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), dataSet.Weights["ExtensionTree-Squire-Tackle-Pref_LeastHpPoints" + weightGroupSuffix]),
                new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), dataSet.Weights["ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage" + weightGroupSuffix]),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), dataSet.Weights["ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange" + weightGroupSuffix]),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),

            });
        }       
    }
}