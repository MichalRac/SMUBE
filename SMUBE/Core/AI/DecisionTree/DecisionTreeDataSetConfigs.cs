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
            Weights = new Dictionary<string, int>()
            {
                // BASE TREE
                //   enemy in reach
                {"BaseTree_EnemyInReach_AttackWeight-Pref_None", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints", 50},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt", 50},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn", 50},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn", 50},
                
                {"BaseTree_EnemyInReach_BaseWalk-Pref_None", 1},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach", 10},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully", 5},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively", 5},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition", 15},
                
                {"BaseTree_EnemyInReach_BaseBlock", 50},
                
                //   enemy out of reach
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None", 1},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach", 5},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully", 30},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively", 15},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition", 5},
                
                {"BaseTree_EnemyOutOfReach_BaseBlock", 25},
                
                
                // EXTENSION TREES
                //    hunter
                {"ExtensionTree-Hunter-HeavyAttack-Pref_None", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints", 50},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt", 50},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn", 50},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn", 50},
                
                {"ExtensionTree-Hunter-Teleport-Pref_None", 1},
                {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach", 5},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully", 30},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively", 15},
                {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition", 5},
                
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_None", 1},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy", 15},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies", 15},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams", 5},
                
                //   scholar
                {"ExtensionTree-Scholar-HealAll", 500},
                
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None", 20},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest", 100},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints", 100},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage", 100},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange", 100},
                
                {"ExtensionTree-Scholar-ShieldPosition-Pref_None", 20},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly", 100},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly", 100},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy", 100},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach", 100},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams", 100},
                
                //   squire
                {"ExtensionTree-Squire-DefendAll", 500},
                
                {"ExtensionTree-Squire-Taunt-Pref_None", 20},
                {"ExtensionTree-Squire-Taunt-Pref_Closest", 100},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints", 100},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage", 100},
                {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange", 100},
                
                {"ExtensionTree-Squire-Tackle-Pref_None", 20},
                {"ExtensionTree-Squire-Tackle-Pref_Closest", 100},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints", 100},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage", 100},
                {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange", 100},
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
            Weights = new Dictionary<string, int>()
            {
                // BASE TREE < < WHEN HEALTHY > >
                //   enemy in reach
                {"BaseTree_EnemyInReach_AttackWeight-Pref_None-HealthyStatus", 1},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-HealthyStatus", 100},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-HealthyStatus", 25},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-HealthyStatus", 25},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-HealthyStatus", 25},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-HealthyStatus", 1},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-HealthyStatus", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-HealthyStatus", 1},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-HealthyStatus", 1},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-HealthyStatus", 1},
                
                {"BaseTree_EnemyInReach_BaseWalk-Pref_None-HealthyStatus", 1},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-HealthyStatus", 5},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-HealthyStatus", 5},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-HealthyStatus", 25},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-HealthyStatus", 10},
                
                {"BaseTree_EnemyInReach_BaseBlock-HealthyStatus", 25},
                
                //   enemy out of reach
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-HealthyStatus", 1},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-HealthyStatus", 1},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-HealthyStatus", 25},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-HealthyStatus", 25},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-HealthyStatus", 10},
                
                {"BaseTree_EnemyOutOfReach_BaseBlock-HealthyStatus", 25},
                
                
                // EXTENSION TREES
                //    hunter
                {"ExtensionTree-Hunter-HeavyAttack-Pref_None-HealthyStatus", 1},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-HealthyStatus", 100},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-HealthyStatus", 25},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-HealthyStatus", 25},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-HealthyStatus", 25},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-HealthyStatus", 1},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-HealthyStatus", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-HealthyStatus", 1},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-HealthyStatus", 1},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-HealthyStatus", 1},
                
                {"ExtensionTree-Hunter-Teleport-Pref_None-HealthyStatus", 1},
                {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-HealthyStatus", 5},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-HealthyStatus", 5},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-HealthyStatus", 25},
                {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-HealthyStatus", 10},
                
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-HealthyStatus", 1},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-HealthyStatus", 15},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-HealthyStatus", 15},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-HealthyStatus", 15},
                
                //   scholar
                {"ExtensionTree-Scholar-HealAll-HealthyStatus", 10},
                
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-HealthyStatus", 1},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-HealthyStatus", 25},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-HealthyStatus", 5},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-HealthyStatus", 5},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-HealthyStatus", 25},
                
                {"ExtensionTree-Scholar-ShieldPosition-Pref_None-HealthyStatus", 1},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-HealthyStatus", 25},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-HealthyStatus", 25},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-HealthyStatus", 1},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-HealthyStatus", 25},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-HealthyStatus", 1},
                
                //   squire
                {"ExtensionTree-Squire-DefendAll-HealthyStatus", 75},
                
                {"ExtensionTree-Squire-Taunt-Pref_None-HealthyStatus", 25},
                {"ExtensionTree-Squire-Taunt-Pref_Closest-HealthyStatus", 25},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-HealthyStatus", 5},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-HealthyStatus", 5},
                {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-HealthyStatus", 25},
                
                {"ExtensionTree-Squire-Tackle-Pref_None-HealthyStatus", 25},
                {"ExtensionTree-Squire-Tackle-Pref_Closest-HealthyStatus", 25},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-HealthyStatus", 25},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-HealthyStatus", 25},
                {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-HealthyStatus", 25},
                
                // BASE TREE < < WHEN HURT > >
                //   enemy in reach
                {"BaseTree_EnemyInReach_AttackWeight-Pref_None-HurtStatus", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-HurtStatus", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-HurtStatus", 50},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-HurtStatus", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-HurtStatus", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-HurtStatus", 50},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-HurtStatus", 50},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-HurtStatus", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-HurtStatus", 5},
                {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-HurtStatus", 50},
                
                {"BaseTree_EnemyInReach_BaseWalk-Pref_None-HurtStatus", 1},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-HurtStatus", 100},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-HurtStatus", 5},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-HurtStatus", 5},
                {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-HurtStatus", 50},
                
                {"BaseTree_EnemyInReach_BaseBlock-HurtStatus", 50},
                
                //   enemy out of reach
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-HurtStatus", 1},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-HurtStatus", 5},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-HurtStatus", 50},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-HurtStatus", 5},
                {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-HurtStatus", 50},
                
                {"BaseTree_EnemyOutOfReach_BaseBlock-HurtStatus", 50},
                
                
                // EXTENSION TREES
                //    hunter
                {"ExtensionTree-Hunter-HeavyAttack-Pref_None-HurtStatus", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-HurtStatus", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-HurtStatus", 50},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-HurtStatus", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-HurtStatus", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-HurtStatus", 75},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-HurtStatus", 100},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-HurtStatus", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-HurtStatus", 5},
                {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-HurtStatus", 100},
                
                {"ExtensionTree-Hunter-Teleport-Pref_None-HurtStatus", 1},
                {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-HurtStatus", 5},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-HurtStatus", 50},
                {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-HurtStatus", 5},
                {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-HurtStatus", 50},
                
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-HurtStatus", 1},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-HurtStatus", 25},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-HurtStatus", 25},
                {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-HurtStatus", 50},
                
                //   scholar
                {"ExtensionTree-Scholar-HealAll-HurtStatus", 100},
                
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-HurtStatus", 50},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-HurtStatus", 50},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-HurtStatus", 5},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-HurtStatus", 5},
                {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-HurtStatus", 50},
                
                {"ExtensionTree-Scholar-ShieldPosition-Pref_None-HurtStatus", 5},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-HurtStatus", 25},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-HurtStatus", 50},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-HurtStatus", 5},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-HurtStatus", 50},
                {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-HurtStatus", 10},
                
                //   squire
                {"ExtensionTree-Squire-DefendAll-HurtStatus", 250},
                
                {"ExtensionTree-Squire-Taunt-Pref_None-HurtStatus", 250},
                {"ExtensionTree-Squire-Taunt-Pref_Closest-HurtStatus", 5},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-HurtStatus", 100},
                {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-HurtStatus", 25},
                {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-HurtStatus", 5},
                
                {"ExtensionTree-Squire-Tackle-Pref_None-HurtStatus", 5},
                {"ExtensionTree-Squire-Tackle-Pref_Closest-HurtStatus", 25},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-HurtStatus", 5},
                {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-HurtStatus", 5},
                {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-HurtStatus", 5},
            }
        };

    }
}