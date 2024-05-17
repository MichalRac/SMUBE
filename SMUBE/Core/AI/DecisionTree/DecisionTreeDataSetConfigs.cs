using System.Collections.Generic;

namespace SMUBE.AI.DecisionTree
{
    public static class DecisionTreeDataSetConfigs
    {
        // First setup, no conditional branching
        public static DecisionTreeDataSet CompetentPlayConfig = new DecisionTreeDataSet()
        {
            Probabilities = new Dictionary<string, float>()
            {
                { "SpecialIfPossible", 0.6f },
                { "BaseTree_EnemyInReach_BaseAttackChance", 0.95f },
            },
            Weights = new Dictionary<string, DecisionTreeDataSetWeight>()
            {
                // BASE TREE
                //   enemy in reach
                {"BaseTree_EnemyInReach_AttackWeight-Pref_None", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest", DecisionTreeDataSetWeight.W10()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage", DecisionTreeDataSetWeight.W10()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange", DecisionTreeDataSetWeight.W10()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn", DecisionTreeDataSetWeight.W10()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn", DecisionTreeDataSetWeight.W10()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn", DecisionTreeDataSetWeight.W50()},
                
                {"BaseTree_EnemyInReach_BaseWalk-Pref_None", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully", DecisionTreeDataSetWeight.W10()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively", DecisionTreeDataSetWeight.W10()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition", DecisionTreeDataSetWeight.W25()},
                
                {"BaseTree_EnemyInReach_BaseBlock", DecisionTreeDataSetWeight.W50()},
                
                //   enemy out of reach
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach", DecisionTreeDataSetWeight.W10()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition", DecisionTreeDataSetWeight.W10()},
                
                {"BaseTree_EnemyOutOfReach_BaseBlock", DecisionTreeDataSetWeight.W25()},
                
                
                // EXTENSION TREES
                //    hunter
                {"ExtensionTree-Hunter-HeavyAttack-Pref_None", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn", DecisionTreeDataSetWeight.W50()},
                
                {"ExtensionTree-Hunter-Teleport-Pref_None", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition", DecisionTreeDataSetWeight.W10()},
                
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_None", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams", DecisionTreeDataSetWeight.W10()},
                
                //   scholar
                {"ExtensionTree-Scholar-HealAll", DecisionTreeDataSetWeight.W400()},
                
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Scholar-ShieldPosition-Pref_None", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams", DecisionTreeDataSetWeight.W100()},
                
                //   squire
                {"ExtensionTree-Squire-DefendAll", DecisionTreeDataSetWeight.W400()},
                
                {"ExtensionTree-Squire-Taunt-Pref_None", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Taunt-Pref_Closest", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Squire-Tackle-Pref_None", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_Closest", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange", DecisionTreeDataSetWeight.W100()},
            }
        };
        
        
        // Second setup, conditional branching based on unit's health
        public static DecisionTreeDataSet ConditionalPlayConfig = new DecisionTreeDataSet()
        {
            Probabilities = new Dictionary<string, float>()
            {
                { "HurtThreshold", 0.35f },
                
                { "SpecialIfPossible-HealthyStatus", 0.6f },
                { "SpecialIfPossible-HurtStatus", 0.9f },
                
                { "BaseTree_EnemyInReach_BaseAttackChance-HealthyStatus", 0.95f },
                { "BaseTree_EnemyInReach_BaseAttackChance-HurtStatus", 0.75f },
            },
            Weights = new Dictionary<string, DecisionTreeDataSetWeight>()
            {
                // BASE TREE < < WHEN HEALTHY > >
                //   enemy in reach
                {"BaseTree_EnemyInReach_AttackWeight-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-HealthyStatus", DecisionTreeDataSetWeight.W100()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                
                {"BaseTree_EnemyInReach_BaseWalk-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"BaseTree_EnemyInReach_BaseBlock-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                //   enemy out of reach
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"BaseTree_EnemyOutOfReach_BaseBlock-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                
                // EXTENSION TREES
                //    hunter
                {"ExtensionTree-Hunter-HeavyAttack-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-HealthyStatus", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                
                {"ExtensionTree-Hunter-Teleport-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                //   scholar
                {"ExtensionTree-Scholar-HealAll-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                {"ExtensionTree-Scholar-ShieldPosition-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                
                //   squire
                {"ExtensionTree-Squire-DefendAll-HealthyStatus", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Squire-Taunt-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Taunt-Pref_Closest-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                {"ExtensionTree-Squire-Tackle-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_Closest-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                // BASE TREE < < WHEN HURT > >
                //   enemy in reach
                {"BaseTree_EnemyInReach_AttackWeight-Pref_None-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"BaseTree_EnemyInReach_BaseWalk-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-HurtStatus", DecisionTreeDataSetWeight.W100()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"BaseTree_EnemyInReach_BaseBlock-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                //   enemy out of reach
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"BaseTree_EnemyOutOfReach_BaseBlock-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                
                // EXTENSION TREES
                //    hunter
                {"ExtensionTree-Hunter-HeavyAttack-Pref_None-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-HurtStatus", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-HurtStatus", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Hunter-Teleport-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                //   scholar
                {"ExtensionTree-Scholar-HealAll-HurtStatus", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"ExtensionTree-Scholar-ShieldPosition-Pref_None-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-HurtStatus", DecisionTreeDataSetWeight.W100()},
                
                //   squire
                {"ExtensionTree-Squire-DefendAll-HurtStatus", DecisionTreeDataSetWeight.W200()},
                
                {"ExtensionTree-Squire-Taunt-Pref_None-HurtStatus", DecisionTreeDataSetWeight.W200()},
                {"ExtensionTree-Squire-Taunt-Pref_Closest-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-HurtStatus", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-HurtStatus", DecisionTreeDataSetWeight.W5()},
                
                {"ExtensionTree-Squire-Tackle-Pref_None-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Tackle-Pref_Closest-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-HurtStatus", DecisionTreeDataSetWeight.W5()},
            }
        };

    }
}