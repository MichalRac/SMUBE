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
        
        public static DecisionTreeDataSet ComplexPlayConfig = new DecisionTreeDataSet()
        {
            Probabilities = new Dictionary<string, float>()
            {
                { "HurtThreshold", 0.35f },
                
                { "SpecialIfPossible-Advantage-HealthyStatus", 0.6f },
                { "SpecialIfPossible-Advantage-HurtStatus", 0.9f },
                { "SpecialIfPossible-Disadvantage-HealthyStatus", 0.6f },
                { "SpecialIfPossible-Disadvantage-HurtStatus", 0.9f },
                
                { "BaseTree_EnemyInReach_BaseAttackChance-Advantage-HealthyStatus", 0.95f },
                { "BaseTree_EnemyInReach_BaseAttackChance-Advantage-HurtStatus", 0.75f },
                
                { "BaseTree_EnemyInReach_BaseAttackChance-Disadvantage-HealthyStatus", 0.95f },
                { "BaseTree_EnemyInReach_BaseAttackChance-Disadvantage-HurtStatus", 0.75f },
            },
            Weights = new Dictionary<string, DecisionTreeDataSetWeight>()
            {
                // BASE TREE < < TEAM ADVANTAGE + HEALTHY > >
                //   enemy in reach
                {"BaseTree_EnemyInReach_AttackWeight-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W100()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                
                {"BaseTree_EnemyInReach_BaseWalk-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"BaseTree_EnemyInReach_BaseBlock-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                //   enemy out of reach
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"BaseTree_EnemyOutOfReach_BaseBlock-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                
                // EXTENSION TREES
                //    hunter
                {"ExtensionTree-Hunter-HeavyAttack-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                
                {"ExtensionTree-Hunter-Teleport-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                //   scholar
                {"ExtensionTree-Scholar-HealAll-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                {"ExtensionTree-Scholar-ShieldPosition-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                
                //   squire
                {"ExtensionTree-Squire-DefendAll-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Squire-Taunt-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Taunt-Pref_Closest-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                {"ExtensionTree-Squire-Tackle-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_Closest-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-Advantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                // BASE TREE < < TEAM ADVANTAGE + UNHEALTHY > >
                //   enemy in reach
                {"BaseTree_EnemyInReach_AttackWeight-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"BaseTree_EnemyInReach_BaseWalk-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-Advantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"BaseTree_EnemyInReach_BaseBlock-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                //   enemy out of reach
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"BaseTree_EnemyOutOfReach_BaseBlock-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                
                // EXTENSION TREES
                //    hunter
                {"ExtensionTree-Hunter-HeavyAttack-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Hunter-Teleport-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-Advantage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-Advantage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                //   scholar
                {"ExtensionTree-Scholar-HealAll-Advantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"ExtensionTree-Scholar-ShieldPosition-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-Advantage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-Advantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-Advantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                
                //   squire
                {"ExtensionTree-Squire-DefendAll-Advantage-HurtStatus", DecisionTreeDataSetWeight.W200()},
                
                {"ExtensionTree-Squire-Taunt-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.W200()},
                {"ExtensionTree-Squire-Taunt-Pref_Closest-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-Advantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-Advantage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                
                {"ExtensionTree-Squire-Tackle-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Tackle-Pref_Closest-Advantage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-Advantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                
                
                // BASE TREE < < TEAM DISADVANTAGE + HEALTHY > >
                //   enemy in reach
                {"BaseTree_EnemyInReach_AttackWeight-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W100()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                
                {"BaseTree_EnemyInReach_BaseWalk-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"BaseTree_EnemyInReach_BaseBlock-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                //   enemy out of reach
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"BaseTree_EnemyOutOfReach_BaseBlock-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                
                // EXTENSION TREES
                //    hunter
                {"ExtensionTree-Hunter-HeavyAttack-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                
                {"ExtensionTree-Hunter-Teleport-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                //   scholar
                {"ExtensionTree-Scholar-HealAll-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W10()},
                
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                {"ExtensionTree-Scholar-ShieldPosition-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Disabled()},
                
                //   squire
                {"ExtensionTree-Squire-DefendAll-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Squire-Taunt-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Taunt-Pref_Closest-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                {"ExtensionTree-Squire-Tackle-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_Closest-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.W25()},
                
                // BASE TREE < < TEAM DISADVANTAGE + UNHEALTHY > >
                //   enemy in reach
                {"BaseTree_EnemyInReach_AttackWeight-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"BaseTree_EnemyInReach_BaseWalk-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"BaseTree_EnemyInReach_BaseBlock-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                //   enemy out of reach
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"BaseTree_EnemyOutOfReach_BaseBlock-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                
                // EXTENSION TREES
                //    hunter
                {"ExtensionTree-Hunter-HeavyAttack-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Hunter-Teleport-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Disabled()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                //   scholar
                {"ExtensionTree-Scholar-HealAll-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                
                {"ExtensionTree-Scholar-ShieldPosition-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W50()},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                
                //   squire
                {"ExtensionTree-Squire-DefendAll-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W200()},
                
                {"ExtensionTree-Squire-Taunt-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W200()},
                {"ExtensionTree-Squire-Taunt-Pref_Closest-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W100()},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                
                {"ExtensionTree-Squire-Tackle-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Tackle-Pref_Closest-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W25()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
                {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.W5()},
            }
        };

    }
}