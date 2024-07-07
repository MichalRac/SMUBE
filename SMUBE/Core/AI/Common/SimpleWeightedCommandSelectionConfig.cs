using System.Collections.Generic;
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

namespace SMUBE.AI.Common
{
    public static class SimpleWeightedCommandSelectionConfig
    {
        public static SimpleWeightedCommandActionSelection OnFarAwayActionSelection()
        {
            return new SimpleWeightedCommandActionSelection(new List<SimpleWeightedCommandSet>
            {
                // BASE
                new SimpleWeightedCommandSet(new TauntedAttack(), 100),
                
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 0),
                
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.None), 5),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach), 0),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully), 50),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively), 50),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition), 50),
                
                new SimpleWeightedCommandSet(new BaseBlock(), 5),
                
                // HUNTER
                /*
                new StateMachineWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.None), 0),
                new StateMachineWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest), 0),
                new StateMachineWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 0),
                new StateMachineWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 0),
                new StateMachineWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 0),
                new StateMachineWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 0),
                new StateMachineWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 0),
                new StateMachineWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 0),
                new StateMachineWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 0),
                new StateMachineWeightedSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 0),
                */
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.None), 5),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach), 0),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully), 10),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively), 10),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition), 10),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.None), 5),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly), 15),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly), 15),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), 10),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach), 5),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenEnemies), 10),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams), 10),
                
                // SCHOLAR
                new SimpleWeightedCommandSet(new HealAll(), 5),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.Closest), 10),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 5),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 10),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly), 10),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly), 10),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), 0),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach), 10),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams), 5),
                
                // SQUIRE
                new SimpleWeightedCommandSet(new DefendAll(), 10),
                
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.None), 5),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.Closest), 5),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 5),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.Closest), 0),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 0),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 0),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 0),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),

            });
        }
        
        public static SimpleWeightedCommandActionSelection OnInCombatActionSelection()
        {
            return new SimpleWeightedCommandActionSelection(new List<SimpleWeightedCommandSet>
            {
                // BASE
                new SimpleWeightedCommandSet(new TauntedAttack(), 100),

                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.None), 5),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest), 5),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 10),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 10),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 25),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 5),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 5),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 5),
                
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach), 5),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully), 0),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively), 0),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition), 10),
                
                new SimpleWeightedCommandSet(new BaseBlock(), 25),
                
                // HUNTER
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.None), 5),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest), 5),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 5),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 5),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 10),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 5),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 5),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 0),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 0),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 5),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach), 5),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully), 0),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively), 0),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition), 5),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.None), 0),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly), 15),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly), 15),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), 10),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach), 5),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenEnemies), 10),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams), 10),
                
                // SCHOLAR
                new SimpleWeightedCommandSet(new HealAll(), 25),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.Closest), 25),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 0),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 0),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 25),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly), 25),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly), 25),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), 5),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach), 25),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams), 5),
                
                // SQUIRE
                new SimpleWeightedCommandSet(new DefendAll(), 25),
                
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.None), 5),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.Closest), 0),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 25),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.None), 5),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.Closest), 25),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 10),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 10),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),

            });
        }
        
        public static SimpleWeightedCommandActionSelection OnHurtActionSelection()
        {
            return new SimpleWeightedCommandActionSelection(new List<SimpleWeightedCommandSet>
            {
                // BASE
                new SimpleWeightedCommandSet(new TauntedAttack(), 100),

                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.None), 25),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 0),
                new SimpleWeightedCommandSet(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),
                
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach), 50),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully), 0),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively), 0),
                new SimpleWeightedCommandSet(new BaseWalk().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition), 10),
                
                new SimpleWeightedCommandSet(new BaseBlock(), 10),
                
                // HUNTER
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest), 0),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 0),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 50),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 0),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 10),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 0),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 0),
                new SimpleWeightedCommandSet(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 10),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach), 50),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully), 0),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively), 0),
                new SimpleWeightedCommandSet(new Teleport().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition), 25),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.None), 0),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly), 15),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly), 15),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), 25),
                //new DecisionTreeWeightedSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach), 5),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenEnemies), 0),
                new SimpleWeightedCommandSet(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams), 25),
                
                // SCHOLAR
                new SimpleWeightedCommandSet(new HealAll(), 50),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.Closest), 25),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 0),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 0),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 25),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly), 25),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly), 50),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy), 0),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach), 25),
                new SimpleWeightedCommandSet(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams), 5),
                
                // SQUIRE
                new SimpleWeightedCommandSet(new DefendAll(), 50),
                
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.None), 25),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.Closest), 25),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 0),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 0),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 25),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.None), 0),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.Closest), 25),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints), 25),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage), 50),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt), 50),
                new SimpleWeightedCommandSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange), 25),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn), 50),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn), 5),
                //new DecisionTreeWeightedSet(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn), 50),
            });
        }

    }
}